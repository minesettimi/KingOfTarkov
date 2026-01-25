using System.Reflection;
using EFT;
using HarmonyLib;
using KingOfTarkov.Models;
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
        if (!ModService.HasMod(ModIds.BLACKHOLE_CORPSES)) return;
        
        __instance.ReleaseHand();
        __instance.gameObject.SetActive(false);
    }
}