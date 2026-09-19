namespace ConversorMoedas.Models;

public class Moeda
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public decimal Taxa { get; set; }
    public DateTime UltimaAtualizacao { get; set; }
}
