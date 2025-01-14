namespace MongoRepoNet.Settings;


public class MongoDbSettingsOptions
{
    public string? ConnectionString { get; set; } = default!;
    public string? DatabaseName { get; set; } = default!;

    /// <summary>
    /// Default section name
    /// </summary>
    public const string Section = "MongoDbSettings";
}