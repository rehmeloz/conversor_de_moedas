namespace ConversorMoedas.Services;

public interface IExchangeRateService
{
    Task<Dictionary<string, decimal>> ObterTaxasAsync(string moedaBase = "USD");
}
