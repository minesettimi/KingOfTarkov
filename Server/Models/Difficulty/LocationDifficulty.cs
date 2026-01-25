using System.Text.Json.Serialization;

namespace KingOfTarkov.Models.Difficulty;

public class LocationDifficulty
{
    [JsonPropertyName("staticLootAdditive")]
    public double StaticLootAdd { get; set; } = 0.5;

    [JsonPropertyName("looseLootAdditive")]
    public double LooseLootAdd { get; set; } = 2;
}