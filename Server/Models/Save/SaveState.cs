using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;

namespace KingOfTarkov.Models.Save;

public class SaveState
{
    [JsonPropertyName("id")]
    public MongoId Id { get; set; } = new();
    
    [JsonPropertyName("Trial")]
    public TrialState Trial { get; set; } = new();
    
    [JsonPropertyName("Location")]
    public LocationState Location { get; set; } = new();
    
    [JsonPropertyName("profile")]
    public ProfileState Profile { get; set; } = new();
    
    [JsonPropertyName("quests")]
    public QuestState Quests { get; set; } = new();
}
