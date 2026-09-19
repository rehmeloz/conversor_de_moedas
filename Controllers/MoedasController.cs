using Microsoft.AspNetCore.Mvc;
using ConversorMoedas.Models;
using ConversorMoedas.Services;

[ApiController]
[Route("api/[controller]")]
public class MoedasController : ControllerBase
{
    private readonly IMoedaService _service;

    public MoedasController(IMoedaService service)
    {
        _service = service;
    }

    [HttpPost("converter")]
    public async Task<ActionResult<ConversaoResponse>> Converter([FromBody] ConversaoRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var resultado = await _service.ConverterMoedaAsync(request);
        return Ok(resultado);
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
