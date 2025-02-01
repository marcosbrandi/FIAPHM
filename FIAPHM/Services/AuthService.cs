using FIAPHM.Database;
using Microsoft.EntityFrameworkCore;
namespace FIAPHM.Services;

public class AuthService
{
    private readonly FIAPDBContext _context;

    public AuthService(FIAPDBContext context)
    {
        _context = context;
    }

    public async Task<string> Authenticate(string email, string senha)
    {
        var medico = await _context.Medicos.FirstOrDefaultAsync(m => m.Email == email && m.Senha == senha);
        var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.Email == email && p.Senha == senha);

        if (medico != null) return "Medico";
        if (paciente != null) return "Paciente";

        return null;
    }
}