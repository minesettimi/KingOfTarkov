using KingOfTarkov.Models.Save;
using KingOfTarkov.Services;
using KingOfTarkov.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Repeatable;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;

namespace KingOfTarkov.Helpers;

//help deal with custom quests
[Injectable(InjectionType.Singleton)]
public class KingQuestHelper(ProfileHelper profileHelper,
    SaveService saveService,
    TimeUtil timeUtil,
    LocationUtil locationUtil,
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

        foreach (MongoId questId in profileInfo.Quests)
        {
            //add quest status to profile if it isn't there already
            //CreateQuestStatusIfAvailable(profile, questId);
            result.Add(quests[questId]);
        }

        return result;
    }

    private void CreateQuestStatusIfAvailable(PmcData pmcData, MongoId questId)
    {
        QuestStatus? questStatus = pmcData.Quests.FirstOrDefault(q => q.QId == questId);

        if (questStatus == null)
            return;

        QuestStatus newStatus = new()
        {
            QId = questId,
            StartTime = timeUtil.GetTimeStamp(),
            Status = QuestStatusEnum.Started,
            StatusTimers = new Dictionary<QuestStatusEnum, double>()
        };
        
        pmcData.Quests.Add(newStatus);
    }

    public void RemoveUnusedLocations(QuestTypePool pool)
    {
        foreach (KeyValuePair<ELocationName, List<string>> location in pool.Pool.Exploration.Locations)
        {
            //get id
            string mapKey = location.Key.ToString().ToLower();

            MongoId mapId = locationUtil.GetMapId(mapKey);

            if (!saveService.CurrentSave.Location.Active.ContainsKey(mapId))
            {
                pool.Pool.Exploration.Locations.Remove(location.Key);
            }
        }
    }

    public Quest? GetDynamicQuest(MongoId id)
    {
        QuestState questState = saveService.CurrentSave.Quests;

        if (questState.Personal.TryGetValue(id, out Quest? quest) || questState.Exfil.TryGetValue(id, out quest))
            return quest;

        return null;
    }
}