using KingOfTarkov.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;

namespace KingOfTarkov.Services;

[Injectable(InjectionType.Singleton)]
public class LocationService(LocationUtil locationUtil,
    DataService dataService,
    DatabaseService databaseService,
    ISptLogger<LocationService> logger)
{
    public Dictionary<MongoId, List<string>> BossCache = new();

    public Task Load()
    {
        AddCustomBossSpawns();
        CacheBosses();
        return Task.CompletedTask;
    }

    private void AddCustomBossSpawns()
    {
        Dictionary<string, Location> locationDb = databaseService.GetLocations().GetDictionary();
        foreach ((string mapName, List<BossLocationSpawn> bossSpawn) in dataService.CustomBossSpawns)
        {
            if (!locationDb.ContainsKey(mapName))
            {
                logger.Error($"[KoT] Custom boss spawn location invalid: {mapName}.");
                continue;
            }
            
            locationDb[mapName].Base.BossLocationSpawn.AddRange(bossSpawn);
        }
        
        //universal bosses
        foreach ((string key, Location location) in locationDb)
        {
            //vengeful event
            location.Base.BossLocationSpawn.Add(new BossLocationSpawn
            {
                BossChance = 0,
                BossDifficulty = "normal",
                BossEscortAmount = "0",
                BossEscortDifficulty = "normal",
                BossEscortType = "bossZryachiy",
                BossName = "ravangeZryachiyEvent",
                IsBossPlayer = false,
                BossZone = "BotZone",
                Delay = 0,
                ForceSpawn = true,
                IgnoreMaxBots = true,
                IsRandomTimeSpawn = false,
                ShowOnTarkovMap = false,
                ShowOnTarkovMapPvE = false,
                SpawnMode = ["pve", "regular"],
                Time = -1,
                TriggerId = "",
                TriggerName = ""
            });
        }
    }
    
    private void CacheBosses()
    {
        foreach (MongoId id in dataService.TrialConfig.Locations.Keys)
        {
            List<string> bosses = GetBossesForLocation(id);
            BossCache.Add(id, bosses);
        }

        logger.Info("[KoT] Finished caching bosses.");
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

        //blacklist Zryachiy due to generally requiring way more to kill him as well as the unaccessible lighthouse item
        return spawns.Select(l => l.BossName)
            .Where(s => s != "bossZryachiy" && s.StartsWith("boss"))
            .ToList();
    }
}