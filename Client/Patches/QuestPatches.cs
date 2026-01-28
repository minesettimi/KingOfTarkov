using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Comfort.Common;
using EFT;
using EFT.Quests;
using EFT.UI;
using HarmonyLib;
using KoTClient.Quests;
using Newtonsoft.Json;
using SPT.Common.Utils;
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
        if (condition is ConditionTrialNumber)
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
        if (condition is ConditionTrialNumber)
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
        __instance.List_0.Add(typeof(ConditionTrialNumber));
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

public class NotifyStatusChangedPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(GClass4005), nameof(GClass4005.TryNotifyConditionalStatusChanged));
    }

    [PatchPostfix]
    public static void Postfix(QuestClass quest)
    {
        //only want our exfil quests, while in raid, and when it has been completed
        if (!Singleton<AbstractGame>.Instantiated || 
            !Singleton<AbstractGame>.Instance.InRaid || 
            Plugin.RaidService.CurrentLocation == null || 
            !Plugin.RaidService.ExfilQuests.Contains(quest.Id) ||
            quest.QuestStatus != EQuestStatus.AvailableForFinish)
            return;
        
        Plugin.RaidService.ExfilQuestCompleted(quest);
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
        
        foreach (Condition? condition in __instance.Conditions[EQuestStatus.AvailableForFinish])
        {
            ConditionCounterCreator counterCreator = condition as ConditionCounterCreator;
            
            TaskConditionCounterClass? counter = __instance.ConditionCountersManager.GetCounter(counterCreator.id);
            
            Plugin.PluginLogger.LogInfo($"[KoT] {JsonConvert.SerializeObject(counter)}");
            counter.Reset();
            
            __instance.ProgressCheckers[condition].Reset();
        }
        
        status = EQuestStatus.Expired;
        notify = false;
        Plugin.RaidService.ExfilQuestCompleted(__instance);
    }
}