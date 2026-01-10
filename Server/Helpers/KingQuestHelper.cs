using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace KingOfTarkov.Helpers;

//help deal with custom quests
[Injectable(InjectionType.Singleton)]
public class KingQuestHelper
{
    public Dictionary<MongoId, List<QuestCondition>> TrialQuests = new();
    
    public void CacheTrialQuest(MongoId tpl, Quest quest)
    {
        foreach (QuestCondition condition in quest.Conditions.AvailableForFinish!)
        {
            if (condition.ConditionType)
        }
    }
}