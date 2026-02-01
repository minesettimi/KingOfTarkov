using System.Reflection;
using HarmonyLib;
using KingOfTarkov.Models;
using KoTClient.Services;
using SPT.Reflection.Patching;

namespace KoTClient.Patches.Modifiers.Bots;

public class ZoneLeaveControllerPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ZoneLeaveControllerClass), nameof(ZoneLeaveControllerClass.method_3));
    }

    [PatchPrefix]
    public static bool Prefix()
    {
        return !ModService.HasMod(ModIds.NOBODY_EXPECTS_CULT);
    }
}