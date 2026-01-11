using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;

namespace KingOfTarkov.Models.Save;

public class TrialState
{
    [JsonPropertyName("trialNum")] 
    public int TrialNum { get; set; } = 1;
    
    //TODO: Trial types
    //[JsonConverter(JsonStringEnumConverter)]
    [JsonPropertyName("trialType")]
    public int TrialType { get; set; } = 1;
    
    [JsonPropertyName("mods")]
    public List<MongoId> mods = [];
}