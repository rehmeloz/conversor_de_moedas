namespace ConversorMoedas.Models;

public class ConversaoResponse
{
    public string MoedaOrigem { get; set; } = string.Empty;
    public string MoedaDestino { get; set; } = string.Empty;
    public decimal ValorOriginal { get; set; }
    public decimal ValorConvertido { get; set; }
    public decimal Taxa { get; set; }
    public DateTime DataConversao { get; set; }
}
