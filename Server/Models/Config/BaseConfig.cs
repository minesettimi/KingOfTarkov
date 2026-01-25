using System.Text.Json.Serialization;

namespace KingOfTarkov.Models.Config;

public class BaseConfig
{
    [JsonPropertyName("difficulty")]
    public string Difficulty { get; set; } = "normal";
    
    [JsonPropertyName("disableStockProfiles")]
    public bool DisableStockProfiles { get; set; } = true;
    
    [JsonPropertyName("endlessMode")]
    public bool EndlessMode { get; set; } = true;
    
    [JsonPropertyName("developer")]
    public bool Developer { get; set; } = false;
}

