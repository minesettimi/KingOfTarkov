using System.Text.Json.Serialization;

namespace KingOfTarkov.Models.Config;

public class GeneratorConfig
{
    [JsonPropertyName("rewardChoices")]
    public int RewardChoices { get; set; } = 3;
    
    [JsonPropertyName("maxAttempts")]
    public int MaxAttempts { get; set; } = 3;
}