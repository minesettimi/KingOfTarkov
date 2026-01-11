using System.Text.Json.Serialization;

namespace KingOfTarkov.Models.Database;

public class LocationData
{
    [JsonPropertyName("min")]
    public int Min { get; set; } = 1;
    
    [JsonPropertyName("max")]
    public int Max { get; set; } = 10;
    
    [JsonPropertyName("minWeight")]
    public double MinWeight { get; set; } = 1.0;
    
    [JsonPropertyName("maxWeight")]
    public double MaxWeight { get; set; } = 1.0;
}