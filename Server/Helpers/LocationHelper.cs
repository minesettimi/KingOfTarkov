using KingOfTarkov.Models.Save;
using KingOfTarkov.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Utils;

namespace KingOfTarkov.Helpers;

[Injectable(InjectionType.Singleton)]
public class LocationHelper(SaveService saveService,
    LocationService locationService,
    ISptLogger<LocationHelper> logger)
{
    public KeyValuePair<MongoId, LocationDataState>? GetLastLocation()
    {
        if (saveService.RemainingRaids != 1)
            return null;
        
        return saveService.CurrentSave.Location.Active.First(l => !l.Value.Completed);
    }

    public void SetupBossLocation(MongoId key, LocationBase locationBase)
    {
        List<string> bosses = locationService.BossCache[key];

        foreach (BossLocationSpawn bossConfig in locationBase.BossLocationSpawn)
        {
            if (!bosses.Contains(bossConfig.BossName))
                continue;
            
            logger.Debug($"[KoT] Setting boss {bossConfig.BossName} to 100%");
            bossConfig.BossChance = 100;
            bossConfig.ShowOnTarkovMap = true;
            bossConfig.ShowOnTarkovMapPvE = true;

            if (GetLastLocation()?.Value.Boss == bossConfig.BossName)
            {
                logger.Debug($"[KoT] Setting boss {bossConfig.BossName} to force spawn.");
                bossConfig.ForceSpawn = true;
            }
        }
    }
}