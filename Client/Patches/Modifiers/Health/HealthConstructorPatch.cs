using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using KingOfTarkov.Models;
using KoTClient.Services;
using SPT.Reflection.Patching;

namespace KoTClient.Patches.Health;

public class HealthConstructorPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Constructor(typeof(GClass3010),
        [
            typeof(Profile.ProfileHealthClass), typeof(Player), typeof(InventoryController), typeof(SkillManager),
            typeof(bool)
        ]);
    }

    [PatchPrefix]
    public static void Prefix(bool aiHealth)
    {
        if (aiHealth)
            return;

        GClass3019 effects = Singleton<BackendConfigSettingsClass>.Instance.Health.Effects;

        effects.Existence.EnergyLoopTime = ModService.HasMod(ModIds.EXTREME_METABOLISM) ? 45f : 60f;
        effects.Existence.HydrationLoopTime = ModService.HasMod(ModIds.DESERT_WEATHER) ? 45f : 60f;
    }

    [PatchPostfix]
    public static void Postfix(GClass3010 __instance)
    {
        if (ModService.HasMod(ModIds.BAD_BATCH))
        {
            __instance.DoExternalBuff("Buffs_BadBatch", 1f);
        }

        if (ModService.HasMod(ModIds.RADIATION_LEAK))
        {
            __instance.DoExternalBuff("Buffs_RadLeak", 1f);
        }

        if (ModService.HasMod(ModIds.HIGH_GRAVITY))
        {
            __instance.DoExternalBuff("Buffs_BadBack", 1f);
        }
    }
}