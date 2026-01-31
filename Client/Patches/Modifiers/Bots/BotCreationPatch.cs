using System.Reflection;
using EFT;
using HarmonyLib;
using KingOfTarkov.Models;
using KoTClient.Services;
using SPT.Reflection.Patching;

namespace KoTClient.Patches.Modifiers.Bots;

public class BotCreationPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(BotCreationDataClass), nameof(BotCreationDataClass.Create));
    }

    [PatchPrefix]
    public static void Prefix(IGetProfileData profileData)
    {
        if (profileData is not BotProfileDataClass botData)
            return;

        if (ModService.HasMod(ModIds.SCAV_TRAINING) && botData.WildSpawnType_0 == WildSpawnType.assault)
        {
            botData.WildSpawnType_0 = WildSpawnType.pmcBot;
        }
        
        if (ModService.HasMod(ModIds.OOPS_ALL_KILLA) && !botData.IsBossOrFollower())
        {
            botData.WildSpawnType_0 = WildSpawnType.bossKilla;
        }
    }
}