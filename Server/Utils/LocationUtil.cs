using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;

namespace KingOfTarkov.Utils;

[Injectable(InjectionType.Singleton)]
public class LocationUtil
{
    public readonly Dictionary<MongoId, string> idToKey = new()
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

    public string GetMapKey(MongoId id)
    {
        idToKey.TryGetValue(id, out string? key);
        return key ?? "Any";
    }
    
    //gives other version of map
    public MongoId GetMapOther(MongoId id)
    {
        if (id == "59fc81d786f774390775787e")
        {
            return new MongoId("55f2d3fd4bdc2d5f408b4567");
        }

        if (id == "65b8d6f5cdde2479cb2a3125")
        {
            return new MongoId("653e6760052c01c1c805532f");
        }

        return id;
    }
}