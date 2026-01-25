using KingOfTarkov.Models.Save;
using KingOfTarkov.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils.Cloners;

namespace KingOfTarkov.Helpers;

[Injectable(InjectionType.Singleton)]
public class LocationHelper(SaveService saveService,
    ISptLogger<LocationHelper> logger)
{
    
    
    public KeyValuePair<MongoId, LocationDataState>? GetLastLocation()
    {
        if (saveService.RemainingRaids != 1)
            return null;
        
        return saveService.CurrentSave.Location.Active.First(l => !l.Value.Completed);
    }

    public void SetupBossLocation(MongoId key, LocationBase locationBase, Dictionary<MongoId, List<string>> bossCache)
    {
        List<string> bosses = bossCache[key];

        foreach (BossLocationSpawn bossConfig in locationBase.BossLocationSpawn)
        {
            if (!bosses.Contains(bossConfig.BossName))
                continue;
            
            logger.Debug($"[KoT] Setting boss {bossConfig.BossName} to 100%");
            bossConfig.BossChance = 100;
            bossConfig.ShowOnTarkovMap = true;
            bossConfig.ShowOnTarkovMapPvE = true;
            bossConfig.DependKarma = false;

            //make *sure* the boss that is the target is killed
            if (GetLastLocation()?.Value.Boss == bossConfig.BossName)
            {
                logger.Debug($"[KoT] Setting boss {bossConfig.BossName} to force spawn.");
                bossConfig.Delay = 0; //don't make the player hunt just to find out they had to wait
                bossConfig.IsRandomTimeSpawn = false;
                bossConfig.ForceSpawn = true;
            }
        }
    }
    
    public List<MongoId> GetActiveMaps()
    {
        return saveService.CurrentSave.Location.Active
            .Where(l => !l.Value.Completed)
            .Select(l => l.Key).ToList();
    }
}