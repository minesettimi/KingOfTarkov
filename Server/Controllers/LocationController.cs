using KingOfTarkov.Models.Database;
using KingOfTarkov.Models.Save;
using KingOfTarkov.Services;
using KingOfTarkov.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using SPTarkov.Server.Core.Utils.Cloners;

namespace KingOfTarkov.Helpers;

[Injectable(InjectionType.Singleton)]
public class LocationController(SaveService save,
    TimeUtil timeUtil,
    MailSendService mailSendService,
    QuestHelper questHelper,
    DatabaseService databaseService,
    ProfileHelper profileHelper,
    RepeatableQuestHelper repeatableQuestHelper,
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
        
        //TODO: Custom token

        Item rewardToken = new()
        {
            Id = new MongoId(),
            Template = "6656560053eaaa7a23349c86"
        };

        PmcData pmcProfile = profile.CharacterData.PmcData;
        
        //give player reward
        int mailRewardTime = timeUtil.GetHoursAsSeconds((int)questHelper.GetMailItemRedeemTimeHoursForProfile(pmcProfile));
        mailSendService.SendLocalisedNpcMessageToPlayer(sessionId,
            Traders.FENCE,
            MessageType.MessageWithItems,
            "TrialReward",
            [rewardToken],
            mailRewardTime);

        int playerLevel = pmcProfile.Info?.Level ?? 1;
        
        //level up
        pmcProfile.Info.Experience = profileHelper.GetExperience(playerLevel + 1);
        pmcProfile.Info.Level =
            pmcProfile.CalculateLevel(databaseService.GetGlobals().Configuration.Exp.Level.ExperienceTable); 
    }
}