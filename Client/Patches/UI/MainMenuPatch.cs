using System.Reflection;
using EFT;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace KoTClient.Patches;

public class MainMenuSpawnPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(MenuScreen), nameof(MenuScreen.Show),
            [typeof(Profile), typeof(MatchmakerPlayerControllerClass), typeof(ESessionMode)]);
    }

    [PatchPostfix]
    public static void Postfix(DefaultUIButton ____playButton)
    {
        if (Plugin.StateService.PlayerData.Lives == 0)
        {
            ____playButton.Interactable = false;
            ____playButton.SetDisabledTooltip("LivesOutTooltip".Localized());
        }
    }
}