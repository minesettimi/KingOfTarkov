using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;

namespace KingOfTarkov.Models.Database;

public class TrialData
{
    [JsonPropertyName("trials")]
    public Dictionary<int, TrialNumData> Trials { get; set; } = new();
    
    [JsonPropertyName("locations")]
    public Dictionary<MongoId, LocationData> Locations { get; set; } = new();
    
    [JsonPropertyName("types")]
    public Dictionary<MongoId, TrialTypeData> Types { get; set; } = new();
}

public class TrialNumData
{
    [JsonPropertyName("locationCount")]
    public int LocationCount { get; set; } = 2;
    
    [JsonPropertyName("globalModCount")]
    public int GlobalModCount { get; set; } = 0;
    
    [JsonPropertyName("locationModCount")]
    public int LocationModCount { get; set; } = 0;
    
    [JsonPropertyName("typeWhitelist")]
    public List<MongoId>? TypeWhitelist { get; set; }
}