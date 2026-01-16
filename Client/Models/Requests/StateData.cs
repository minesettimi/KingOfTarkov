using System.Collections.Generic;
using EFT;

namespace KoTClient.Models;

public class StateData
{
    public MongoID id { get; set; }
    public TrialData trial { get; set; }
    public string color { get; set; }
    public Dictionary<MongoID, LocationData> location { get; set; }
}

public class TrialData
{
    public int trialNum { get; set; }
    public MongoID trialType { get; set; }
    public List<MongoID> mods { get; set; }
}

public class LocationData
{
    public bool completed;
    public List<MongoID> exfilRequirements { get; set; }
    public List<MongoID> mods { get; set; }
}