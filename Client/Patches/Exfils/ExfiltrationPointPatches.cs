using System.Reflection;
using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace KoTClient.Patches;

public class ProceedPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ExfiltrationPoint), nameof(ExfiltrationPoint.Proceed));
    }

    [PatchPostfix]
    public static bool Prefix()
    {
        return Plugin.RaidService.ExfilQuests.Count == 0;
    }
}