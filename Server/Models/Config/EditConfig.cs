using System.Text.Json.Serialization;

namespace KingOfTarkov.Models.Config;

public class EditConfig
{
    [JsonPropertyName("staticLootAdd")]
    public double StaticLootAdd { get; set; } = 2.0;
    
    [JsonPropertyName("looseLootAdd")]
    public double LooseLootAdd { get; set; } = 2.0;
    
    [JsonPropertyName("disableRepeatable")]
    public bool DisableRepeatable { get; set; } = true;
}