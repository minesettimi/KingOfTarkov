using System.Reflection;
using KingOfTarkov.Helpers;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace KingOfTarkov.Overrides.Helpers;

public class GetQuestDbRewardOverride : AbstractPatch
{
    private static KingQuestHelper _questHelper;
    
    protected override MethodBase? GetTargetMethod()
    {
        _questHelper = ServiceLocator.ServiceProvider.GetService<KingQuestHelper>();
        return typeof(QuestRewardHelper).GetMethod("GetQuestFromDb", BindingFlags.Instance | BindingFlags.NonPublic);
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