using System.Collections.Generic;
using EFT;
using Newtonsoft.Json;

namespace KoTClient.Models;

public class StateData
{
    [JsonProperty("id")]
    public MongoID Id { get; set; }
    
    [JsonProperty("trial")]
    public TrialData trial { get; set; }
    
    [JsonProperty("color")]
    public string color { get; set; }
    
    [JsonProperty("location")]
    public Dictionary<MongoID, LocationData> location { get; set; }
}

public class TrialData
{
    [JsonProperty("trialNum")]
    public int trialNum { get; set; }
    
    [JsonProperty("trialType")]
    public MongoID trialType { get; set; }
    
    [JsonProperty("mods")]
    public List<MongoID> mods { get; set; }
}

public class LocationData
{
    [JsonProperty("completed")]
    public bool completed;
    
    [JsonProperty("exfilRequirements")]
    public List<MongoID> exfilRequirements { get; set; }
    
    [JsonProperty("mods")]
    public List<MongoID> mods { get; set; }
}

public class IdData
{
    [JsonProperty("id")]
    public MongoID Id { get; set; }
}