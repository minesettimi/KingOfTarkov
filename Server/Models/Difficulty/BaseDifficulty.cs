using System.Text.Json.Serialization;

namespace KingOfTarkov.Models.Difficulty;

public class BaseDifficulty
{
    [JsonPropertyName("core")]
    public CoreDifficulty Core { get; set; } = new();
    
    [JsonPropertyName("trial")]
    public TrialDifficulty Trial { get; set; } = new();
    
    [JsonPropertyName("endless")]
    public EndlessDifficulty Endless { get; set; } = new();
    
    [JsonPropertyName("location")]
    public LocationDifficulty Location { get; set; } = new();
    
    [JsonPropertyName("skills")]
    public SkillDifficulty Skills { get; set; } = new();
}