using CatalogoJogos.Api.Models;

namespace CatalogoJogos.Api.Repositories;

public interface IJogoRepository
{
    Task<List<Jogo>> ObterTodosAsync();
    Task<Jogo?> ObterPorIdAsync(string id);
    Task CriarAsync(Jogo jogo);
    Task<bool> AtualizarAsync(string id, Jogo jogo);
    Task<bool> RemoverAsync(string id);

    // Desafio 1 – filtro por plataforma e faixa de preço
    Task<List<Jogo>> BuscarPorPlataformaEPrecoAsync(string plataforma, decimal precoMaximo);

    // Desafio 2 – agregação por plataforma
    Task<List<RelatorioEstoquePlataforma>> ObterRelatorioEstoqueAsync();
}
