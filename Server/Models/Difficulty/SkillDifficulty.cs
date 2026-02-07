using System.Text.Json.Serialization;

namespace KingOfTarkov.Models.Difficulty;

public class SkillDifficulty
{
    [JsonPropertyName("generalSkill")]
    public double GeneralSkill { get; set; } = 1.0;
    
    [JsonPropertyName("weaponSkill")]
    public double WeaponSkill { get; set; } = 1.5;
    
    [JsonPropertyName("minimumFatigue")]
    public double MinimumFatigue { get; set; } = 0.01;
    
    [JsonPropertyName("fatiguePerPoint")]
    public double FatiguePerPoint { get; set; } = 0.6;
    
    [JsonPropertyName("freshSkills")]
    public int FreshSkills { get; set; } = 15;
    
    [JsonPropertyName("preSkillsFatigue")]
    public int PreSkillsFatigue { get; set; } = 25;
}