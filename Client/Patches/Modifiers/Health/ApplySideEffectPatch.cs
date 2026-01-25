using System;
using System.Reflection;
using EFT.HealthSystem;
using HarmonyLib;
using KingOfTarkov.Models;
using KoTClient.Services;
using SPT.Reflection.Patching;

namespace KoTClient.Patches.Health;

public class ApplySideEffectPatch : ModulePatch
{
    private static Type testType;
    private static MethodInfo applyBuff;
    
    protected override MethodBase GetTargetMethod()
    {
        testType = typeof(ActiveHealthController).GetNestedType("Stimulator");
        
        return AccessTools.Method(typeof(ActiveHealthController), nameof(ActiveHealthController.TryApplySideEffects));
    }

    [PatchPostfix]
    public static bool Postfix(bool __result, ActiveHealthController __instance, DamageInfoStruct damage)
    {
        if (!__instance.IsAlive || __instance.DamageCoeff <= 0 || damage.Weapon == null || !ModService.HasMod(ModIds.POISON_BULLETS))
            return __result;
        
        //maybe do a more refined system later
        __instance.TryDoExternalBuff("Buffs_KultistsToxin");

        return true;
    }
    
    
}