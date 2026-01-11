using System.Text.Json.Serialization;

namespace KingOfTarkov.Models.Config;

public class KingConfig
{
    [JsonPropertyName("disableStockProfiles")]
    public bool DisableStockProfiles { get; set; } = true;
    
    [JsonPropertyName("endlessMode")]
    public bool EndlessMode { get; set; } = true;
    
    [JsonPropertyName("levelPerRaid")]
    public int LevelPerRaid { get; set; } = 1;
    
    [JsonPropertyName("generators")]
    public GeneratorConfig Generators { get; set; } = new();
    
    [JsonPropertyName("configEdits")]
    public EditConfig ConfigEdits { get; set; } = new();
    
    [JsonPropertyName("developer")]
    public bool Developer { get; set; } = false;
}

