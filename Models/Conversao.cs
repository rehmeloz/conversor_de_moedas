namespace ConversorMoedas.Models;

public class Conversao
{
    public int Id { get; set; }
    public string MoedaOrigem { get; set; } = string.Empty;
    public string MoedaDestino { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public decimal Resultado { get; set; }
    public decimal TaxaUsada { get; set; }
    public DateTime DataConversao { get; set; }
}
