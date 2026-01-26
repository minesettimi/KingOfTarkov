using KingOfTarkov.Helpers;
using KingOfTarkov.Models.Save;
using KingOfTarkov.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Utils;

namespace KoTServer.Controllers;

[Injectable(InjectionType.Singleton)]
public class KingQuestController(KingQuestHelper questHelper,
    SaveService saveService,
    ConfigService configService,
    ISptLogger<KingQuestController> logger)
{
    public void HandleQuest(Quest dynamicQuest, PmcData pmcData)
    {
        if (dynamicQuest.QuestName == "RevivePayment")
        {
            HandleReviveQuest(dynamicQuest, pmcData);
            return;
        }
        
        logger.Error($"[KoT] Unhandled dynamic quest: {dynamicQuest.QuestName}");
    }

    private void HandleReviveQuest(Quest quest, PmcData pmcData)
    {
        saveService.CurrentSave.Profile.Profiles.TryGetValue(pmcData.Id!.Value, out ProfileInfoState? profileInfo);

        if (profileInfo == null)
        {
            logger.Info($"[KoT] Couldn't get profile for dynamic quest with user: {pmcData.Info!.Nickname}");
            return;
        }

        saveService.CurrentSave.Quests.Personal.Remove(quest.Id);
        profileInfo.Quests.Remove(quest.Id);

        profileInfo.Lives = configService.Difficulty.Core.Lives;
        
        saveService.SaveCurrentState();
    }
}