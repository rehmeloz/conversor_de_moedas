namespace ConversorMoedas.Services;
using System.Net.Http.Json;
using ConversorMoedas.Data;
using ConversorMoedas.Models;
using Microsoft.EntityFrameworkCore;

public class MoedaService : IMoedaService
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly ILogger<MoedaService> _logger;

    public MoedaService(AppDbContext context, HttpClient httpClient, ILogger<MoedaService> logger)
    {
        _context = context;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ConversaoResponse> ConverterMoedaAsync(ConversaoRequest request)
    {
        var moedaOrigem = await _context.Moedas.FirstOrDefaultAsync(m => m.Codigo == request.De);

        var moedaDestino = await _context.Moedas.FirstOrDefaultAsync(m => m.Codigo == request.Para);

        if (moedaOrigem == null || moedaDestino == null)
            throw new Exception("Moeda não encontrada");

        decimal taxa = moedaDestino.Taxa / moedaOrigem.Taxa;
        decimal resultado = request.Valor * taxa;

        var conversao = new Conversao
        { 
            MoedaOrigem = request.De,
            MoedaDestino = request.Para,
            Valor = request.Valor,
            Resultado = resultado,
            TaxaUsada = taxa,
            DataConversao = DateTime.Now
        };

        _context.Conversoes.Add(conversao);
        await _context.SaveChangesAsync();

        return new ConversaoResponse
        {
            MoedaOrigem = request.De,
            MoedaDestino = request.Para,
            ValorOriginal = request.Valor,
            ValorConvertido = resultado,
            Taxa = taxa,
            DataConversao = DateTime.Now
        };
    }

    public async Task<List<Moeda>> ListarMoedasAsync()
    {
        return await _context.Moedas.ToListAsync();
    }

    public async Task AtualizarTaxasAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<dynamic>(
                "https://api.exchangerate-api.com/v4/latest/USD");

            _logger.LogInformation("Taxas atualizadas com sucesso!");
        }
        catch(Exception ex)
        {
            _logger.LogError($"Erro ao atualizar taxas: {ex.Message}");
        }
    }

    public async Task<List<Conversao>> ObterHistoricoAsync()
    {
        return await _context.Conversoes
            .OrderByDescending(c => c.DataConversao)
            .Take(50)
            .ToListAsync();
    }
}
