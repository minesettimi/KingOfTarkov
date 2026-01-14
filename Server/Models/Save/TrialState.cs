using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;

namespace KingOfTarkov.Models.Save;

public class TrialState
{
    [JsonPropertyName("trialId")]
    public MongoId TrialId { get; set; }
    
    [JsonPropertyName("trialNum")] 
    public int TrialNum { get; set; }
    
    [JsonPropertyName("trialType")]
    public MongoId TrialType { get; set; }
    
    [JsonPropertyName("mods")]
    public List<MongoId> Mods { get; set; }  = [];
}