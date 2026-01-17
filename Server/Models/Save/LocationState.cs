using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;

namespace KingOfTarkov.Models.Save;

public class LocationState
{
    [JsonPropertyName("active")]
    public Dictionary<MongoId, LocationDataState> Active { get; set; } = new();
    
    [JsonPropertyName("previous")]
    public List<MongoId> Previous { get; set; } = new();
}

public class LocationDataState
{
    [JsonPropertyName("completed")] 
    public bool Completed { get; set; } = false;

    [JsonPropertyName("boss")] 
    public string? Boss { get; set; }

    [JsonPropertyName("exfilRequirements")]
    public List<MongoId> ExfilRequirements { get; set; } = [];

    [JsonPropertyName("mods")] 
    public List<MongoId> Mods { get; set; } = [];
}