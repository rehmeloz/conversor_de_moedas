using Microsoft.AspNetCore.Mvc;
using ConversorMoedas.Models;
using ConversorMoedas.Services;
using FluentValidation;

[ApiController]
[Route("api/[controller]")]
public class MoedasController : ControllerBase
{
    private readonly IMoedaService _service;
    private readonly IValidator<ConversaoRequest> _validator;

    public MoedasController(IMoedaService service, IValidator<ConversaoRequest> validator)
    {
        _service = service;
        _validator = validator;
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
        await _service.AtualizarTaxasAsync();
        return Ok("Taxas atualizadas");
    }
}