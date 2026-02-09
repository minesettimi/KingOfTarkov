using System.Reflection;
using EFT;
using EFT.Quests;
using HarmonyLib;
using KingOfTarkov.Models;
using KoTClient.Quests;
using KoTClient.Services;
using SPT.Reflection.Patching;

namespace KoTClient.Patches.Modifiers.Bots;

public class CreateCorpsePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(Player), nameof(Player.OnDead));
    }

    [PatchPostfix]
    public static void Postfix(Player __instance)
    {
        if (__instance.IsAI)
        {
            foreach (Player player in __instance.GameWorld.AllAlivePlayersList)
            {
                if (player.IsAI)
                    continue;
                
                foreach (GInterface518 type in player.IEnumerable_0)
                {
                    if (type is not AbstractQuestControllerClass questController)
                        continue;

                    questController.ConditionalBook.TestConditions(1, 
                        new GStruct458(typeof(ConditionBossDied))
                            .Test(__instance.Profile.Info.Settings.Role.ToStringNoBox()),
                        new GStruct458(typeof(ConditionLocation)).Test(__instance.Location));
                }
            }
        }
        
        if (!ModService.HasMod(ModIds.BLACKHOLE_CORPSES)) return;
        
        __instance.ReleaseHand();
        __instance.gameObject.SetActive(false);
    }
}