using ConversorMoedas.Data;
using ConversorMoedas.Models;
using Microsoft.EntityFrameworkCore;

namespace ConversorMoedas.Services
{
    public class MoedaService : IMoedaService
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly ILogger<MoedaService> _logger;
        private readonly IExchangeRateService _exchangeRateService;

        public MoedaService(
            AppDbContext context,
            HttpClient httpClient,
            ILogger<MoedaService> logger,
            IExchangeRateService exchangeRateService)
        {
            _context = context;
            _httpClient = httpClient;
            _logger = logger;
            _exchangeRateService = exchangeRateService;
        }

        public async Task<ConversaoResponse> ConverterMoedaAsync(ConversaoRequest request)
        {
            var moedaOrigem = await _context.Moedas
                .FirstOrDefaultAsync(m => m.Codigo == request.De);

            var moedaDestino = await _context.Moedas
                .FirstOrDefaultAsync(m => m.Codigo == request.Para);

            if (moedaOrigem == null || moedaDestino == null)
                throw new Exception("Moeda não encontrada");

            var taxas = await _exchangeRateService.ObterTaxasAsync("USD");

            decimal taxaOrigem = taxas.ContainsKey(request.De) ? taxas[request.De] : moedaOrigem.Taxa;
            decimal taxaDestino = taxas.ContainsKey(request.Para) ? taxas[request.Para] : moedaDestino.Taxa;

            decimal taxa = taxaDestino / taxaOrigem;
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

            _logger.LogInformation($"Conversão realizada: {request.Valor} {request.De} = {resultado} {request.Para}");

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
                var taxas = await _exchangeRateService.ObterTaxasAsync("USD");

                var moedas = await _context.Moedas.ToListAsync();

                foreach (var moeda in moedas)
                {
                    if (taxas.ContainsKey(moeda.Codigo))
                    {
                        moeda.Taxa = taxas[moeda.Codigo];
                        moeda.UltimaAtualizacao = DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Taxas atualizadas com sucesso a partir da API!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao atualizar taxas: {ex.Message}");
                throw;
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
}