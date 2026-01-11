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
    
    [JsonPropertyName("staticLootAdd")]
    public double StaticLootAdd { get; set; } = 2.0;
    
    [JsonPropertyName("looseLootAdd")]
    public double LooseLootAdd { get; set; } = 2.0;
    
    [JsonPropertyName("disableRepeatable")]
    public bool DisableRepeatable { get; set; } = true;
    
    [JsonPropertyName("rewardChoices")]
    public int RewardChoices { get; set; } = 3;
    
    [JsonPropertyName("developer")]
    public bool Developer { get; set; } = false;
}

