using System.Threading.Tasks;
using EFT;
using Newtonsoft.Json;
using UnityEngine;

namespace KoTClient.Models;

public class ModifierData
{
    [JsonIgnore]
    public MongoID _Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("image")]
    public string Image { get; set; }
    
    [JsonIgnore]
    public Sprite? Sprite { get; set; }

    public async Task LoadIconSprite(GInterface221 session)
    {
        Sprite newSprite = await GClass855.LoadIconSprite(session, Image);
        Sprite = newSprite;
    }
}