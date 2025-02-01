namespace FIAPHM.Models;

public class Paciente
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string CPF { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public ICollection<Consulta> Consultas { get; set; }
}
