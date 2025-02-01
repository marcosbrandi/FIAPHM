namespace FIAPHM.DTO;

public class ConsultaDTO
{
    public DateTime DataHora { get; set; } // Data e hora da consulta
    public int PacienteId { get; set; }    // ID do paciente
    public int MedicoId { get; set; }      // ID do médico
}