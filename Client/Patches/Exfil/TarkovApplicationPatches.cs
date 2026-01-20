using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace KoTClient.Patches;

public class RaidCreatePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(TarkovApplication), nameof(TarkovApplication.method_49));
    }

    [PatchPrefix]
    public static void Prefix(RaidSettings ____raidSettings)
    {
        MongoID locationId = ____raidSettings.SelectedLocation._Id;
        Plugin.RaidService.SetupRaidStart(locationId);
        Plugin.ModService.CacheModifiers(locationId);
    }
}