using System.Reflection;
using EFT;
using EFT.UI.Matchmaker;
using HarmonyLib;
using KoTClient.Bundles;
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
            GameObject? testAsset = BundleLoader.Instance.Bundle.LoadAsset<GameObject>("TrialInfo.prefab");
        
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
        public static bool Prefix(ISession session,
            RaidSettings raidSettings,
            MatchmakerPlayerControllerClass matchmaker,
            MatchMakerSelectionLocationScreen __instance)
        {
            session.GetLevelSettings();
            //MessageBoxHelper.Show($"Test Message", "KOTTEST", MessageBoxHelper.MessageBoxType.OK);
            
            return true;
        }

        [PatchPostfix]
        public static void Postfix(MatchMakerSelectionLocationScreen __instance)
        {
            Transform locationTransform = __instance.gameObject.transform;
            
            locationTransform.Find("Content/Location Info Panel/DescriptionPanel/Location Description").gameObject.SetActive(false);
            locationTransform.Find("CaptionsHolder").gameObject.SetActive(false);

            GameObject trialInfo = locationTransform.Find("TrialInfo").gameObject;
            if (trialInfo == null)
                NotificationManagerClass.DisplayMessageNotification("trialInfo not found.");
            
            TrialUI test = trialInfo.GetComponent<TrialUI>();
            
            if (test == null)
                NotificationManagerClass.DisplayMessageNotification("TrialUI not present.");
            
            test.NumLabel.SetText("Trial 1:");
            test.NameLabel.SetText("Poison");
            test.NameLabel.color = Color.green;
        }
    }
}