using CatalogoJogos.Api.DTOs;
using CatalogoJogos.Api.Models;

namespace CatalogoJogos.Api.Services;

public interface IJogoService
{
    Task<List<Jogo>> ListarAsync();
    Task<Jogo?> BuscarPorIdAsync(string id);
    Task<Jogo> CriarAsync(JogoRequest request);
    Task<Jogo?> AtualizarAsync(string id, JogoRequest request);
    Task<bool> RemoverAsync(string id);
    Task<List<Jogo>> BuscarPorPlataformaEPrecoAsync(string plataforma, decimal precoMaximo);
    Task<List<RelatorioEstoquePlataforma>> GerarRelatorioEstoqueAsync();

    /// <summary>Retorna true se o texto é um ObjectId válido (24 caracteres hexadecimais).</summary>
    bool IdValido(string id);
}
