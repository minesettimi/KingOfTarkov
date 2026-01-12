using KingOfTarkov.Models.Save;
using KingOfTarkov.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Utils;

namespace KingOfTarkov.Helpers;

[Injectable(InjectionType.Singleton)]
public class LocationHelper(SaveService save, ISptLogger<LocationHelper> logger)
{
    //maps with multiple versions are given the same id to match the system
    private Dictionary<string, MongoId> _mapNameToId = new()
    {
        {"bigmap", "56f40101d2720b2a4d8b45d6"},
        {"factory4_day", "55f2d3fd4bdc2d5f408b4567"},
        {"factory4_night", "55f2d3fd4bdc2d5f408b4567"},
        {"sandbox", "653e6760052c01c1c805532f"},
        {"sandbox_high", "653e6760052c01c1c805532f"},
        {"interchange", "5714dbc024597771384a510d"},
        {"lighthouse", "5704e4dad2720bb55b8b4567"},
        {"reservbase", "5704e5fad2720bc05b8b4567"},
        {"shoreline", "5704e554d2720bac5b8b456e"},
        {"tarkovstreets", "5714dc692459777137212e12"},
        {"laboratory", "5b0fc42d86f7744a585f9105"},
        {"labyrinth", "6733700029c367a3d40b02af"},
        {"woods","5704e3c2d2720bac5b8b4567"}
    };
    
    //LINQ is messy with dictionaries, do this the classic way
    public List<MongoId> GetActiveMaps()
    {
        List<MongoId> result = [];

        foreach ((MongoId id, LocationDataState data) in save.CurrentSave.Location.Active)
        {
            if (data.Completed) continue;
            
            result.Add(id);
        }

        return result;
    }

    public MongoId MapNameToId(string name)
    {
        MongoId? result = _mapNameToId.GetValueOrDefault(name);

        return result ?? throw new Exception("[KoT] Tried to retrieve id for map with name {name} which isn't in the lookup table.");
    }
}