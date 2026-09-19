namespace ConversorMoedas.Models;

public class ConversaoResponse
{
    public string MoedaOrigtem { get; set; } = string.Empty;
    public string MoedaDestino { get; set; } = string.Empty;
    public decimal ValorOriginal { get; set; }
    public decimal ValorConvertido { get; set; }
    public DateTime DataConversao { get; set; }
}
