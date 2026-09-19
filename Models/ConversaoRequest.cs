namespace ConversorMoedas.Models;

public class ConversaoRequest
{
    public string De { get; set; } = string.Empty;
    public string Para { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}
