using System.Text.RegularExpressions;
using CatalogoJogos.Api.Models;
using CatalogoJogos.Api.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CatalogoJogos.Api.Repositories;

/// <summary>
/// Única camada que conhece o MongoDB. Recebe o IMongoDatabase (registrado no DI)
/// e o nome da coleção via IOptions&lt;MongoDbSettings&gt;.
/// </summary>
public class JogoRepository : IJogoRepository
{
    private readonly IMongoCollection<Jogo> _jogos;

    public JogoRepository(IMongoDatabase database, IOptions<MongoDbSettings> settings)
    {
        _jogos = database.GetCollection<Jogo>(settings.Value.JogosCollectionName);
    }

    public async Task<List<Jogo>> ObterTodosAsync() =>
        await _jogos.Find(Builders<Jogo>.Filter.Empty)
                    .SortBy(j => j.Titulo)
                    .ToListAsync();

    public async Task<Jogo?> ObterPorIdAsync(string id) =>
        await _jogos.Find(Builders<Jogo>.Filter.Eq(j => j.Id, id))
                    .FirstOrDefaultAsync();

    public async Task CriarAsync(Jogo jogo) =>
        await _jogos.InsertOneAsync(jogo); // o driver preenche jogo.Id com o ObjectId gerado

    public async Task<bool> AtualizarAsync(string id, Jogo jogo)
    {
        jogo.Id = id;
        var resultado = await _jogos.ReplaceOneAsync(
            Builders<Jogo>.Filter.Eq(j => j.Id, id), jogo);

        return resultado.MatchedCount > 0;
    }

    public async Task<bool> RemoverAsync(string id)
    {
        var resultado = await _jogos.DeleteOneAsync(
            Builders<Jogo>.Filter.Eq(j => j.Id, id));

        return resultado.DeletedCount > 0;
    }

    /// <summary>
    /// Desafio 1: jogos de uma plataforma com preço &lt;= precoMaximo.
    /// Combina dois filtros com Builders&lt;Jogo&gt;.Filter.And().
    /// A plataforma é comparada por igualdade exata, mas sem diferenciar maiúsculas/minúsculas
    /// (regex ancorada ^...$ com o texto escapado).
    /// </summary>
    public async Task<List<Jogo>> BuscarPorPlataformaEPrecoAsync(string plataforma, decimal precoMaximo)
    {
        var builder = Builders<Jogo>.Filter;

        var filtroPlataforma = builder.Regex(
            j => j.Plataforma,
            new BsonRegularExpression($"^{Regex.Escape(plataforma.Trim())}$", "i"));

        var filtroPreco = builder.Lte(j => j.Preco, precoMaximo);

        var filtro = builder.And(filtroPlataforma, filtroPreco);

        return await _jogos.Find(filtro)
                           .SortBy(j => j.Preco)
                           .ToListAsync();
    }

    /// <summary>
    /// Desafio 2: pipeline de agregação (IAggregateFluent).
    ///   $group   -> agrupa por plataforma, conta títulos e soma (preco * estoque)
    ///   $project -> renomeia _id para "plataforma" e remove o _id
    ///   $sort    -> ordena pelo valor total do inventário (maior primeiro)
    /// </summary>
    public async Task<List<RelatorioEstoquePlataforma>> ObterRelatorioEstoqueAsync()
    {
        var group = new BsonDocument
        {
            { "_id", "$plataforma" },
            { "quantidadeTitulos", new BsonDocument("$sum", 1) },
            { "totalUnidadesEstoque", new BsonDocument("$sum", "$estoque") },
            { "valorTotalInventario", new BsonDocument("$sum",
                new BsonDocument("$multiply", new BsonArray { "$preco", "$estoque" })) }
        };

        var project = new BsonDocument
        {
            { "_id", 0 },
            { "plataforma", "$_id" },
            { "quantidadeTitulos", 1 },
            { "totalUnidadesEstoque", 1 },
            { "valorTotalInventario", 1 }
        };

        return await _jogos.Aggregate()
                           .Group(group)
                           .Project(project)
                           .Sort(new BsonDocument("valorTotalInventario", -1))
                           .As<RelatorioEstoquePlataforma>()
                           .ToListAsync();
    }
}
