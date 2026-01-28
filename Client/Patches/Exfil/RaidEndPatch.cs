using System.Reflection;
using EFT;
using EFT.Quests;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace KoTClient.Patches;

public class RaidEndPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(Player), nameof(Player.OnGameSessionEnd));
    }

    [PatchPrefix]
    public static void Prefix(Player __instance)
    {
        foreach (QuestClass? quest in __instance.AbstractQuestControllerClass.Quests)
        {
            if (Plugin.RaidService.ExfilQuests.Contains(quest.Id))
            {
                quest.SetStatus(EQuestStatus.Started, false, true);
            }
        }
    }
}