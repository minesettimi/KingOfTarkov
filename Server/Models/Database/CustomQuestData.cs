using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Spt.Config;

namespace KingOfTarkov.Models.Database;

public class CustomQuestData
{
    [JsonPropertyName("customRepeatable")]
    public RepeatableQuestConfig CustomRepeatable { get; set; }
}