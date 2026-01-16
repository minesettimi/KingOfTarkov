using KingOfTarkov.Models.Save;
using KingOfTarkov.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;

namespace KingOfTarkov.Helpers;

[Injectable(InjectionType.Singleton)]
public class LocationHelper(SaveService saveService,
    LocationService locationService)
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

            bossConfig.BossChance = 100;
            bossConfig.ShowOnTarkovMap = true;
            bossConfig.ShowOnTarkovMapPvE = true;
        }
    }
}