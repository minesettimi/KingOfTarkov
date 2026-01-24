using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace KoTClient.Patches.Modifiers.Bots;

//spawn cultists like normal bosses
public class BotSettingsPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(BotSettingsRepoClass), nameof(BotSettingsRepoClass.IsSectant));
    }

    [PatchPrefix]
    public static bool Prefix(ref bool __result)
    {
        __result = false;
        return false;
    }
}