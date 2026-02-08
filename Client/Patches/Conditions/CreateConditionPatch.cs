using System.Reflection;
using EFT.Quests;
using HarmonyLib;
using KoTClient.Quests;
using SPT.Reflection.Patching;

namespace KoTClient.Patches.Conditions;

public class CreateConditionPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(GClass4048), nameof(GClass4048.Create));
    }

    [PatchPrefix]
    public static bool Prefix(Condition condition, ref ConditionProgressChecker __result)
    {
        if (condition is ConditionBossDied conditionBossDied)
        {
            __result = new BossDiedChecker(conditionBossDied);
            return false;
        }

        return true;
    }
}