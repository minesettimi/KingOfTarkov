using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;

namespace KingOfTarkov.Models.Database;

public class TrialTypeData
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("min")]
    public int Min { get; set; }
    
    [JsonPropertyName("modPool")]
    public List<MongoId> ModPool { get; set; } = [];
}