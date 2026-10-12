using System.ComponentModel.DataAnnotations;
using CatalogoJogos.Api.Models;

namespace CatalogoJogos.Api.DTOs;

/// <summary>
/// Corpo das requisições POST e PUT. As validações geram 400 Bad Request
/// automaticamente graças ao atributo [ApiController].
/// </summary>
public class JogoRequest
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(150, MinimumLength = 1)]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "A plataforma é obrigatória.")]
    [StringLength(80, MinimumLength = 1)]
    public string Plataforma { get; set; } = string.Empty;

    [Required(ErrorMessage = "O gênero é obrigatório.")]
    [StringLength(50, MinimumLength = 1)]
    public string Genero { get; set; } = string.Empty;

    [Range(typeof(decimal), "0", "999999", ErrorMessage = "O preço deve ser maior ou igual a 0.")]
    public decimal Preco { get; set; }

    [Range(1950, 2100, ErrorMessage = "O ano de lançamento deve estar entre 1950 e 2100.")]
    public int AnoLancamento { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "O estoque não pode ser negativo.")]
    public int Estoque { get; set; }

    public Jogo ToEntity(string? id = null) => new()
    {
        Id = id,
        Titulo = Titulo.Trim(),
        Plataforma = Plataforma.Trim(),
        Genero = Genero.Trim(),
        Preco = Preco,
        AnoLancamento = AnoLancamento,
        Estoque = Estoque
    };
}
