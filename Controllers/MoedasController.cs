using Microsoft.AspNetCore.Mvc;
using ConversorMoedas.Models;
using ConversorMoedas.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MoedasController : ControllerBase
{
    private readonly IMoedaService _service;
    private readonly IValidator<ConversaoRequest> _validator;
    private readonly ILogger<MoedasController> _logger;

    public MoedasController(IMoedaService service, IValidator<ConversaoRequest> validator, ILogger<MoedasController> logger)
    {
        _service = service;
        _validator = validator;
        _logger = logger;
    }

    [HttpPost("converter")]
    public async Task<ActionResult<ConversaoResponse>> Converter([FromBody] ConversaoRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                title = "One or more validation errors occurred.",
                status = 400,
                errors = validationResult.ToDictionary()
            });
        }

        try
        {
            var resultado = await _service.ConverterMoedaAsync(request);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro na conversão: {ex.Message}");
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpGet("moedas")]
    public async Task<ActionResult<List<Moeda>>> ListarMoedas()
    {
        var moedas = await _service.ListarMoedasAsync();
        return Ok(moedas);
    }

    [HttpGet("historico")]
    public async Task<ActionResult<List<Conversao>>> Historico()
    {
        var historico = await _service.ObterHistoricoAsync();
        return Ok(historico);
    }

    [HttpPost("atualizar-taxas")]
    public async Task<IActionResult> AtualizarTaxas()
    {
        try
        {
            await _service.AtualizarTaxasAsync();
            return Ok(new { mensagem = "Taxas atualizadas com sucesso!" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao atualizar taxas: {ex.Message}");
            return BadRequest(new { erro = ex.Message });
        }
    }
}