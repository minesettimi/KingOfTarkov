using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;

namespace KingOfTarkov.Models.Save;

public class TrialState
{
    [JsonPropertyName("trialId")]
    public MongoId trialId { get; set; }
    
    [JsonPropertyName("trialNum")] 
    public int TrialNum { get; set; } = 0;
    
    [JsonPropertyName("trialType")]
    public MongoId TrialType { get; set; } = new MongoId("69634713f3ed3f71243d25ce");
    
    [JsonPropertyName("mods")]
    public List<MongoId> mods { get; set; }  = [];
}