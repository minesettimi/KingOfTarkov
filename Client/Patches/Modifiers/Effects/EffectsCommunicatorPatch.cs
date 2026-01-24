using System;
using System.Reflection;
using Comfort.Common;
using EFT;
using HarmonyLib;
using KingOfTarkov.Models;
using KoTClient.Services;
using SPT.Reflection.Patching;
using Systems.Effects;
using UnityEngine;

namespace KoTClient.Patches.Modifiers;

public class EffectsCommunicatorPatch : ModulePatch
{

    private static readonly MineDirectional.MineSettings ExplosionSettings = new()
    {
        _blindness = new Vector3(6, 6, 6),
        _contusion = new Vector3(6, 6, 6),
        _maxExplosionDistance = 2,
        _fragmentsCount = 0,
        _strength = 3,
        _tag = "default",
        _armorDamage = 0.5f,
        _staminaBurnRate = 5,
        _penetrationPower = 20,
        _fragmentType = "5996f6d686f77467977ba6cc",
        _fxName = "Grenade_indoor",
        _directionalDamageAngle = 30.0f,
        _directionalDamageMultiplier = 4
    };

    private static Lazy<ISharedBallisticsCalculator> Ballistics;
    
    protected override MethodBase GetTargetMethod()
    {
        Ballistics = new Lazy<ISharedBallisticsCalculator>(GetBallisticsCalc);
        return AccessTools.Method(typeof(EffectsCommutator), nameof(EffectsCommutator.PlayHitEffect));
    }

    [PatchPrefix]
    public static bool Prefix(EffectsCommutator __instance, EftBulletClass info, ShotInfoClass playerHitInfo)
    {
        if (!ModService.HasMod(ModIds.EXPLOSIVE_BULLETS))
            return true;
        
        if (__instance.IsHitPointAlreadyProcessed(info.HitPoint))
            return false;
        
        if (info.HittedBallisticCollider == null)
            return true;

        DamageInfoProvider damageInfo = new(info.Player);
        
        Singleton<Effects>.Instance.EmitGrenade(ExplosionSettings.FXName, info.HitPoint, Vector3.up);
        
        ExplosionSettings.Explosion(info.HitPoint, info.PlayerProfileID, Ballistics.Value, null, damageInfo.GetDamageInfo, ExplosionSettings.GetDirectionalDamageMultiplier, ExplosionSettings.GetDirectionalDamageAngle, info.Direction, false);

        return true;
    }

    private class DamageInfoProvider(IPlayerOwner player)
    {
        public DamageInfoStruct GetDamageInfo()
        {
            return new DamageInfoStruct
            {
                DamageType = EDamageType.Explosion,
                ArmorDamage = ExplosionSettings.ArmorDamage,
                StaminaBurnRate = ExplosionSettings.StaminaBurnRate,
                PenetrationPower = ExplosionSettings.PenetrationPower,
                Direction = Vector3.zero,
                Player = player,
                IsForwardHit = false
            };
        }
    }

    private static ISharedBallisticsCalculator GetBallisticsCalc()
    {
        return Singleton<GInterface169>.Instance.CreateBallisticCalculator(0);
    }
}