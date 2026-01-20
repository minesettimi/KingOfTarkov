using System.Reflection;
using EFT;
using EFT.UI.Matchmaker;
using HarmonyLib;
using KoTClient.Models;
using KoTClient.UI;
using SPT.Reflection.Patching;
using UnityEngine;

namespace KoTClient.Patches
{
    public class SelectionAwakePatch : ModulePatch
    {
        public static GameObject TrialInfoObj;
        
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MatchMakerSelectionLocationScreen),
                nameof(MatchMakerSelectionLocationScreen.Awake));
        }

        [PatchPostfix]
        public static void Postfix(MatchMakerSelectionLocationScreen __instance)
        {
            GameObject? infoAsset = Plugin.BundleLoader.Bundle.LoadAsset<GameObject>("TrialInfo.prefab");

            if (infoAsset == null)
            {
                NotificationManagerClass.DisplayMessageNotification("Error loading bundle.");
                return;
            }
        
            TrialInfoObj = Object.Instantiate(infoAsset,  __instance.transform)!;
            TrialInfoObj.name = "TrialInfo";
            
            TrialUI trialUI = TrialInfoObj.GetComponent<TrialUI>();
            trialUI.PrefixLabel.SetText("TrialPrefix".Localized());
        }
    }
    
    public class SelectionShowPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MatchMakerSelectionLocationScreen), nameof(MatchMakerSelectionLocationScreen.Show),
                [typeof(ISession), typeof(RaidSettings), typeof(MatchmakerPlayerControllerClass)]);
        }

        [PatchPostfix]
        public static void Postfix(MatchMakerSelectionLocationScreen __instance, ISession ___iSession)
        {
            Transform locationTransform = __instance.gameObject.transform;
            
            locationTransform.Find("Content/Location Info Panel/DescriptionPanel/Location Description").gameObject.SetActive(false);
            locationTransform.Find("CaptionsHolder").gameObject.SetActive(false);
            
            TrialUI trialUI = SelectionAwakePatch.TrialInfoObj.GetComponent<TrialUI>();
            StateData? trialData = Plugin.StateService.StateData;

            if (trialData == null)
            {
                NotificationManagerClass.DisplayMessageNotification("Trial Data not present in postfix!");
                return;
            }
            
            trialUI.NumLabel.SetText(string.Format("TrialTitleNumber".Localized(), trialData.trial.trialNum));
            trialUI.NameLabel.SetText($"{trialData.trial.trialType} name".Localized());

            if (!ColorUtility.TryParseHtmlString(trialData.color, out Color color))
            {
                NotificationManagerClass.DisplayMessageNotification($"Failed to parse color {trialData.color}!");
                return;
            }
            
            trialUI.NameLabel.color = color;


            Transform modifierHolder = SelectionAwakePatch.TrialInfoObj.transform.Find("ModifierHolder");
            ModifierUI modUI = modifierHolder.gameObject.GetComponent<ModifierUI>();
            
            modUI.ShowModifiers(___iSession, trialData.trial.mods);
        }
    }
}