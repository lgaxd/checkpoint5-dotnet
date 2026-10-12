// Popula o banco com jogos de exemplo.
// Uso: docker exec -i mongo-local mongosh < scripts/seed.js
db = db.getSiblingDB("CatalogoJogosDb");
db.Jogos.deleteMany({});
db.Jogos.insertMany([
  { titulo: "Gran Turismo 5",         plataforma: "PlayStation 3", genero: "Corrida", preco: NumberDecimal("79.90"),  anoLancamento: 2010, estoque: 12 },
  { titulo: "The Last of Us",         plataforma: "PlayStation 3", genero: "Ação",    preco: NumberDecimal("99.90"),  anoLancamento: 2013, estoque: 8 },
  { titulo: "Uncharted 2",            plataforma: "PlayStation 3", genero: "Ação",    preco: NumberDecimal("59.90"),  anoLancamento: 2009, estoque: 5 },
  { titulo: "Forza Motorsport 4",     plataforma: "Xbox 360",      genero: "Corrida", preco: NumberDecimal("69.90"),  anoLancamento: 2011, estoque: 10 },
  { titulo: "Halo 3",                 plataforma: "Xbox 360",      genero: "Ação",    preco: NumberDecimal("49.90"),  anoLancamento: 2007, estoque: 15 },
  { titulo: "Mass Effect 2",          plataforma: "Xbox 360",      genero: "RPG",     preco: NumberDecimal("39.90"),  anoLancamento: 2010, estoque: 7 },
  { titulo: "Mario Kart DS",          plataforma: "Nintendo DS",   genero: "Corrida", preco: NumberDecimal("89.90"),  anoLancamento: 2005, estoque: 20 },
  { titulo: "Pokémon HeartGold",      plataforma: "Nintendo DS",   genero: "RPG",     preco: NumberDecimal("149.90"), anoLancamento: 2009, estoque: 4 },
  { titulo: "The World Ends with You",plataforma: "Nintendo DS",   genero: "RPG",     preco: NumberDecimal("119.90"), anoLancamento: 2007, estoque: 3 }
]);
print("Jogos inseridos: " + db.Jogos.countDocuments());
