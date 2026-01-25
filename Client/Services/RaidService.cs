using System.Collections.Generic;
using EFT;
using EFT.Communications;

namespace KoTClient.Services;

public class RaidService
{
    public MongoID? CurrentLocation;
    public List<MongoID> ExfilQuests = [];

    public void SetupRaidStart(MongoID locationId)
    {
        CurrentLocation = locationId;
        ExfilQuests.Clear();

        //avoid copying the ref from the data
        foreach (MongoID quest in Plugin.StateService.StateData.location[locationId].exfilRequirements)
        {
            ExfilQuests.Add(quest);
        }
        
        Plugin.PluginLogger.LogInfo($"[KoT] Raid started on location: {locationId}");
    }

    public void ExfilQuestCompleted(QuestClass quest)
    {
        ExfilQuests.Remove(quest.Id);
        if (ExfilQuests.Count == 0)
        {
            ExfiltrationControllerClass.Instance.EnableExitsInteraction();
            NotificationManagerClass.DisplayMessageNotification("ExfilCompleted".Localized(), ENotificationDurationType.Long);
        }
        
        //TODO: Update UI
    }
}