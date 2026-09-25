using System.Text.Json;
using ConversorMoedas.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ConversorMoedas.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ExchangeRateService> _logger;

        public ExchangeRateService(HttpClient httpClient, IConfiguration configuration, IMemoryCache cache, ILogger<ExchangeRateService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Dictionary<string, decimal>> ObterTaxasAsync(string moedaBase = "USD")
        {
            string cacheKey = $"exchange_rates_{moedaBase}";

            if (_cache.TryGetValue(cacheKey, out Dictionary<string, decimal>? cachedRates) && cachedRates != null)
            {
                _logger.LogInformation($"Taxas obtidas do CACHE para {moedaBase}");
                return cachedRates;
            }

            try
            {
                var baseUrl = _configuration["ExchangeRateApi:BaseUrl"] ?? "";
                var apiKey = _configuration["ExchangeRateApi:ApiKey"] ?? "";

                _logger.LogInformation($"Obtendo taxas para: {moedaBase}");

                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new Exception("BaseUrl não configurada em appsettings.json");
                }

                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new Exception("ApiKey não configurada em appsettings.json");
                }

                var url = $"{baseUrl}/{moedaBase}?apikey={apiKey}";
                _logger.LogInformation($"Chamando API: {url}");

                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Status Code: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"API retornou erro {response.StatusCode}: {content}");
                    throw new Exception($"API retornou erro: {response.StatusCode}");
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var exchangeResponse = JsonSerializer.Deserialize<ExchangeRateResponse>(content, options);

                if (exchangeResponse?.Rates == null || exchangeResponse.Rates.Count == 0)
                {
                    _logger.LogError($"Resposta vazia ou inválida da API");
                    throw new Exception("Resposta vazia ou inválida da API");
                }

                _logger.LogInformation($"{exchangeResponse.Rates.Count} moedas obtidas da API");
                _logger.LogInformation($"Data: {exchangeResponse.Date}");

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(cacheKey, exchangeResponse.Rates, cacheOptions);

                _logger.LogInformation($"Taxas cacheadas por 1 hora");

                return exchangeResponse.Rates;
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError($"Erro de conexão HTTP: {httpEx.Message}");
                throw new Exception($"Erro de conexão: {httpEx.Message}");
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError($"Erro ao fazer parse do JSON: {jsonEx.Message}");
                throw new Exception($"Erro no JSON: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro: {ex.GetType().Name} - {ex.Message}");
                throw;
            }
        }
    }
}