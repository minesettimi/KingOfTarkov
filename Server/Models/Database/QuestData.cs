using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;

namespace KingOfTarkov.Models.Database;

public class QuestData
{
    [JsonPropertyName("exfil")]
    public ExfilData Exfil { get; set; }
}

public class ExfilData
{
    [JsonPropertyName("elimination")]
    public MongoId Elimination { get; set; }
    
    [JsonPropertyName("boss")]
    public MongoId Boss { get; set; }
}