using System.Reflection;
using EFT.InventoryLogic;
using EFT.UI;
using HarmonyLib;
using KoTClient.Services;
using KoTClient.UI;
using SPT.Reflection.Patching;
using UnityEngine;

namespace KoTClient.Patches.UI;

public class InventoryScreenAwake : ModulePatch
{
    public static ModifierUI InventoryModUI;
    
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(InventoryScreen), nameof(InventoryScreen.Awake));
    }

    [PatchPostfix]
    public static void Postfix(ItemsPanel ____itemsPanel)
    {
        GameObject? modifierAsset = Plugin.BundleLoader.Bundle.LoadAsset<GameObject>("InventoryModifierHolder.prefab");

        if (modifierAsset == null)
        {
            NotificationManagerClass.DisplayMessageNotification("Error loading bundle.");
            return;
        }

        Transform stashPanel = ____itemsPanel.transform.Find("Stash Panel");
        
        GameObject modifierObj = Object.Instantiate(modifierAsset, stashPanel)!;
        modifierObj.name = "InventoryModifierHelper";
        modifierObj.SetActive(false);
        InventoryModUI = modifierObj.GetComponent<ModifierUI>();
    }
}

public class ItemsPanelShow : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ItemsPanel), nameof(ItemsPanel.Show));
    }

    [PatchPostfix]
    public static void Postfix(ItemsPanel __instance, bool inRaid, CompoundItem lootItem, ISession session, AddViewListClass ___UI)
    {
        if (!inRaid || lootItem != null)
            return;
        
        InventoryScreenAwake.InventoryModUI.Show(session, ModService.ModifierCache.Keys);
        ___UI.AddDisposable(InventoryScreenAwake.InventoryModUI);
    }
}