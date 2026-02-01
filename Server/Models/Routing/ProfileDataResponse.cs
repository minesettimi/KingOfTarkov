using System.Text.Json.Serialization;

namespace KingOfTarkov.Models.Response;

public class ProfileDataResponse
{
    [JsonPropertyName("valid")]
    public bool Valid { get; set; }
    
    [JsonPropertyName("lives")]
    public int Lives { get; set; }
    
    [JsonPropertyName("revives")]
    public int Revives { get; set; }
}

public class ProfileLockResponse(bool locked)
{
    [JsonPropertyName("locked")]
    public bool Locked { get; set; } = locked;
}