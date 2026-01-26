using System.Reflection;
using KingOfTarkov.Helpers;
using KoTServer.Controllers;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Quests;

namespace KingOfTarkov.Overrides.Helpers;

public class GetClientQuestsOverride : AbstractPatch
{
    private static KingQuestHelper _questHelper;
    
    protected override MethodBase? GetTargetMethod()
    {
        _questHelper = ServiceLocator.ServiceProvider.GetService<KingQuestHelper>();
        return typeof(QuestHelper).GetMethod(nameof(QuestHelper.GetClientQuests));
    }

    [PatchPostfix]
    public static List<Quest> Postfix(List<Quest> __result, MongoId sessionID)
    {
        //if there was an error, it outputs an empty array
        if (__result.Count > 0)
            __result.AddRange(_questHelper.RetrieveDynamicQuests(sessionID));
        
        return __result;
    }
}

public class GetQuestFromDbOverride : AbstractPatch
{
    private static KingQuestHelper _questHelper;
    
    protected override MethodBase? GetTargetMethod()
    {
        _questHelper = ServiceLocator.ServiceProvider.GetService<KingQuestHelper>();
        return typeof(QuestHelper).GetMethod(nameof(QuestHelper.GetQuestFromDb));
    }

    [PatchPrefix]
    public static bool Prefix(MongoId questId, ref Quest? __result)
    {
        Quest? dynamicQuest = _questHelper.GetDynamicQuest(questId);
        if (dynamicQuest == null)
            return true;

        __result = dynamicQuest;
        return false;
    }
}

public class CompleteQuestOverride : AbstractPatch
{
    private static KingQuestController _questController;
    private static KingQuestHelper _questHelper;
    
    protected override MethodBase? GetTargetMethod()
    {
        _questHelper = ServiceLocator.ServiceProvider.GetService<KingQuestHelper>();
        _questController = ServiceLocator.ServiceProvider.GetService<KingQuestController>();
        return typeof(QuestHelper).GetMethod(nameof(QuestHelper.CompleteQuest));
    }

    [PatchPostfix]
    public static void Postfix(PmcData pmcData, CompleteQuestRequestData request)
    {
        MongoId questId = request.QuestId;
        
        Quest? dynamicQuest = _questHelper.GetDynamicQuest(questId);
        if (dynamicQuest == null)
            return;
        
        _questController.HandleQuest(dynamicQuest, pmcData);

        pmcData.Quests!.RemoveAll(quest => quest.QId == questId);
    }
}