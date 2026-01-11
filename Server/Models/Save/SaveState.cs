using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;

namespace KingOfTarkov.Models.Save;

public class SaveState
{
    [JsonPropertyName("id")]
    public MongoId Id { get; set; } = new();
    
    [JsonPropertyName("trial")]
    public TrialState Trial { get; set; } = new();
    
    [JsonPropertyName("location")]
    public LocationState Location { get; set; } = new();
    
    [JsonPropertyName("profile")]
    public ProfileState Profile { get; set; } = new();
}
