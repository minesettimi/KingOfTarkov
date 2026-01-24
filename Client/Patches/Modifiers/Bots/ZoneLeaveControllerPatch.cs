using System.Reflection;
using HarmonyLib;
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
        return false;
    }
}