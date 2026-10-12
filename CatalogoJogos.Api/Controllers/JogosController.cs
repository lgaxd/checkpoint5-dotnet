using CatalogoJogos.Api.DTOs;
using CatalogoJogos.Api.Models;
using CatalogoJogos.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoJogos.Api.Controllers;

[ApiController]
[Route("api/jogos")]
[Produces("application/json")]
public class JogosController : ControllerBase
{
    private readonly IJogoService _service;

    public JogosController(IJogoService service)
    {
        _service = service;
    }

    /// <summary>Cadastra um novo jogo.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Jogo), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Jogo>> Criar([FromBody] JogoRequest request)
    {
        var jogo = await _service.CriarAsync(request);
        return CreatedAtAction(nameof(BuscarPorId), new { id = jogo.Id }, jogo);
    }

    /// <summary>Lista todos os jogos cadastrados.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<Jogo>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Jogo>>> Listar()
    {
        return Ok(await _service.ListarAsync());
    }

    /// <summary>Busca um jogo pelo ID.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Jogo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Jogo>> BuscarPorId(string id)
    {
        if (!_service.IdValido(id))
            return BadRequest(new { mensagem = "ID inválido. Informe um ObjectId com 24 caracteres hexadecimais." });

        var jogo = await _service.BuscarPorIdAsync(id);
        if (jogo is null)
            return NotFound(new { mensagem = $"Jogo com ID '{id}' não encontrado." });

        return Ok(jogo);
    }

    /// <summary>Atualiza os dados de um jogo existente.</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Jogo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Jogo>> Atualizar(string id, [FromBody] JogoRequest request)
    {
        if (!_service.IdValido(id))
            return BadRequest(new { mensagem = "ID inválido. Informe um ObjectId com 24 caracteres hexadecimais." });

        var jogo = await _service.AtualizarAsync(id, request);
        if (jogo is null)
            return NotFound(new { mensagem = $"Jogo com ID '{id}' não encontrado." });

        return Ok(jogo);
    }

    /// <summary>Remove um jogo pelo ID.</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(string id)
    {
        if (!_service.IdValido(id))
            return BadRequest(new { mensagem = "ID inválido. Informe um ObjectId com 24 caracteres hexadecimais." });

        var removido = await _service.RemoverAsync(id);
        if (!removido)
            return NotFound(new { mensagem = $"Jogo com ID '{id}' não encontrado." });

        return NoContent();
    }

    /// <summary>
    /// Desafio 1: jogos de uma plataforma com preço menor ou igual ao informado.
    /// Ex.: GET /api/jogos/busca?plataforma=PlayStation 3&amp;precoMaximo=100
    /// </summary>
    [HttpGet("busca")]
    [ProducesResponseType(typeof(List<Jogo>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<Jogo>>> Buscar(
        [FromQuery] string? plataforma,
        [FromQuery] decimal? precoMaximo)
    {
        if (string.IsNullOrWhiteSpace(plataforma))
            return BadRequest(new { mensagem = "O parâmetro 'plataforma' é obrigatório." });

        if (precoMaximo is null || precoMaximo < 0)
            return BadRequest(new { mensagem = "O parâmetro 'precoMaximo' é obrigatório e deve ser maior ou igual a 0." });

        return Ok(await _service.BuscarPorPlataformaEPrecoAsync(plataforma, precoMaximo.Value));
    }

    /// <summary>
    /// Desafio 2: relatório agrupado por plataforma com quantidade de títulos
    /// e valor total do inventário (Preco x Estoque).
    /// </summary>
    [HttpGet("relatorio-estoque")]
    [ProducesResponseType(typeof(List<RelatorioEstoquePlataforma>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RelatorioEstoquePlataforma>>> RelatorioEstoque()
    {
        return Ok(await _service.GerarRelatorioEstoqueAsync());
    }
}
