namespace CatalogoJogos.Api.Settings;

/// <summary>
/// Configurações do MongoDB lidas da seção "MongoDbSettings" do appsettings.json
/// e injetadas via Options Pattern (IOptions&lt;MongoDbSettings&gt;).
/// </summary>
public class MongoDbSettings
{
    public const string SectionName = "MongoDbSettings";

    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string JogosCollectionName { get; set; } = "Jogos";
}
