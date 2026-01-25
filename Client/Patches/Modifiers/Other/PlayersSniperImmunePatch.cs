using System.Reflection;
using EFT.Interactive;
using HarmonyLib;
using KingOfTarkov.Models;
using KoTClient.Services;
using SPT.Reflection.Patching;

namespace KoTClient.Patches.Modifiers.Other;

public class PlayersSniperImmunePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(PlayersWithImmuneToSniperFireCollector),
            nameof(PlayersWithImmuneToSniperFireCollector.method_0));
    }

    [PatchPrefix]
    public static bool Prefix()
    {
        return !ModService.HasMod(ModIds.BLOOD_SNIPERS);
    }
}