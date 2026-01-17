using System.Reflection;
using System.Threading.Tasks;
using EFT;
using EFT.Communications;
using EFT.UI.Matchmaker;
using HarmonyLib;
using KoTClient.Bundles;
using KoTClient.Models;
using KoTClient.Services;
using SPT.Reflection.Patching;
using UnityEngine;

namespace KoTClient.Patches
{
    public class SelectionAwakePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MatchMakerSelectionLocationScreen),
                nameof(MatchMakerSelectionLocationScreen.Awake));
        }

        [PatchPostfix]
        public static void Postfix(MatchMakerSelectionLocationScreen __instance)
        {
            GameObject? testAsset = Plugin.BundleLoader.Bundle.LoadAsset<GameObject>("TrialInfo.prefab");
        
            if (testAsset == null)
                NotificationManagerClass.DisplayMessageNotification("Error loading bundle.");
        
            GameObject obj = Object.Instantiate(testAsset,  __instance.transform);
            obj.name = "TrialInfo";
        }
    }
    
    public class SelectionShowPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MatchMakerSelectionLocationScreen), nameof(MatchMakerSelectionLocationScreen.Show),
                [typeof(ISession), typeof(RaidSettings), typeof(MatchmakerPlayerControllerClass)]);
        }

        [PatchPrefix]
        public static async void Prefix(ISession session,
            RaidSettings raidSettings,
            MatchmakerPlayerControllerClass matchmaker,
            MatchMakerSelectionLocationScreen __instance)
        {
            // StateService stateService = Plugin.StateService;
            //
            // if (stateService.IsStateOutdated())
            // {
            //     session.GetLevelSettings();
            //     await stateService.RequestState();
            // }
            
            //MessageBoxHelper.Show($"Test Message", "KOTTEST", MessageBoxHelper.MessageBoxType.OK);
        }

        [PatchPostfix]
        public static void Postfix(MatchMakerSelectionLocationScreen __instance)
        {
            Transform locationTransform = __instance.gameObject.transform;
            
            locationTransform.Find("Content/Location Info Panel/DescriptionPanel/Location Description").gameObject.SetActive(false);
            locationTransform.Find("CaptionsHolder").gameObject.SetActive(false);

            GameObject trialInfo = locationTransform.Find("TrialInfo").gameObject;
            
            if (trialInfo == null)
            {
                NotificationManagerClass.DisplayMessageNotification("trialInfo not found.");
                return;
            }
            
            TrialUI trialUI = trialInfo.GetComponent<TrialUI>();
            StateData? trialData = Plugin.StateService.stateData;

            if (trialData == null)
            {
                NotificationManagerClass.DisplayMessageNotification("Trial Data not present in postfix!");
                return;
            }
            
            trialUI.PrefixLabel.SetText("TrialPrefix".Localized());
            
            trialUI.NumLabel.SetText(string.Format("TrialTitleNumber".Localized(), trialData.trial.trialNum));
            trialUI.NameLabel.SetText($"{trialData.trial.trialType} name".Localized());

            if (!ColorUtility.TryParseHtmlString(trialData.color, out Color color))
            {
                NotificationManagerClass.DisplayMessageNotification($"Failed to parse color {trialData.color}!");
                return;
            }
            
            trialUI.NameLabel.color = color;
        }
    }
}