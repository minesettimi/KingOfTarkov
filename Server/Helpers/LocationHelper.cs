using KingOfTarkov.Models.Save;
using KingOfTarkov.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
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

namespace KingOfTarkov.Helpers;

[Injectable(InjectionType.Singleton)]
public class LocationHelper(SaveService save,
    TimeUtil timeUtil,
    MailSendService mailSendService,
    QuestHelper questHelper,
    DatabaseService databaseService,
    ProfileHelper profileHelper,
    ISptLogger<LocationHelper> logger)
{
    //maps with multiple versions are given the same id to match the system
    private static readonly Dictionary<string, MongoId> _mapNameToId = new()
    {
        {"bigmap", "56f40101d2720b2a4d8b45d6"},
        {"factory4_day", "55f2d3fd4bdc2d5f408b4567"},
        {"factory4_night", "55f2d3fd4bdc2d5f408b4567"},
        {"sandbox", "653e6760052c01c1c805532f"},
        {"sandbox_high", "653e6760052c01c1c805532f"},
        {"interchange", "5714dbc024597771384a510d"},
        {"lighthouse", "5704e4dad2720bb55b8b4567"},
        {"reservbase", "5704e5fad2720bc05b8b4567"},
        {"shoreline", "5704e554d2720bac5b8b456e"},
        {"tarkovstreets", "5714dc692459777137212e12"},
        {"laboratory", "5b0fc42d86f7744a585f9105"},
        {"labyrinth", "6733700029c367a3d40b02af"},
        {"woods","5704e3c2d2720bac5b8b4567"}
    };
    
    //LINQ is messy with dictionaries, do this the classic way
    public List<MongoId> GetActiveMaps()
    {
        List<MongoId> result = [];

        foreach ((MongoId id, LocationDataState data) in save.CurrentSave.Location.Active)
        {
            if (data.Completed) continue;
            
            result.Add(id);
        }

        return result;
    }
    
    //gives other version of map
    public MongoId? GetMapOther(MongoId id)
    {
        if (id == "59fc81d786f774390775787e")
        {
            return new MongoId("55f2d3fd4bdc2d5f408b4567");
        }

        if (id == "65b8d6f5cdde2479cb2a3125")
        {
            return new MongoId("653e6760052c01c1c805532f");
        }

        return null;
    }

    public MongoId MapNameToId(string name)
    {
        MongoId? result = _mapNameToId.GetValueOrDefault(name);

        return result ?? throw new Exception("[KoT] Tried to retrieve id for map with name {name} which isn't in the lookup table.");
    }

    public void HandlePostRaid(MongoId sessionId,
        SptProfile profile,
        bool isDead,
        bool isSurvived,
        string locationName)
    {
        if (isDead)
        {
            //TODO: Hardcore mode
            
            return;
        }
        
        //not dead but not survived, ran through. No reward.
        if (!isSurvived)
        {
            return;
        }
        
        MongoId locationId = MapNameToId(locationName);

        //TODO: FIKA SUPPORT
        save.CurrentSave.Location.Active[locationId].Completed = true;
        save.RemainingRaids--;
        
        //mark trial as different
        save.CurrentSave.Id = new MongoId();

        //trial complete
        if (save.RemainingRaids <= 0)
        {
            save.IncrementTrial();
        }
        
        save.SaveCurrentState();
        
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