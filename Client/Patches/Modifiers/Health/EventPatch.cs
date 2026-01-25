using System.Reflection;
using EFT.HealthSystem;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace KoTClient.Patches.Health;

public class EventPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ActiveHealthController), nameof(ActiveHealthController.DoEventEffect));
    }

    //disable vengeful misfire, its not suitable for limited life gameplay
    [PatchPrefix]
    public static bool Prefix()
    {
        return false;
    }
}