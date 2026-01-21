using System.Reflection;
using EFT;
using EFT.UI;
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
        public static TrialUI TrialInfoObj;
        public static LocationInfoUI LocationInfoObj;
        
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MatchMakerSelectionLocationScreen),
                nameof(MatchMakerSelectionLocationScreen.Awake));
        }

        [PatchPostfix]
        public static void Postfix(MatchMakerSelectionLocationScreen __instance, LocationInfoPanel ____infoPanel)
        {
            GameObject? infoAsset = Plugin.BundleLoader.Bundle.LoadAsset<GameObject>("TrialInfo.prefab");

            if (infoAsset == null)
            {
                NotificationManagerClass.DisplayMessageNotification("Error loading bundle.");
                return;
            }
        
            GameObject trialObj = Object.Instantiate(infoAsset,  __instance.transform)!;
            trialObj.name = "TrialInfo";
            TrialInfoObj = trialObj.GetComponent<TrialUI>();
            
            TrialInfoObj.PrefixLabel.SetText("TrialPrefix".Localized());
            
            //add location info
            Transform locInfoTransform = ____infoPanel.transform.Find("DescriptionPanel");

            GameObject? locInfoAsset = Plugin.BundleLoader.Bundle.LoadAsset<GameObject>("LocationMods.prefab");

            if (locInfoAsset == null)
            {
                NotificationManagerClass.DisplayMessageNotification("Error loading bundle.");
                return;
            }

            GameObject locationInfoObj = Object.Instantiate(locInfoAsset, locInfoTransform);
            locationInfoObj.name = "LocationMods";
            LocationInfoObj = locationInfoObj.GetComponent<LocationInfoUI>();
            
            LocationInfoObj.Header.SetText("LocationHeader".Localized());
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
            
            StateData? trialData = Plugin.StateService.StateData;

            if (trialData == null)
            {
                NotificationManagerClass.DisplayMessageNotification("Trial Data not present in postfix!");
                return;
            }
            
            SelectionAwakePatch.TrialInfoObj.NumLabel.SetText(string.Format("TrialTitleNumber".Localized(), trialData.trial.trialNum));
            SelectionAwakePatch.TrialInfoObj.NameLabel.SetText($"{trialData.trial.trialType} name".Localized());

            if (!ColorUtility.TryParseHtmlString(trialData.color, out Color color))
            {
                NotificationManagerClass.DisplayMessageNotification($"Failed to parse color {trialData.color}!");
                return;
            }
            
            SelectionAwakePatch.TrialInfoObj.NameLabel.color = color;

            Transform modifierHolder = SelectionAwakePatch.TrialInfoObj.transform.Find("ModifierHolder");
            ModifierUI modUI = modifierHolder.gameObject.GetComponent<ModifierUI>();
            
            modUI.Show(___iSession, trialData.trial.mods);
        }
    }
}