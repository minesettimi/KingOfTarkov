using KingOfTarkov.Generators;
using KingOfTarkov.Helpers;
using KingOfTarkov.Models.Save;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Utils;

namespace KingOfTarkov.Services;

[Injectable(InjectionType.Singleton)]
public class TrialService(SaveService saveService, 
    KingProfileHelper profileHelper, 
    TrialGenerator trialGenerator,
    SaveService save,
    QuestGenerator questGenerator,
    LocationHelper locationHelper,
    ISptLogger<TrialService> logger)
{
    public Task Load()
    {
        if (saveService.NewTrial)
        {
            StartNewTrial();
            saveService.NewTrial = false;
        }
        
        return Task.CompletedTask;
    }

    public void CompleteLocation(MongoId locationId)
    {
        //TODO: FIKA SUPPORT
        if (!save.CurrentSave.Location.Active[locationId].Completed)
        {
            save.CurrentSave.Location.Active[locationId].Completed = true;
            save.RemainingRaids--;
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
        
        save.SaveCurrentState();
    }
    
    public void StartNewTrial()
    {
        SaveState currentSave = saveService.CurrentSave;
        
        int newTrialNum = currentSave.Trial.TrialNum + 1;
        
        currentSave.Trial = trialGenerator.GenerateTrial(newTrialNum);

        LocationState locationState = currentSave.Location;
        
        LocationState newState = trialGenerator.GenerateLocationState(newTrialNum, currentSave.Trial, locationState.Previous);
        newState.Previous.AddRange(locationState.Active.Keys);
        saveService.RemainingRaids = newState.Active.Count;
        
        currentSave.Location = newState;
        
        profileHelper.ResetAllRepeatables();
        GenerateExfilQuests();
        
        logger.Info($"[KoT] Started new Trial {newTrialNum}");
    }

    public void GenerateExfilQuests()
    {
        Dictionary<MongoId, Quest> newQuestList = [];
        foreach ((MongoId id, LocationDataState data) in saveService.CurrentSave.Location.Active)
        {
            Quest? newQuest = questGenerator.GenerateEliminationExfilQuest(id);

            if (newQuest is null)
                continue;
            
            newQuestList.Add(newQuest.Id, newQuest);
            data.ExfilRequirements.Add(newQuest.Id);
        }
        
        saveService.CurrentSave.Quests.Exfil = newQuestList;
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
        
        Quest? bossQuest = questGenerator.GenerateBossExfilQuest(finalLocation.Value.Key);

        if (bossQuest is null)
        {
            logger.Error($"[KoT] Failed to create boss quest for location {finalLocation.Value.Key}");
            return;
        }
        
        saveService.CurrentSave.Quests.Exfil.Add(bossQuest.Id, bossQuest);
        finalLocation.Value.Value.ExfilRequirements.Add(bossQuest.Id);
    }
}