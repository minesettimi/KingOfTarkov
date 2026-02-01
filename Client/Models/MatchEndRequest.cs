using EFT;
using Newtonsoft.Json;

namespace KoTClient.Models;

public class MatchEndRequest
{
    [JsonProperty("survived")]
    public bool Survived { get; set; }
    
    [JsonProperty("trialId")]
    public MongoID TrialId { get; set; }
    
    [JsonProperty("locationName")]
    public string LocationName { get; set; }
}