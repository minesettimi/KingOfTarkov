using System.Text.Json.Serialization;

namespace KingOfTarkov.Models.Difficulty;

public class TrialDifficulty
{
   [JsonPropertyName("levelPerRaid")]
   public int LevelPerRaid { get; set; } = 1;
   
   [JsonPropertyName("levelPerTrial")]
   public int LevelPerTrial { get; set; } = 3;
}