using KingOfTarkov.Generators;
using KingOfTarkov.Models.Save;
using KingOfTarkov.Services;
using KingOfTarkov.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Utils;

namespace KingOfTarkov.Helpers;

[Injectable(InjectionType.Singleton)]
public class LocationController(SaveService save,
    KingProfileHelper profileHelper,
    LocationUtil locationUtil,
    TrialService trialService,
    QuestGenerator questGenerator,
    ConfigService config,
    ISptLogger<LocationController> logger)
{

    public void HandlePostRaid(MongoId sessionId,
        SptProfile profile,
        bool isDead,
        bool isSurvived,
        string locationName)
    {
        PmcData pmcProfile = profile.CharacterData!.PmcData!;
        save.CurrentSave.Profile.Profiles.TryGetValue(pmcProfile.Id!.Value, out ProfileInfoState? profileData);

        if (profileData == null)
            return;
        
        if (isDead)
        {
            if (--profileData.Lives == 0 && config.Difficulty.Core.Revives && profileData.Revives > 0)
            {
                profileData.Revives--;
                GivePlayerReviveQuest(pmcProfile.Id!.Value);
            }
            
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
        profileHelper.LevelUpPlayer(pmcProfile!, config.Difficulty.Trial.LevelPerRaid);
    }
    
    public void GivePlayerReviveQuest(MongoId playerId)
    {
        Quest? reviveQuest = questGenerator.GenerateReviveQuest();

        if (reviveQuest == null)
            return;

        ProfileInfoState profileData = save.CurrentSave.Profile.Profiles[playerId];
        profileData.Quests.Add(reviveQuest.Id);
        
        save.CurrentSave.Quests.Personal.Add(reviveQuest.Id, reviveQuest);
    }
}