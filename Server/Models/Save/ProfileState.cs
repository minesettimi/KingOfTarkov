using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;

namespace KingOfTarkov.Models.Save;

public class ProfileState
{
    [JsonPropertyName("locked")]
    public bool Locked { get; set; } = false;

    [JsonPropertyName("profiles")] 
    public Dictionary<MongoId, ProfileInfoState> Profiles { get; set; } = new();
}

public class ProfileInfoState
{
    [JsonPropertyName("lives")]
    public int Lives { get; set; }
    
    [JsonPropertyName("revives")]
    public int Revives { get; set; }
    
    [JsonPropertyName("quests")]
    public List<MongoId> Quests { get; set; } = [];
}