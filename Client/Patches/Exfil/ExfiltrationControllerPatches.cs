using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace KoTClient.Patches;

public class IsMyPlayerBannedPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ExfiltrationControllerClass), nameof(ExfiltrationControllerClass.IsMyPlayerBanned));
    }

    [PatchPrefix]
    public static bool Prefix(ref bool __result)
    {
        if (GamePlayerOwner.MyPlayer == null)
            return true;

        __result = Plugin.RaidService.ExfilQuests.Count != Plugin.RaidService.CompletedQuests.Count;
        
        return false;
    }
}

public class InitExfiltrationPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ExfiltrationControllerClass),
            nameof(ExfiltrationControllerClass.InitAllExfiltrationPoints));
    }

    [PatchPostfix]
    public static void Postfix(ExfiltrationControllerClass __instance)
    {
        __instance.DisableExitsInteraction();
    }
}