using System.Reflection;
using System.Threading.Tasks;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace KoTClient.Patches.Endpoints;

public class FinishQuestPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(QuestView), nameof(QuestView.FinishQuest), []);
    }

    [PatchPostfix]
    public static async void Postfix()
    {
        await Task.Delay(300);
        
        await Plugin.StateService.RequestPlayerState();
    }
}