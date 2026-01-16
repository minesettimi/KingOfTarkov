using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;

namespace KingOfTarkov.Utils;

[Injectable(InjectionType.Singleton)]
public class LocationUtil
{
    private readonly Dictionary<MongoId, string> _idToKey = new()
    {
        { "56f40101d2720b2a4d8b45d6", "bigmap" },
        {"55f2d3fd4bdc2d5f408b4567", "factory4_day"},
        {"5714dbc024597771384a510d", "Interchange"},
        {"5b0fc42d86f7744a585f9105", "Laboratory"},
        {"5704e5fad2720bc05b8b4567", "RezervBase"},
        {"5704e554d2720bac5b8b456e", "Shoreline"},
        {"5704e3c2d2720bac5b8b4567", "Woods"},
        {"5704e4dad2720bb55b8b4567", "Lighthouse"},
        {"5714dc692459777137212e12", "TarkovStreets"},
        {"653e6760052c01c1c805532f", "Sandbox"},
        {"6733700029c367a3d40b02af", "Labyrinth"}
    };

    //post raid outputs lower case location ids, SPT's method is case sensitive and requires caps,
    //due to maps like reserve I can't just programmatically do it
    private readonly Dictionary<string, MongoId> _keyToId = new()
    {
        {"bigmap", "56f40101d2720b2a4d8b45d6"  },
        {"factory4_day", "55f2d3fd4bdc2d5f408b4567"},
        {"interchange", "5714dbc024597771384a510d"},
        {"laboratory", "5b0fc42d86f7744a585f9105"},
        {"rezervbase","5704e5fad2720bc05b8b4567"},
        {"shoreline", "5704e554d2720bac5b8b456e"},
        {"woods","5704e3c2d2720bac5b8b4567"},
        {"lighthouse", "5704e4dad2720bb55b8b4567"},
        {"tarkovstreets","5714dc692459777137212e12"},
        {"sandbox", "653e6760052c01c1c805532f"},
        {"labyrinth", "6733700029c367a3d40b02af"}
    };

    public string GetMapKey(MongoId id)
    {
        _idToKey.TryGetValue(id, out string? key);
        return key ?? "Any";
    }

    public MongoId GetMapId(string locationName)
    {
        _keyToId.TryGetValue(locationName, out MongoId id);
        return id;
    }
    
    //gives other version of map
    public MongoId GetMapOther(MongoId id)
    {
        if (id == "59fc81d786f774390775787e")
        {
            return "55f2d3fd4bdc2d5f408b4567";
        }

        if (id == "65b8d6f5cdde2479cb2a3125")
        {
            return "653e6760052c01c1c805532f";
        }

        return id;
    }
}