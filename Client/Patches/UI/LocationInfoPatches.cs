using System.Reflection;
using EFT;
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

        MongoID locationId = location._Id;
        
        //allow sandboxHigh and factory night to show info
        if (locationId == "59fc81d786f774390775787e")
        {
            locationId = "55f2d3fd4bdc2d5f408b4567";
        }

        if (locationId == "65b8d6f5cdde2479cb2a3125")
        {
            locationId = "653e6760052c01c1c805532f";
        }
        
        Plugin.StateService.StateData!.location.TryGetValue(locationId, out LocationData? locationData);

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