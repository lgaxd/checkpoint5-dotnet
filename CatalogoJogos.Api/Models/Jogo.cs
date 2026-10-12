using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CatalogoJogos.Api.Models;

/// <summary>
/// Documento que representa um jogo na coleção "Jogos" do MongoDB.
/// </summary>
public class Jogo
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("titulo")]
    public string Titulo { get; set; } = string.Empty;

    [BsonElement("plataforma")]
    public string Plataforma { get; set; } = string.Empty;

    [BsonElement("genero")]
    public string Genero { get; set; } = string.Empty;

    // Decimal128 garante precisão monetária e permite cálculos ($multiply/$sum) na agregação.
    [BsonElement("preco")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Preco { get; set; }

    [BsonElement("anoLancamento")]
    public int AnoLancamento { get; set; }

    [BsonElement("estoque")]
    public int Estoque { get; set; }
}
