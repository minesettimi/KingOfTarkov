using System.Text.Json.Serialization;
using KingOfTarkov.Models.Save;
using SPTarkov.Server.Core.Models.Common;

namespace KingOfTarkov.Models.Response;

public record TrialDataResponse
{
    [JsonPropertyName("id")]
    public MongoId Id { get; set; }
    
    [JsonPropertyName("trial")]
    public TrialState Trial { get; set; }
    
    [JsonPropertyName("color")]
    public string Color { get; set; }
    
    [JsonPropertyName("Location")]
    public Dictionary<MongoId, LocationDataState> Location { get; set; }
}

public record TrialIdResponse(MongoId trialId)
{
    [JsonPropertyName("id")] public MongoId Id { get; set; } = trialId;
}