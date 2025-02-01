using FIAPHM.Database;
using FIAPHM.DTO;
using FIAPHM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FIAPHM.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "Medico")]
public class MedicoController : ControllerBase
{
    private readonly FIAPDBContext _context;

    public MedicoController(FIAPDBContext context)
    {
        _context = context;
    }

    [HttpPost("agenda")]
    [Authorize(Roles = "Medico")]
    public async Task<IActionResult> CadastrarAgenda([FromBody] Agenda agenda)
    {
        // Verifica se o médico existe
        var medico = await _context.Medicos.FindAsync(agenda.MedicoId);
        if (medico == null)
        {
            return BadRequest("Médico não encontrado.");
        }

        // Verifica se a data e hora estão no futuro
        if (agenda.DataHora <= DateTime.Now)
        {
            return BadRequest("A data e hora devem ser futuras.");
        }

        // Verifica se já existe uma agenda no mesmo horário para o médico
        if (_context.Agendas.Any(a => a.MedicoId == agenda.MedicoId && a.DataHora == agenda.DataHora))
        {
            return BadRequest("Já existe uma agenda cadastrada para este horário.");
        }

        // Adiciona a agenda ao banco de dados
        _context.Agendas.Add(agenda);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Agenda cadastrada com sucesso!", Agenda = agenda });
    }

    [HttpPost("cadastrar")]
    public async Task<IActionResult> CadastrarMedico([FromBody] MedicoDTO medicoDTO)
    {
        // Verifica se o email já está cadastrado
        if (_context.Medicos.Any(m => m.Email == medicoDTO.Email))
        {
            return BadRequest("Email já cadastrado.");
        }

        // Cria um novo médico a partir do DTO
        var medico = new Medico
        {
            Nome = medicoDTO.Nome,
            CPF = medicoDTO.CPF,
            CRM = medicoDTO.CRM,
            Email = medicoDTO.Email,
            Senha = medicoDTO.Senha // Em um cenário real, a senha deve ser hasheada
        };

        // Adiciona o médico ao banco de dados
        _context.Medicos.Add(medico);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Médico cadastrado com sucesso!", Medico = medico });
    }

    // Endpoint para listar todos os médicos
    [HttpGet("listar")]
    [Authorize]
    public async Task<IActionResult> ListarMedicos([FromQuery] string nome)
    {
        var query = _context.Medicos.AsQueryable();

        // Aplica o filtro por nome, se fornecido
        if (!string.IsNullOrEmpty(nome))
        {
            query = query.Where(m => m.Nome.Contains(nome));
        }

        // Projeta os campos desejados
        var medicos = await query
            .Select(m => new
            {
                m.Id,
                m.Nome,
                m.CRM,
                m.Email,
                m.CPF
            })
            .ToListAsync();

        return Ok(medicos);
    }

    // Endpoint para listar agendamentos por médico (com autenticação)
    [HttpGet("agendamentos")]
    [Authorize(Roles = "Medico")]
    public async Task<IActionResult> ListarAgendamentosPorMedico([FromQuery] DateTime? data)
    {
        // Obtém o ID do médico autenticado a partir do token JWT
        var medicoId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var query = _context.Consultas
            .Where(c => c.MedicoId == medicoId);

        // Aplica o filtro por data, se fornecido
        if (data.HasValue)
        {
            query = query.Where(c => c.DataHora.Date == data.Value.Date);
        }

        // Projeta os campos desejados
        var agendamentos = await query
            .Select(c => new
            {
                c.Id,
                c.DataHora,
                Paciente = new
                {
                    c.Paciente.Id,
                    c.Paciente.Nome,
                    c.Paciente.Email
                }
            })
            .ToListAsync();

        return Ok(agendamentos);
    }

    // Endpoint para listar a agenda do médico (com autenticação)
    [HttpGet("agenda")]
    [Authorize(Roles = "Medico")] // Exige autenticação e o papel "Medico"
    public async Task<IActionResult> ListarAgendaDoMedico([FromQuery] int mes, [FromQuery] int ano)
    {
        // Validação do mês
        if (mes < 1 || mes > 12)
        {
            return BadRequest("O mês deve estar entre 1 e 12.");
        }

        // Validação do ano
        if (ano < DateTime.Now.Year - 1 || ano > DateTime.Now.Year + 1)
        {
            return BadRequest("O ano deve ser um valor válido.");
        }

        // Obtém o ID do médico autenticado a partir do token JWT
        var medicoId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // Define o primeiro e o último dia do mês
        var primeiroDiaDoMes = new DateTime(ano, mes, 1);
        var ultimoDiaDoMes = primeiroDiaDoMes.AddMonths(1).AddDays(-1);

        // Busca os agendamentos do médico no banco de dados para o mês e ano especificados
        var agendamentos = await _context.Consultas
            .Where(c => c.MedicoId == medicoId && c.DataHora >= primeiroDiaDoMes && c.DataHora <= ultimoDiaDoMes)
            .Select(c => new
            {
                c.DataHora,
                PacienteNome = c.Paciente.Nome
            })
            .ToListAsync();

        // Cria a estrutura da agenda
        var agenda = new List<object>();

        // Itera sobre todos os dias do mês
        for (var dia = primeiroDiaDoMes; dia <= ultimoDiaDoMes; dia = dia.AddDays(1))
        {
            // Busca os agendamentos para o dia atual
            var agendamentosDoDia = agendamentos
                .Where(a => a.DataHora.Date == dia.Date)
                .OrderBy(a => a.DataHora)
                .Select(a => new
                {
                    Hora = a.DataHora.ToString("HH:mm"),
                    PacienteNome = a.PacienteNome
                })
                .ToList();

            // Adiciona o dia à agenda
            agenda.Add(new
            {
                DiaDoMes = dia.Day,
                DiaDaSemana = dia.ToString("dddd", new System.Globalization.CultureInfo("pt-BR")),
                Agendamentos = agendamentosDoDia
            });
        }

        return Ok(agenda);
    }
}
