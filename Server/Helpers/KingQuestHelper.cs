using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Utils;

namespace KingOfTarkov.Helpers;

//help deal with custom quests
[Injectable(InjectionType.Singleton)]
public class KingQuestHelper(ISptLogger<KingQuestHelper> logger)
{
    public Dictionary<MongoId, List<QuestCondition>> TrialQuests = new();
    
    public void CacheTrialQuest(Quest quest)
    {
        foreach (QuestCondition condition in quest.Conditions.AvailableForFinish!)
        {
            if (condition.ConditionType != "TrialCompletion")
            {
                continue;
            }

            if (!TrialQuests.TryGetValue(quest.Id, out List<QuestCondition> conditions))
            {
                conditions ??= [];
                conditions.Add(condition);
                
                TrialQuests.Add(quest.Id, conditions);
                continue;
            }
            
            conditions.Add(condition);
        }
    }

    public List<TaskConditionCounter>? GetActiveTrialCounters(PmcData pmcData)
    {
        return pmcData.TaskConditionCounters?.Values.Where(condition =>
            TrialQuests.ContainsKey(condition.SourceId!.Value) && condition.Type == "SellItemToTrader").ToList();
    }

    public void IncrementTrialCompletion(PmcData profile)
    {
        List<TaskConditionCounter>? activeCounters = GetActiveTrialCounters(profile);

        if (activeCounters == null || activeCounters.Count == 0)
            return;

        foreach (TaskConditionCounter counter in activeCounters)
        {
            if (!TrialQuests.TryGetValue(counter.SourceId.Value, out List<QuestCondition>? conditions))
            {
                logger.Error("[KoT] Unable to find quest with TrialCompletion type.");
                continue;
            }

            foreach (QuestCondition condition in conditions)
            {
                counter.Value++;
            }
        }
    }
}