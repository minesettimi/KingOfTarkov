using System.Reflection;
using EFT.UI.Matchmaker;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace KoTClient.Patches;

public class SideSelectionPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(MatchMakerSideSelectionScreen), nameof(MatchMakerSideSelectionScreen.Awake));
    }

    [PatchPostfix]
    public static void Postfix(MatchMakerSideSelectionScreen __instance)
    {
        GameObject selectionObject = __instance.gameObject;

        //disable scav area
        GameObject savageArea = selectionObject.transform.Find("Savage").gameObject;
        savageArea.SetActive(false);
        
        //move PMC to center
        Transform pmcArea = selectionObject.transform.Find("PMCs");

        foreach (Transform? child in pmcArea.GetChildren())
        {
            if (child == null)
                continue;

            RectTransform rectTransform = child.GetComponent<RectTransform>();
            
            Vector3 position = rectTransform.anchoredPosition;

            position.x -= 225;
            
            rectTransform.anchoredPosition = position;
        }
    }
}