using KingOfTarkov.Helpers;
using KingOfTarkov.Models;
using KingOfTarkov.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils.Cloners;
using Locations = SPTarkov.Server.Core.Models.Spt.Server.Locations;

namespace KingOfTarkov.Services;

[Injectable(InjectionType.Singleton)]
public class LocationService(LocationUtil locationUtil,
    DataService dataService,
    DatabaseService dbService,
    ModifierService modService,
    LocationHelper locationHelper,
    ICloner cloner,
    ISptLogger<LocationService> logger)
{
    public readonly Dictionary<MongoId, List<string>> BossCache = new();
    private Dictionary<string, LocationBase> _locationCache = new();

    public Task Load()
    {
        Dictionary<string, Location> locations = dbService.GetLocations().GetDictionary();
        AddCustomBossSpawns(locations);
        CacheBosses();
        
        CacheLocations(locations);
        SetupTrialLocations(locations);
        
        return Task.CompletedTask;
    }

    private void AddCustomBossSpawns(Dictionary<string, Location> locationDb)
    {
        foreach ((string mapName, List<BossLocationSpawn> bossSpawn) in dataService.CustomBossSpawns)
        {
            if (mapName == "global")
                continue;
            
            if (!locationDb.ContainsKey(mapName))
            {
                logger.Error($"[KoT] Custom boss spawn location invalid: {mapName}.");
                continue;
            }
            
            locationDb[mapName].Base.BossLocationSpawn.AddRange(cloner.Clone(bossSpawn)!);
            locationDb[mapName].Base.BossLocationSpawn.AddRange(cloner.Clone(dataService.CustomBossSpawns["global"])!);
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

        List<BossLocationSpawn>? spawns = dbService.GetLocation(locationKey)?.Base.BossLocationSpawn;

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
    
    private void CacheLocations(Dictionary<string, Location> locationDb)
    {
        foreach ((string key, Location oldLoc) in locationDb)
        {
            if (key == "Laboratory" || key == "Labyrinth")
            {
                oldLoc.Base.IsSecret = false;
                oldLoc.Base.AccessKeys = [];
                oldLoc.Base.AccessKeysPvE = [];
                oldLoc.Base.Enabled = true;
            }
            else if (key == "Terminal")
            {
                oldLoc.Base.Enabled = false;
            }
            
            LocationBase locCopy = cloner.Clone(oldLoc.Base)!;
            
            _locationCache.Add(key, locCopy);
        }
        
        logger.Info("[KoT] Finished caching locations.");
    }

    public void SetupNewLocations()
    {
        Dictionary<string, Location> locationDb = dbService.GetLocations().GetDictionary();
        
        foreach ((string key, LocationBase oldLoc) in _locationCache)
        {
            locationDb[key].Base = cloner.Clone(oldLoc)!;
        }
        
        SetupTrialLocations(locationDb);
    }
    
    private void SetupTrialLocations(Dictionary<string, Location> locationDb)
    {
        List<MongoId> activeMaps = locationHelper.GetActiveMaps();
        foreach ((string key, Location oldLoc) in locationDb)
        {
            LocationBase location = oldLoc.Base;
            
            MongoId checkId = locationUtil.GetMapOther(location.IdField); 
            if (activeMaps.Contains(checkId))
            {
                location.IsSecret = true;
                location.Locked = false;
                
                HandleLocationModifiers(location, checkId);
                
                //last map
                if (activeMaps.Count == 1)
                {
                    locationHelper.SetupBossLocation(checkId, location, BossCache);
                }
            }
            else
            {
                location.IsSecret = false;
                location.Locked = true;
            }
        }
    }

    private readonly Dictionary<string, MongoId> _modBossSpawns = new()
    {
        {"sectantPriest", ModIds.NOBODY_EXPECTS_CULT},
        {"sectantPredvestnik", ModIds.FORGOTTEN_SOLDIERS},
        {"followerBigPipe", ModIds.BIG_PIPE},
        {"ravangeZryachiyEvent", ModIds.VENGEFUL},
        {"blackDivAssault", ModIds.CLEANING_HOUSE}
    };

    private void HandleLocationModifiers(LocationBase location, MongoId locationId)
    {
        
        if (modService.HasMod(ModIds.ANTI_AIRCRAFT, locationId))
            location.AirdropParameters?.Clear();
        
        //exfils
        foreach (Exit exfil in location.Exits)
        {
            if (exfil.Name.Contains("sniper_exit", StringComparison.CurrentCultureIgnoreCase) 
                && modService.HasMod(ModIds.BLOOD_SNIPERS, locationId))
            {
                exfil.Chance = 0;
                exfil.ChancePVE = 0;
            }

            if (exfil is { RequirementTip: "EXFIL_Item", ExfiltrationType: ExfiltrationType.SharedTimer })
            {
                if (modService.HasMod(ModIds.TAXI_TAX, locationId))
                {
                    exfil.Count *= 5;
                    exfil.CountPVE *= 5;
                }

                if (modService.HasMod(ModIds.SLOW_ENGINE, locationId))
                {
                    exfil.ExfiltrationTime *= 2.5;
                    exfil.ExfiltrationTimePVE *= 2.5;
                }
            }
        }

        //reduce calculation time in the loop
        List<string> bossesToApply = _modBossSpawns.Where(p => modService.HasMod(p.Value, locationId))
            .Select(p => p.Key)
            .ToList();

        bool bolsteredNumbers = modService.HasMod(ModIds.BOLSTERED_NUMBERS, locationId);
        
        //bot spawns
        foreach (BossLocationSpawn bossSetting in location.BossLocationSpawn)
        {
            if (bossesToApply.Contains(bossSetting.BossName))
            {
                bossSetting.ForceSpawn = true;
                bossSetting.BossChance = 100;
            }

            if (!bolsteredNumbers) continue;
            
            bossSetting.BossEscortAmount = IncreaseBotCount(bossSetting.BossEscortAmount!, 1.5);

            if (bossSetting.Supports == null) continue;
                
            foreach (BossSupport support in bossSetting.Supports)
            {
                support.BossEscortAmount = IncreaseBotCount(support.BossEscortAmount!, 1.25);
            }
        }
        
        if (modService.HasMod(ModIds.BETTER_THINGS_TO_DO, locationId))
        {
            int newTimeLimit = (int)Math.Round(location.EscapeTimeLimit!.Value * 0.75);
            location.EscapeTimeLimit = newTimeLimit;
            location.EscapeTimeLimitCoop = newTimeLimit;
            location.EscapeTimeLimitPVE = newTimeLimit;
        }
    }

    private string IncreaseBotCount(string numbers, double increase)
    {
        string[] followerStrings = numbers.Split(",");
        int[] followerNumbers = Array.ConvertAll(followerStrings, int.Parse);

        for (int i = 0; i < followerNumbers.Length; i++)
        {
            double count = followerNumbers[i];
                    
            //surprise add followers
            if (count == 0)
                count = 1;
                    
            followerNumbers[i] = (int)Math.Ceiling(count * increase);
        }

        return string.Join(",", followerNumbers);
    }
}