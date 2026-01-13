using System.Text.Json.Serialization;
using KingOfTarkov.Models.Save;
using SPTarkov.Server.Core.Models.Common;

namespace KingOfTarkov.Models.Response;

public record TrialDataResponse
{
    [JsonPropertyName("trial")]
    public TrialState Trial { get; set; }
    
    [JsonPropertyName("color")]
    public string Color { get; set; }
    
    [JsonPropertyName("Location")]
    public Dictionary<MongoId, List<MongoId>> Location { get; set; }
}