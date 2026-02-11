using System.Reflection;
using System.Threading.Tasks;
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

    [PatchPostfix]
    public static async Task Postfix(Task __result, LocalRaidSettings settings, RaidEndDescriptorClass results)
    {
        await __result;
        
        MatchEndRequest requestData = new()
        {
            LocationName = settings.location,
            Survived = results.result == ExitStatus.Survived,
            TrialId = Plugin.StateService.StateData!.Id
        };
        
        await RequestHandler.PutJsonAsync("/kot/match/end", JsonConvert.SerializeObject(requestData));
    }
}