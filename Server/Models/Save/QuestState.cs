using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace KingOfTarkov.Models.Save;

public class QuestState
{
    [JsonPropertyName("exfil")]
    public Dictionary<MongoId, Quest> Exfil { get; set; }
    
    [JsonPropertyName("personal")]
    public Dictionary<MongoId, Quest> Personal { get; set; } = new();
}