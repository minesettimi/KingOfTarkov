using System.Text.Json.Serialization;

namespace KingOfTarkov.Models.Database;

public class ModifierData
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "ERROR";
    
    [JsonPropertyName("nonGlobal")]
    public bool NonGlobal { get; set; } = false;
    
    [JsonPropertyName("free")]
    public bool Free { get; set; } = false;
    
    [JsonPropertyName("image")]
    public string Image { get; set; } = "/files/modifiers/icon/error.png";
}