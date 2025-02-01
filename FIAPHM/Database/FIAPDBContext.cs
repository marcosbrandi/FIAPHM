using FIAPHM.Models;
using Microsoft.EntityFrameworkCore;

namespace FIAPHM.Database
{
    public class FIAPDBContext : DbContext
    {
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Agenda> Agendas { get; set; }
        public DbSet<Consulta> Consultas { get; set; }

        public FIAPDBContext(DbContextOptions<FIAPDBContext> options) : base(options) { }
    }
}
