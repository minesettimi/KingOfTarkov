using System.Reflection;
using System.Threading.Tasks;
using Comfort.Common;
using EFT;
using HarmonyLib;
using KingOfTarkov.Models;
using KoTClient.Services;
using SPT.Reflection.Patching;

namespace KoTClient.Patches.Modifiers.Game;

public class BaseLocalGamePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(BaseLocalGame<EftGamePlayerOwner>), nameof(BaseLocalGame<EftGamePlayerOwner>.method_4));
    }

    [PatchPostfix]
    public static async void Postfix(Task __result, LocalRaidSettings ___localRaidSettings_0)
    {
        if (!ModService.HasMod(ModIds.SUDDEN_BLIZZARD)) return;
        
        await __result;

        BackendConfigSettingsClass instance = Singleton<BackendConfigSettingsClass>.Instance;
        Singleton<GameWorld>.Instance.RunddansController = new LocalGameRunddansControllerClass(
            instance.runddansSettings, ___localRaidSettings_0.selectedLocation);
    }
}