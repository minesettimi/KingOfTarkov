using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace KoTClient.Patches;

public class LocaleClassRunPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(LocaleClass), nameof(LocaleClass.Run));
    }

    [PatchPostfix]
    public static async void Postfix()
    {
        await Plugin.StateService.RequestState();
    }
}