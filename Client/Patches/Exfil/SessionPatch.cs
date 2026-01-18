using System;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace KoTClient.Patches;

public class GetLevelSettingsPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(Class308), nameof(Class308.GetLevelSettings));
    }

    [PatchPostfix]
    public static async Task<LocationSettingsClass> Prefix(Task<LocationSettingsClass> __result)
    {
        LocationSettingsClass result = await __result;
        
        Plugin.StateService.RequestState();
        
        //its executed both on launch and post raid, should work fine
        bool playerResult = await Plugin.StateService.RequestPlayerState();

        if (!playerResult)
            throw new Exception("Failed to validate profile!");

        return result;
    }
}