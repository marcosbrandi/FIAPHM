using FIAPHM.Database;
using FIAPHM.DTO;
using FIAPHM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FIAPHM.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "Paciente")]
public class PacienteController : ControllerBase
{
    private readonly FIAPDBContext _context;

    public PacienteController(FIAPDBContext context)
    {
        _context = context;
    }

    // Endpoint para agendar consulta (com autenticação)
    [HttpPost("agendar")]
    [Authorize(Roles = "Paciente")] // Exige autenticação e o papel "Paciente"
    public async Task<IActionResult> AgendarConsulta([FromBody] ConsultaDTO consultaDTO)
    {
        // Verifica se o paciente existe
        var paciente = await _context.Pacientes.FindAsync(consultaDTO.PacienteId);
        if (paciente == null)
        {
            return BadRequest("Paciente não encontrado.");
        }

        // Verifica se o médico existe
        var medico = await _context.Medicos.FindAsync(consultaDTO.MedicoId);
        if (medico == null)
        {
            return BadRequest("Médico não encontrado.");
        }

        // Verifica se o horário está disponível na agenda do médico
        var horarioDisponivel = await _context.Agendas
            .AnyAsync(a => a.MedicoId == consultaDTO.MedicoId && a.DataHora == consultaDTO.DataHora);

        if (!horarioDisponivel)
        {
            return BadRequest("Horário indisponível para o médico selecionado.");
        }

        // Verifica se já existe uma consulta agendada no mesmo horário para o médico
        var consultaExistente = await _context.Consultas
            .AnyAsync(c => c.MedicoId == consultaDTO.MedicoId && c.DataHora == consultaDTO.DataHora);

        if (consultaExistente)
        {
            return BadRequest("Já existe uma consulta agendada para este horário.");
        }

        // Cria uma nova consulta a partir do DTO
        var consulta = new Consulta
        {
            DataHora = consultaDTO.DataHora,
            PacienteId = consultaDTO.PacienteId,
            MedicoId = consultaDTO.MedicoId
        };

        // Adiciona a consulta ao banco de dados
        _context.Consultas.Add(consulta);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Consulta agendada com sucesso!", Consulta = consulta });
    }

    // Endpoint para cadastrar um paciente
    [HttpPost("cadastrar")]
    public async Task<IActionResult> CadastrarPaciente([FromBody] PacienteDTO pacienteDTO)
    {
        // Verifica se o email já está cadastrado
        if (_context.Pacientes.Any(p => p.Email == pacienteDTO.Email))
        {
            return BadRequest("Email já cadastrado.");
        }

        // Cria um novo paciente a partir do DTO
        var paciente = new Paciente
        {
            Nome = pacienteDTO.Nome,
            CPF = pacienteDTO.CPF,
            Email = pacienteDTO.Email,
            Senha = pacienteDTO.Senha // Em um cenário real, a senha deve ser hasheada
        };

        // Adiciona o paciente ao banco de dados
        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Paciente cadastrado com sucesso!", Paciente = paciente });
    }
}
