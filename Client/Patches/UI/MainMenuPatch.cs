using System.Reflection;
using EFT;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;
using TMPro;
using UnityEngine;

namespace KoTClient.Patches;

public class MainMenuAwakePatch : ModulePatch
{
    public static GameObject LifeCountObj;
    
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(MenuScreen), nameof(MenuScreen.Awake));
    }

    [PatchPostfix]
    public static void Postfix(MenuScreen __instance)
    {
        //move beta up
        Transform menuTransform = __instance.transform;

        GameObject betaWarning = menuTransform.Find("BetaWarningPanel").gameObject;
        RectTransform uiTransform = betaWarning.GetComponent<RectTransform>();

        Vector3 pos = uiTransform.anchoredPosition;
        pos.y += 15;

        uiTransform.anchoredPosition = pos;
        
        GameObject? livesAsset = Plugin.BundleLoader.Bundle.LoadAsset<GameObject>("LifeCount.prefab");

        if (livesAsset == null)
        {
            NotificationManagerClass.DisplayMessageNotification("Error loading bundle.");
            return;
        }

        LifeCountObj = Object.Instantiate(livesAsset, __instance.transform)!;
        LifeCountObj.name = "LifeCount";
    }
}

public class MainMenuShowPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(MenuScreen), nameof(MenuScreen.Show),
            [typeof(Profile), typeof(MatchmakerPlayerControllerClass), typeof(ESessionMode)]);
    }

    [PatchPostfix]
    public static void Postfix(MenuScreen __instance, DefaultUIButton ____playButton)
    {
        int lives = Plugin.StateService.PlayerData.Lives;
        
        if (Plugin.StateService.PlayerData.Lives <= 0)
        {
            ____playButton.Interactable = false;
            ____playButton.SetDisabledTooltip("LivesOutTooltip".Localized());
        }
        
        TextMeshProUGUI lifeText = MainMenuAwakePatch.LifeCountObj.GetComponent<TextMeshProUGUI>();
        lifeText.SetText(string.Format("LivesLeft".Localized(), lives));

        if (lives <= 1)
            lifeText.color = Color.red;
    }
}