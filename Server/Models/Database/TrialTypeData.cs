using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;

namespace KingOfTarkov.Models.Database;

public class TrialTypeData
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("color")]
    public string Color { get; set; }
    
    [JsonPropertyName("min")]
    public int Min { get; set; }
    
    [JsonPropertyName("max")]
    public int Max { get; set; }
    
    [JsonPropertyName("exclusive")]
    public bool Exclusive { get; set; } = false;
    
    [JsonPropertyName("endless")]
    public bool AllowEndless { get; set; } = true;
    
    [JsonPropertyName("modPool")]
    public List<MongoId> ModPool { get; set; } = [];
}