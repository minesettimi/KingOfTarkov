using Newtonsoft.Json;

namespace KoTClient.Models;

public class PlayerData
{
    [JsonProperty("valid")]
    public bool Valid { get; set; }
    
    [JsonProperty("lives")]
    public int Lives { get; set; }
}

public class ProfileLockedResponse
{
    [JsonProperty("locked")]
    public bool Locked { get; set; }
}