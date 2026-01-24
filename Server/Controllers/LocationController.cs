using KingOfTarkov.Models;
using KingOfTarkov.Services;
using KingOfTarkov.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils.Cloners;

namespace KingOfTarkov.Helpers;

[Injectable(InjectionType.Singleton)]
public class LocationController(SaveService save,
    KingProfileHelper profileHelper,
    LocationUtil locationUtil,
    TrialService trialService,
    LocationHelper locationHelper,
    ICloner cloner,
    ModifierService modService,
    ISptLogger<LocationController> logger)
{

    public LocationsGenerateAllResponse HandleGetLocations(LocationsGenerateAllResponse initial)
    {
        List<MongoId> activeMaps = GetActiveMaps();
        
        foreach ((MongoId id, LocationBase oldLoc) in initial.Locations)
        {
            LocationBase location = cloner.Clone(oldLoc)!;
            
            MongoId checkId = locationUtil.GetMapOther(id); 
            if (activeMaps.Contains(checkId))
            {
                location.IsSecret = true;
                location.Locked = false;
                
                HandleLocationModifiers(location, checkId);
                
                //last map
                if (activeMaps.Count == 1)
                {
                    locationHelper.SetupBossLocation(checkId, location);
                }
            }
            else
            {
                location.IsSecret = false;
                location.Locked = true;
            }

            initial.Locations[id] = location;
        }

        return initial;
    }

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
        
        
        //bot spawns
        foreach (BossLocationSpawn bossSetting in location.BossLocationSpawn)
        {
            if (bossSetting.BossName == "bossPartisan" && modService.HasMod(ModIds.PROFESSIONAL_CAMPER, locationId))
            {
                bossSetting.ForceSpawn = true;
                bossSetting.BossChance = 100;
            }
            
            if (bossSetting.BossName == "sectantPriest" && modService.HasMod(ModIds.NOBODY_EXPECTS_CULT, locationId))
            {
                bossSetting.ForceSpawn = true;
                bossSetting.BossChance = 100;
            }

            if (bossSetting.BossName == "ravangeZryachiyEvent" && modService.HasMod(ModIds.VENGEFUL, locationId))
            {
                bossSetting.BossChance = 100;
            }
        }
        
        if (modService.HasMod(ModIds.BETTER_THINGS_TO_DO, locationId))
        {
            location.EscapeTimeLimit *= 0.5;
            location.EscapeTimeLimitCoop /= 2;
            location.EscapeTimeLimitPVE /= 2;
        }
    }
    
    public List<MongoId> GetActiveMaps()
    {
        return save.CurrentSave.Location.Active
            .Where(l => !l.Value.Completed)
            .Select(l => l.Key).ToList();
    }

    public void HandlePostRaid(MongoId sessionId,
        SptProfile profile,
        bool isDead,
        bool isSurvived,
        string locationName)
    {
        PmcData pmcProfile = profile.CharacterData.PmcData;
        
        if (isDead)
        {
            save.CurrentSave.Profile.Profiles[pmcProfile.Id.Value].Lives--;
            save.SaveCurrentState();
            return;
        }
        
        //not dead but not survived, ran through. No reward.
        if (!isSurvived)
        {
            return;
        }
        
        MongoId locationId = locationUtil.GetMapId(locationName)!;
        locationId = locationUtil.GetMapOther(locationId);

        trialService.CompleteLocation(locationId);

        
        //level player up
        profileHelper.LevelUpPlayer(pmcProfile!, 2);
    }
}