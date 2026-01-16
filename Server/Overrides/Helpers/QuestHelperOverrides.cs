using System.Reflection;
using KingOfTarkov.Helpers;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;

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