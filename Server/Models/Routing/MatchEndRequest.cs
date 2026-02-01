using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Utils;

namespace KingOfTarkov.Models.Response;

public class MatchEndRequest : IRequestData
{
    [JsonPropertyName("survived")]
    public bool Survived { get; set; }
    
    [JsonPropertyName("trialId")]
    public MongoId TrialId { get; set; }
    
    [JsonPropertyName("locationName")]
    public string LocationName { get; set; }
}