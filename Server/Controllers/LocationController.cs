using KingOfTarkov.Services;
using KingOfTarkov.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Utils;

namespace KingOfTarkov.Helpers;

[Injectable(InjectionType.Singleton)]
public class LocationController(SaveService save,
    KingProfileHelper profileHelper,
    LocationUtil locationUtil,
    TrialService trialService,
    ConfigService config,
    ISptLogger<LocationController> logger)
{

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
        profileHelper.LevelUpPlayer(pmcProfile!, config.BaseDifficulty.Trial.LevelPerRaid);
    }
}