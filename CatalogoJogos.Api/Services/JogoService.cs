using CatalogoJogos.Api.DTOs;
using CatalogoJogos.Api.Models;
using CatalogoJogos.Api.Repositories;
using MongoDB.Bson;

namespace CatalogoJogos.Api.Services;

/// <summary>
/// Regras de negócio. Não conhece HTTP nem detalhes de consulta do Mongo:
/// conversa apenas com IJogoRepository.
/// </summary>
public class JogoService : IJogoService
{
    private readonly IJogoRepository _repository;

    public JogoService(IJogoRepository repository)
    {
        _repository = repository;
    }

    public bool IdValido(string id) => ObjectId.TryParse(id, out _);

    public Task<List<Jogo>> ListarAsync() => _repository.ObterTodosAsync();

    public Task<Jogo?> BuscarPorIdAsync(string id) => _repository.ObterPorIdAsync(id);

    public async Task<Jogo> CriarAsync(JogoRequest request)
    {
        var jogo = request.ToEntity();
        await _repository.CriarAsync(jogo);
        return jogo;
    }

    public async Task<Jogo?> AtualizarAsync(string id, JogoRequest request)
    {
        var jogo = request.ToEntity(id);
        var atualizado = await _repository.AtualizarAsync(id, jogo);
        return atualizado ? jogo : null;
    }

    public Task<bool> RemoverAsync(string id) => _repository.RemoverAsync(id);

    public Task<List<Jogo>> BuscarPorPlataformaEPrecoAsync(string plataforma, decimal precoMaximo) =>
        _repository.BuscarPorPlataformaEPrecoAsync(plataforma, precoMaximo);

    public Task<List<RelatorioEstoquePlataforma>> GerarRelatorioEstoqueAsync() =>
        _repository.ObterRelatorioEstoqueAsync();
}
