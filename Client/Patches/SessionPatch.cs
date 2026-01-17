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
    public static async void Prefix(Task __result)
    {
        await __result;
        
        await Plugin.StateService.RequestState();
    }
}