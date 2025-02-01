namespace FIAPHM.Models;

public class Agenda
{
    public int Id { get; set; }
    public DateTime DataHora { get; set; }
    public int MedicoId { get; set; }
    public Medico Medico { get; set; }
}