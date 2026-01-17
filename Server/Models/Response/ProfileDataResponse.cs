using System.Text.Json.Serialization;

namespace KingOfTarkov.Models.Response;

public class ProfileDataResponse
{
    [JsonPropertyName("valid")]
    public bool Valid { get; set; }
    
    [JsonPropertyName("lives")]
    public int Lives { get; set; }
}