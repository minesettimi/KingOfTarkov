using System.Collections.Generic;
using EFT;
using EFT.Communications;
using EFT.Interactive;
using EFT.Interactive.SecretExfiltrations;
using EFT.UI;

namespace KoTClient.Services;

public class RaidService
{
    public MongoID? CurrentLocation;
    public List<MongoID> ExfilQuests = [];
    public List<MongoID> CompletedQuests = [];

    public void SetupRaidStart(MongoID locationId)
    {
        CurrentLocation = locationId;
        ExfilQuests.Clear();
        CompletedQuests.Clear();

        //avoid copying the ref from the data
        foreach (MongoID quest in Plugin.StateService.StateData.location[locationId].exfilRequirements)
        {
            ExfilQuests.Add(quest);
        }
        
        Plugin.PluginLogger.LogInfo($"[KoT] Raid started on location: {locationId}");
    }

    public void ExfilQuestCompleted(QuestClass quest)
    {
        if (CompletedQuests.Contains(quest.Id))
            return;
        
        CompletedQuests.Add(quest.Id);
        if (ExfilQuests.Count == CompletedQuests.Count)
        {
            ExfiltrationControllerClass.Instance.EnableExitsInteraction();
            
            //enable secret exits
            // foreach (SecretExfiltrationPoint exfilPoint in ExfiltrationControllerClass.Instance.SecretExfiltrationPoints)
            // {
            //     exfilPoint.EnableInteraction();
            // }
            
            NotificationManagerClass.DisplayNotification(new GClass2555
            {
                Text = "ExfilCompleted".Localized(),
                Duration = ENotificationDurationType.Long,
                ShowImmediately = true,
                SoundType = EUISoundType.QuestCompleted
            });
        }
        
        //TODO: Update UI
    }
}