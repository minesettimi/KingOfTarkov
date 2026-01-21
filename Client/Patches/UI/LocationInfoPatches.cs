using System.Reflection;
using EFT.UI;
using HarmonyLib;
using KoTClient.Models;
using KoTClient.UI;
using SPT.Reflection.Patching;

namespace KoTClient.Patches;

public class LocationInfoSetPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(LocationInfoPanel), nameof(LocationInfoPanel.Set));
    }

    [PatchPostfix]
    public static void Set(LocationInfoPanel __instance, LocationSettingsClass.Location? location, ISession session)
    {
        //didn't properly run
        if (location == null)
        {
            return;
        }
        
        LocationInfoUI infoObj = SelectionAwakePatch.LocationInfoObj;
        Plugin.StateService.StateData!.location.TryGetValue(location._Id, out LocationData? locationData);

        //not a valid map
        if (locationData == null)
        {
            infoObj.gameObject.SetActive(false);
            return;
        }

        infoObj.gameObject.SetActive(true);
        
        infoObj.ExfilQuestLabel.SetText(string.Format("LocationExfils".Localized(), locationData.exfilRequirements.Count));
        infoObj.ModHolder.Show(session, locationData.mods);
    }
}