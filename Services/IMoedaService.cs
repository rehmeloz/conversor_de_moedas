namespace ConversorMoedas.Services;
using ConversorMoedas.Models;

public interface IMoedaService
{
    Task<ConversaoResponse> ConverterMoedaAsync (ConversaoRequest request);
    Task<List<Moeda>> ListarMoedasAsync();
    Task AtualizarTaxasAsync();
    Task<List<Conversao>> ObterHistoricoAsync();
}
