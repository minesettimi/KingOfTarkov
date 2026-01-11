using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;

namespace KingOfTarkov.Models.Save;

public class ProfileState
{
    [JsonPropertyName("locked")]
    public bool Locked { get; set; } = true;
    
}

public class ProfileData
{
    [JsonPropertyName("died")]
    public bool Died { get; set; } = false;
    
    [JsonPropertyName("quests")]
    public List<MongoId> Quests { get; set; } = [];
}