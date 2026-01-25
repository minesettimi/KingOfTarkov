using System.Reflection;
using KingOfTarkov.Helpers;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Generators.RepeatableQuestGeneration;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Repeatable;

namespace KingOfTarkov.Overrides.Generators;

public class TryGetLocationInfoOverride : AbstractPatch
{
    private static KingQuestHelper _questHelper;
    
    protected override MethodBase? GetTargetMethod()
    {
        _questHelper = ServiceLocator.ServiceProvider.GetService<KingQuestHelper>();
        return typeof(ExplorationQuestGenerator).GetMethod("TryGetLocationInfo",
            BindingFlags.Instance | BindingFlags.NonPublic);
    }

    [PatchPrefix]
    public static void Prefix(QuestTypePool pool)
    {
        _questHelper.RemoveUnusedLocations(pool);
    }
}

//fix bug by hard setting it
public class GetNumberOfExitsOverride : AbstractPatch
{
    protected override MethodBase? GetTargetMethod()
    {
        return typeof(ExplorationQuestGenerator).GetMethod("GetNumberOfExits",
            BindingFlags.Instance | BindingFlags.NonPublic);
    }

    [PatchPrefix]
    public static bool Prefix(ref int __result)
    {
        __result = 1;
        return false;
    }
}