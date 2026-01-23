using System.Reflection;
using EFT;
using HarmonyLib;
using KingOfTarkov.Models;
using KoTClient.Services;
using SPT.Reflection.Patching;

namespace KoTClient.Patches.Modifiers.Bots;

public class StandardBrainPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(StandartBotBrain), nameof(StandartBotBrain.Activate));
    }

    [PatchPostfix]
    public static void Postfix(StandartBotBrain __instance)
    {
        if (!ModService.HasMod(ModIds.CURSED)) return;
        
        BotOwner owner = __instance.BotOwner_0;
        
        owner.PriorityAxeTarget.AllPursuit = true;
        
        Class105 persuitLayer = new(owner, 45);
        __instance.BaseBrain.method_0(99, persuitLayer, true);
    }
}