using KingOfTarkov.Models.Save;
using KingOfTarkov.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Utils;

namespace KingOfTarkov.Helpers;

//help deal with custom quests
[Injectable(InjectionType.Singleton)]
public class KingQuestHelper(ProfileHelper profileHelper,
    SaveService saveService,
    ISptLogger<KingQuestHelper> logger)
{
    public List<Quest> RetrieveDynamicQuests(MongoId sessionId)
    {
        PmcData? profile = profileHelper.GetPmcProfile(sessionId);

        if (profile == null)
        {
            return [];
        }

        List<Quest> result = [];

        SaveState currentState = saveService.CurrentSave;
        
        //add all exfil quests, they should be removed when a location is completed
        result.AddRange(currentState.Quests.Exfil.Values);

        ProfileInfoState? profileInfo = currentState.Profile.Profiles.GetValueOrDefault(profile.Id!.Value);

        if (profileInfo == null)
        {
            logger.Error($"[KoT] Profile {profile.Info.Nickname} not cached by mod!");
            return result;
        }

        Dictionary<MongoId, Quest> quests = currentState.Quests.Personal;

        result.AddRange(profileInfo.Quests.Select(questId => quests[questId]));

        return result;
    }
}