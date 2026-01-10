using System.Reflection;
using EFT.Quests;
using EFT.UI;
using HarmonyLib;
using KoTClient.Quests;
using SPT.Reflection.Patching;
using SPT.Reflection.Utils;
using UnityEngine;

namespace KoTClient.Patches;

public class ConditionIsCounterPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(GClass4013), nameof(GClass4013.ConditionIsCounter));
    }

    [PatchPrefix]
    public static bool Prefix(Condition condition, ref bool __result)
    {
        if (condition is ConditionTrialCompletion)
        {
            __result = true;
            return false;
        }
        
        return true;
    }
}

public class GetQuestIconPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(StaticIcons), nameof(StaticIcons.GetQuestIcon));
    }

    [PatchPrefix]
    public static bool Prefix(Condition condition, StaticIcons __instance, ref Sprite __result)
    {
        if (condition is ConditionTrialCompletion)
        {
            __result = __instance.QuestTypeSprites[RawQuestClass.EQuestType.Levelling];
            return false;
        }

        return true;
    }
}

public class ConditionTypePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(GClass1871).GetConstructor([])!;
    }

    [PatchPostfix]
    public static void Prefix(GClass1871 __instance)
    {
        __instance.List_0.Add(typeof(ConditionTrialCompletion));
    }
}

// public class InvokeConditionsPatch : ModulePatch
// {
//     protected override MethodBase GetTargetMethod()
//     {
//         return AccessTools.Method(typeof(AbstractQuestClass<QuestClass>), "InvokeConditionsConnector");
//     }
// }