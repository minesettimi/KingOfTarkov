using KingOfTarkov.Generators;
using KingOfTarkov.Services;
using KingOfTarkov.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Spt.Services;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;

namespace KingOfTarkov.Helpers;

[Injectable(InjectionType.Singleton)]
public class KingProfileHelper(SaveServer saveServer,
    ProfileHelper profileHelper,
    DatabaseService databaseService,
    ProfileActivityService profileActivityService,
    LocationUtil locationUtil,
    ConfigService config,
    SaveService saveService,
    ISptLogger<KingProfileHelper> logger)
{
    public void SetupTrialForProfiles(bool newTrial)
    {
        foreach ((MongoId session, SptProfile profile) in saveServer.GetProfiles())
        {
            if (saveServer.IsProfileInvalidOrUnloadable(session))
                continue;
            
            PmcData playerData = profile.CharacterData!.PmcData!;

            if (!saveService.CurrentSave.Profile.Profiles.ContainsKey(playerData.Id!.Value) || 
                profile.ProfileInfo!.Edition != "KingOfTarkov")
                continue;

            foreach (PmcDataRepeatableQuest quest in playerData.RepeatableQuests!)
            {
                quest.EndTime = 0;
            }

            if (newTrial) continue;

            LevelUpPlayer(playerData, config.Difficulty.Trial.LevelPerTrial);
        }
    }

    public void LevelUpPlayer(PmcData profile, int number)
    {
        int playerLevel = profile.Info?.Level ?? 1;
        
        //level up
        profile.Info.Experience = profileHelper.GetExperience(playerLevel + number);
        profile.Info.Level =
            profile.CalculateLevel(databaseService.GetGlobals().Configuration.Exp.Level.ExperienceTable); 
    }

    public MongoId? GetLocationFromSession(MongoId sessionId)
    {
        ProfileActivityRaidData raidData = profileActivityService.GetProfileActivityRaidData(sessionId);
        string? locationName = raidData.RaidConfiguration?.Location?.ToLower();

        return locationName != null ? locationUtil.GetMapId(locationName) : null;
    }
}