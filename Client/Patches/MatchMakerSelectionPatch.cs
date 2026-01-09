using System.Reflection;
using EFT;
using EFT.UI.Matchmaker;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace KoTClient.Patches
{
    
    public class MatchMakerSelectionPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MatchMakerSelectionLocationScreen), nameof(MatchMakerSelectionLocationScreen.Show),
                [typeof(ISession), typeof(RaidSettings), typeof(MatchmakerPlayerControllerClass)]);
        }

        [PatchPrefix]
        public static bool Prefix(ISession session, RaidSettings raidSettings, MatchmakerPlayerControllerClass matchmaker)
        {
            session.GetLevelSettings();
            //MessageBoxHelper.Show($"Test Message", "KOTTEST", MessageBoxHelper.MessageBoxType.OK);
            NotificationManagerClass.DisplayMessageNotification("Test Message");
            
            return true;
        }
    }
}