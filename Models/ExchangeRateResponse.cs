namespace ConversorMoedas.Models;

public class ExchangeRateResponse
{
    public string Result { get; set; } = string.Empty;
    public string Base { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public long Time_last_updated { get; set; }
    public Dictionary<string, decimal> Rates { get; set; }
}
