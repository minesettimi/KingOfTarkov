using KingOfTarkov.Generators;
using KingOfTarkov.Models.Response;
using KingOfTarkov.Models.Save;
using KingOfTarkov.Services;
using KingOfTarkov.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Utils;

namespace KingOfTarkov.Helpers;

[Injectable(InjectionType.Singleton)]
public class LocationController(SaveService save,
    KingProfileHelper kingProfileHelper,
    ProfileHelper profileHelper,
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
        }
        else if (isSurvived) //only if survived, not if run-through
        {
            kingProfileHelper.LevelUpPlayer(pmcProfile, config.Difficulty.Trial.LevelPerRaid);
        }
            
        save.SaveCurrentState();
    }

    public void HandleMatchEnd(MatchEndRequest matchEndRequest, MongoId sessionId)
    {
        PmcData? profile = profileHelper.GetPmcProfile(sessionId);

        if (profile == null)
        {
            logger.Error("[KoT] Unable to retrieve profile for match end route.");
            return;
        }

        if (!matchEndRequest.Survived || matchEndRequest.TrialId != save.CurrentSave.Id)
            return;
        
        MongoId locationId = locationUtil.GetMapId(matchEndRequest.LocationName.ToLower());
        locationId = locationUtil.GetMapOther(locationId);

        trialService.CompleteLocation(locationId);
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