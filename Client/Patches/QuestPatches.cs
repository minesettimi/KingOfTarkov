using System.Reflection;
using EFT.Quests;
using EFT.UI;
using HarmonyLib;
using KoTClient.Quests;
using SPT.Reflection.Patching;
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
        if (condition is ConditionTrialNumber || condition is ConditionBossDied)
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
        switch (condition)
        {
            case ConditionTrialNumber:
                __result = __instance.QuestTypeSprites[RawQuestClass.EQuestType.Levelling];
                return false;
            case ConditionBossDied:
                __result = __instance.QuestTypeSprites[RawQuestClass.EQuestType.Elimination];
                return false;
            default:
                return true;
        }
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
        __instance.List_0.Add(typeof(ConditionTrialNumber));
        __instance.List_0.Add(typeof(ConditionBossDied));
    }
}

public class ConditionLocalizationPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Constructor(typeof(ConditionCounterCreator));
    }

    [PatchPostfix]
    public static void Postfix()
    {
        //WHY IS THIS ALREADY SET??????
        ConditionCounterCreator.LocalizationTypes[typeof(ConditionBossDied)] = "{kill}";
    }
}

public class InvokeConditionsPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(QuestControllerAbstractClass<IConditional>), "InvokeConditionsConnector");
    }

    [PatchPrefix]
    public static bool Prefix(QuestClass conditional, EQuestStatus status, 
        Condition condition, QuestControllerAbstractClass<IConditional> __instance)
    {
        if (condition is ConditionTrialNumber conditionTrialNum)
        {
            TrialNumberHandler<IConditional> handler = new()
            {
                controller = __instance,
                condition = conditionTrialNum,
                conditional = conditional,
                status = status
            };

            ConditionProgressChecker conditionProgressChecker = handler.conditional.ProgressCheckers[handler.condition];
            conditionProgressChecker.SetCurrentValueGetter(handler.GetCurrentValue);
            Plugin.StateService.TrialUpdate += handler.OnValueChanged;
            conditionProgressChecker.OnDisconnect += handler.OnDisconnect;
            conditionProgressChecker.OnReset += handler.OnReset;
            
            return false;
        }

        return true;
    }
}

public class SetStatusPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(QuestClass), nameof(QuestClass.SetStatus));
    }

    [PatchPrefix]
    public static void Prefix(QuestClass __instance, ref EQuestStatus status, ref bool notify)
    {
        if (!Plugin.RaidService.ExfilQuests.Contains(__instance.Id) ||
            status != EQuestStatus.AvailableForFinish) return;

        __instance.CompletedConditions.Clear();
        
        foreach (EFT.Quests.Condition? condition in __instance.Conditions[EQuestStatus.AvailableForFinish])
        {
            ConditionCounterCreator counterCreator = condition as ConditionCounterCreator;
            
            TaskConditionCounterClass? counter = __instance.ConditionCountersManager.GetCounter(counterCreator.id);
            
            counter.Reset();
            
            __instance.ProgressCheckers[condition].Reset();
        }
        
        status = EQuestStatus.Expired;
        notify = false;
        Plugin.RaidService.ExfilQuestCompleted(__instance);
    }
}