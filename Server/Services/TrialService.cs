using KingOfTarkov.Generators;
using KingOfTarkov.Helpers;
using KingOfTarkov.Models.Save;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Utils;

namespace KingOfTarkov.Services;

[Injectable(InjectionType.Singleton)]
public class TrialService(KingProfileHelper profileHelper, 
    TrialGenerator trialGenerator,
    SaveService save,
    QuestGenerator questGenerator,
    LocationHelper locationHelper,
    LocationService locationService,
    ISptLogger<TrialService> logger)
{
    public Task Load()
    {
        if (save.NewTrial)
        {
            StartNewTrial();
            save.NewTrial = false;
            save.SaveCurrentState();
        }
        
        return Task.CompletedTask;
    }

    public void CompleteLocation(MongoId locationId)
    {
        //TODO: FIKA SUPPORT

        LocationDataState currentLocation = save.CurrentSave.Location.Active[locationId];
        if (!currentLocation.Completed)
        {
            currentLocation.Completed = true;
            save.RemainingRaids--;

            //delete exfil quests
            foreach (MongoId questId in currentLocation.ExfilRequirements)
            {
                save.CurrentSave.Quests.Exfil.Remove(questId);
            }
        }

        //mark trial as different
        save.CurrentSave.Id = new MongoId();

        switch (save.RemainingRaids)
        {
            //trial complete
            case <= 0:
                StartNewTrial();
                break;
            case 1:
                SetupTrialFinale();
                break;
        }
        
        locationService.SetupNewLocations();
        save.SaveCurrentState();
    }
    
    public void StartNewTrial()
    {
        SaveState currentSave = save.CurrentSave;
        
        int newTrialNum = currentSave.Trial.TrialNum + 1;
        
        currentSave.Trial = trialGenerator.GenerateTrial(newTrialNum);

        LocationState locationState = currentSave.Location;
        
        LocationState newState = trialGenerator.GenerateLocationState(newTrialNum, currentSave.Trial, locationState.Previous);
        newState.Previous.AddRange(locationState.Active.Keys);
        save.RemainingRaids = newState.Active.Count;
        
        currentSave.Location = newState;
        
        profileHelper.SetupTrialForProfiles(newTrialNum == 1);
        GenerateExfilQuests();
        
        logger.Info($"[KoT] Started new Trial {newTrialNum}");
    }

    public void GenerateExfilQuests()
    {
        Dictionary<MongoId, Quest> newQuestList = [];
        foreach ((MongoId id, LocationDataState data) in save.CurrentSave.Location.Active)
        {
            Quest? newQuest = questGenerator.GenerateEliminationExfilQuest(id);

            if (newQuest is null)
                continue;
            
            newQuestList.Add(newQuest.Id, newQuest);
            data.ExfilRequirements.Add(newQuest.Id);
        }
        
        save.CurrentSave.Quests.Exfil = newQuestList;
    }
    
    public void SetupTrialFinale()
    {
        //find the final location
        KeyValuePair<MongoId, LocationDataState>? finalLocation = locationHelper.GetLastLocation();

        if (finalLocation == null)
        {
            logger.Error("[KoT] Setting up final location");
            return;
        }
        
        KeyValuePair<string, Quest>? bossQuest = questGenerator.GenerateBossExfilQuest(finalLocation.Value.Key);

        if (bossQuest == null)
        {
            logger.Error($"[KoT] Failed to create boss quest for location {finalLocation.Value.Key}");
            return;
        }
        
        KeyValuePair<string, Quest> result = bossQuest.Value;
        
        finalLocation.Value.Value.Boss = result.Key;
        
        save.CurrentSave.Quests.Exfil.Add(result.Value.Id, result.Value);
        finalLocation.Value.Value.ExfilRequirements.Add(result.Value.Id);
    }
}