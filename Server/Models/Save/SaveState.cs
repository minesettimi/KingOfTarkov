using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;

namespace KingOfTarkov.Models.Save;

public class SaveState
{
    [JsonPropertyName("id")]
    public MongoId Id = new();
    
    [JsonPropertyName("trial")]
    public TrialState Trial = new();
    
    [JsonPropertyName("location")]
    public LocationState Location = new();
    
    [JsonPropertyName("profile")]
    public ProfileState Profile = new();
}
