using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;

namespace KingOfTarkov.Models.Save;

public class ProfileState
{
    [JsonPropertyName("locked")]
    public bool Locked { get; set; } = true;

    [JsonPropertyName("profiles")] public Dictionary<MongoId, ProfileInfoState> Profiles { get; set; } = new();
}

public class ProfileInfoState
{
    [JsonPropertyName("died")]
    public bool Died { get; set; } = false;
    
    [JsonPropertyName("quests")]
    public List<MongoId> Quests { get; set; } = [];
}