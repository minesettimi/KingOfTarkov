using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using KingOfTarkov.Models;
using KoTClient.Services;
using SPT.Reflection.Patching;

namespace KoTClient.Patches.Modifiers.Game;

public class SetTimePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(BaseLocalGame<EftGamePlayerOwner>),
            nameof(BaseLocalGame<EftGamePlayerOwner>.method_3));
    }

    [PatchPostfix]
    public static void Postfix(BaseLocalGame<EftGamePlayerOwner> __instance, GameDateTime ___gameDateTime_1)
    {
        if (!ModService.HasMod(ModIds.WHO_TURNED_OFF_LIGHTS)) return;
        
        __instance.GameDateTime.Reset(___gameDateTime_1.DateTime_0, 
            new DateTime(2025, 1, 1, 1, 1, 1),
            0);
    }
}