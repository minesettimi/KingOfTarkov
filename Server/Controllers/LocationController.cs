using KingOfTarkov.Services;
using KingOfTarkov.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Profile;
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
        if (isDead)
        {
            //TODO: Lives system
            
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

        PmcData pmcProfile = profile.CharacterData.PmcData;
        
        //level player up
        profileHelper.LevelUpPlayer(pmcProfile!, 1);
    }
}