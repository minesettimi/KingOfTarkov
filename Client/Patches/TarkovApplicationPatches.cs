using System.Reflection;
using System.Threading.Tasks;
using EFT;
using HarmonyLib;
using JsonType;
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
        Plugin.RaidService.SetupRaidStart(____raidSettings.SelectedLocation._Id);
    }
}