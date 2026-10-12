using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CatalogoJogos.Api.Models;

/// <summary>
/// Resultado do pipeline de agregação: uma linha por plataforma.
/// </summary>
[BsonIgnoreExtraElements]
public class RelatorioEstoquePlataforma
{
    [BsonElement("plataforma")]
    public string Plataforma { get; set; } = string.Empty;

    [BsonElement("quantidadeTitulos")]
    public int QuantidadeTitulos { get; set; }

    [BsonElement("totalUnidadesEstoque")]
    public int TotalUnidadesEstoque { get; set; }

    [BsonElement("valorTotalInventario")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal ValorTotalInventario { get; set; }
}
