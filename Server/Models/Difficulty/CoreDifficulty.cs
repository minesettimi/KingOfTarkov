using System.Text.Json.Serialization;

namespace KingOfTarkov.Models.Difficulty;

public class CoreDifficulty
{
    [JsonPropertyName("lives")]
    public int Lives { get; set; } = 3;
    
    [JsonPropertyName("revives")]
    public bool Revives { get; set; } = false;
    
    [JsonPropertyName("reviveCost")]
    public int ReviveCost { get; set; } = 1000000;
    
    [JsonPropertyName("reviveLimit")]
    public int ReviveLimit { get; set; } = 1;
}