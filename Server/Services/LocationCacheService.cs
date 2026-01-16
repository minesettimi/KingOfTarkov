using KingOfTarkov.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;

namespace KingOfTarkov.Services;

[Injectable(InjectionType.Singleton)]
public class LocationCacheService(LocationUtil locationUtil,
    DataService dataService,
    DatabaseService databaseService,
    ISptLogger<LocationCacheService> logger)
{
    public Dictionary<MongoId, List<string>> BossCache = new();

    public Task Load()
    {
        CacheBosses();
        return Task.CompletedTask;
    }
    
    public void CacheBosses()
    {
        foreach (MongoId id in dataService.TrialConfig.Locations.Keys)
        {
            List<string> bosses = GetBossesForLocation(id);
            BossCache.Add(id, bosses);
        }
    }

    public List<string> GetBossesForLocation(MongoId locationId)
    {
        string locationKey = locationUtil.GetMapKey(locationId);

        List<BossLocationSpawn>? spawns = databaseService.GetLocation(locationKey)?.Base.BossLocationSpawn;

        if (spawns == null)
        {
            logger.Error($"[KoT] No boss spawn data found on location: {locationKey}!");
            return [];
        }

        return spawns.FindAll(s => s.BossName.StartsWith("boss") || s.BossName.StartsWith("sectant"))
            .Select(l => l.BossName)
            .ToList();
    }
}