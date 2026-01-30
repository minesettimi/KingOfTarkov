using System.Text.Json.Serialization;

namespace KingOfTarkov.Models.Difficulty;

public class EndlessDifficulty
{
    [JsonPropertyName("globalIncrement")]
    public int GlobalIncrement { get; set; } = 3;
    
    [JsonPropertyName("locationCount")]
    public int LocationCount { get; set; } = 5;
}