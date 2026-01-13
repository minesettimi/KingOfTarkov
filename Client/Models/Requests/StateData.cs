using System.Collections.Generic;
using EFT;
using KoTClient.Models.Data;

namespace KoTClient.Models;

public class StateData
{
    public TrialData trial { get; set; }
    public string color { get; set; }
    public Dictionary<MongoID, List<MongoID>> location { get; set; }
}

public class TrialData
{
    public MongoID id { get; set; }
    public int trialNum { get; set; }
    public MongoID trialType { get; set; }
    public List<MongoID> modPool { get; set; }
}