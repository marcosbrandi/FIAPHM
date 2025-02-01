namespace FIAPHM.Models;
public class Consulta
{
    public int Id { get; set; }
    public DateTime DataHora { get; set; }
    public int PacienteId { get; set; }
    public Paciente Paciente { get; set; }
    public int MedicoId { get; set; }
    public Medico Medico { get; set; }
}