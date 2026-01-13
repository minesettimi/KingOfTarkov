using System.Collections.Generic;
using EFT;

namespace KoTClient.Models.Data;

public class TrialType
{
    public MongoID id { get; set; }
    public string color {get; set;}
    public List<MongoID> modPool { get; set; }
}