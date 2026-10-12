# Catálogo de Jogos – .NET 8 + MongoDB

Web API para gerenciar um catálogo de jogos de videogame, com CRUD completo, filtro por plataforma/preço e relatório de estoque usando o pipeline de agregação do MongoDB.

**Integrantes:**

- Lucas Grillo Alcântara – RM 561413
- Pedro Peres Benitez – RM 561792

## Arquitetura

```
CatalogoJogos.Api/
├── Controllers/JogosController.cs      → HTTP (rotas, status codes)
├── Services/IJogoService + JogoService → regras de negócio
├── Repositories/IJogoRepository + JogoRepository → acesso ao MongoDB (Builders, Aggregate)
├── Models/Jogo.cs                      → documento com atributos BSON
├── Models/RelatorioEstoquePlataforma.cs→ resultado da agregação
├── DTOs/JogoRequest.cs                 → corpo do POST/PUT com validações (400)
├── Settings/MongoDbSettings.cs         → Options Pattern
└── Program.cs                          → injeção de dependência
```

Fluxo: **Controller → JogoService → IJogoRepository → MongoDB**.

- `MongoDbSettings` é ligado à seção `MongoDbSettings` do `appsettings.json` via `builder.Services.Configure<MongoDbSettings>(...)`.
- `IMongoClient` e `IMongoDatabase` são registrados como **Singleton** a partir de `IOptions<MongoDbSettings>`.
- Repository e Service são registrados como **Scoped**.

## Como executar

Pré-requisitos: .NET 8 SDK e Docker.

```bash
# 1. Subir o MongoDB
docker run -d -p 27017:27017 --name mongo-local mongo
#    (ou: docker compose up -d)

# 2. (Opcional) Popular com dados de exemplo
docker exec -i mongo-local mongosh < scripts/seed.js

# 3. Rodar a API
cd CatalogoJogos.Api
dotnet run
```

Swagger: http://localhost:5162/swagger

## Endpoints

| Método | Rota | Retornos |
|---|---|---|
| POST | `/api/jogos` | 201 Created, 400 |
| GET | `/api/jogos` | 200 |
| GET | `/api/jogos/{id}` | 200, 400 (ID inválido), 404 |
| PUT | `/api/jogos/{id}` | 200, 400, 404 |
| DELETE | `/api/jogos/{id}` | 204, 400, 404 |
| GET | `/api/jogos/busca?plataforma=PlayStation 3&precoMaximo=100` | 200, 400 |
| GET | `/api/jogos/relatorio-estoque` | 200 |

Exemplo de corpo (POST/PUT):

```json
{
  "titulo": "Gran Turismo 5",
  "plataforma": "PlayStation 3",
  "genero": "Corrida",
  "preco": 79.90,
  "anoLancamento": 2010,
  "estoque": 12
}
```

O arquivo `CatalogoJogos.Api.http` tem todas as requisições prontas para testar no VS Code / Visual Studio / Rider.

## Desafio 1 – Filtro por plataforma e preço

Em `JogoRepository.BuscarPorPlataformaEPrecoAsync`:

```csharp
var builder = Builders<Jogo>.Filter;
var filtro = builder.And(
    builder.Regex(j => j.Plataforma, new BsonRegularExpression($"^{Regex.Escape(plataforma)}$", "i")),
    builder.Lte(j => j.Preco, precoMaximo));
```

A plataforma precisa ser igual à informada (sem diferenciar maiúsculas/minúsculas) e o preço deve ser `<=` ao máximo.

## Desafio 2 – Relatório de estoque (agregação)

Em `JogoRepository.ObterRelatorioEstoqueAsync`, usando `IAggregateFluent`:

1. `$group` por `plataforma`: `quantidadeTitulos = $sum: 1`, `totalUnidadesEstoque = $sum: "$estoque"`, `valorTotalInventario = $sum: { $multiply: ["$preco", "$estoque"] }`
2. `$project` para renomear `_id` → `plataforma`
3. `$sort` pelo valor total do inventário (decrescente)

Exemplo de resposta (com o seed):

```json
[
  { "plataforma": "Nintendo DS",   "quantidadeTitulos": 3, "totalUnidadesEstoque": 27, "valorTotalInventario": 2757.30 },
  { "plataforma": "PlayStation 3", "quantidadeTitulos": 3, "totalUnidadesEstoque": 25, "valorTotalInventario": 2057.50 },
  { "plataforma": "Xbox 360",      "quantidadeTitulos": 3, "totalUnidadesEstoque": 32, "valorTotalInventario": 1726.80 }
]
```

O preço é salvo como `Decimal128` (`[BsonRepresentation(BsonType.Decimal128)]`) para manter precisão monetária e permitir o `$multiply` no banco.
