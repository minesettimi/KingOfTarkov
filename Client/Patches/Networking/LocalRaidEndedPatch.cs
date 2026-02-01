using System.Reflection;
using EFT;
using HarmonyLib;
using KoTClient.Models;
using Newtonsoft.Json;
using SPT.Common.Http;
using SPT.Reflection.Patching;

namespace KoTClient.Patches.Networking;

public class LocalRaidEndedPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(Class308), nameof(Class308.LocalRaidEnded));
    }

    [PatchPrefix]
    public static void Prefix(LocalRaidSettings settings, RaidEndDescriptorClass results)
    {
        MatchEndRequest requestData = new()
        {
            LocationName = settings.location,
            Survived = results.result == ExitStatus.Survived,
            TrialId = Plugin.StateService.StateData!.Id
        };
        
        RequestHandler.PutJson("/kot/match/end", JsonConvert.SerializeObject(requestData));
    }
}