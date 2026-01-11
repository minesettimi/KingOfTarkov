using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;

namespace KingOfTarkov.Models.Save;

public class LocationState
{
    [JsonPropertyName("completed")] 
    public bool Completed { get; set; } = false;

    [JsonPropertyName("mods")] 
    public List<MongoId> Mods { get; set; } = [];
}