using KingOfTarkov.Services;
using KingOfTarkov.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;
using SPTarkov.Server.Core.Utils.Cloners;
using SPTarkov.Server.Core.Utils.Json;

namespace KingOfTarkov.Generators;

[Injectable(InjectionType.Singleton)]
public class QuestGenerator(DataService dataService,
    SaveService saveService,
    ICloner cloner,
    LocationUtil locationUtil,
    LocationService locationService,
    RandomUtil randomUtil,
    ISptLogger<QuestGenerator> logger)
{
    public Quest? GenerateEliminationExfilQuest(MongoId locationId)
    {
        MongoId templateId = dataService.TrialConfig.Quests.Exfil.Elimination;
        
        Quest? baseQuest = GenerateExfilQuest(templateId, locationId);

        if (baseQuest is null)
        {
            logger.Error("[Kot] Failed to generate exfil elimination quest.");
            return null;
        }

        //TODO: More interesting elimination system
        int eliminationCount = 4 + saveService.CurrentSave.Trial.TrialNum;
        
        QuestCondition elimCondition = baseQuest.Conditions.AvailableForFinish![0];
        elimCondition.Value = eliminationCount;
        
        return baseQuest;
    }

    public Quest? GenerateBossExfilQuest(MongoId locationId)
    {
        MongoId templateId = dataService.TrialConfig.Quests.Exfil.Elimination;
        
        Quest? baseQuest = GenerateExfilQuest(templateId, locationId);

        if (baseQuest is null)
        {
            logger.Error("[Kot] Failed to generate exfil elimination quest.");
            return null;
        }
        
        //get the boss we want to task
        List<string> possibleBosses = randomUtil.DrawRandomFromList(locationService.BossCache[locationId]);
        string selectedBoss = possibleBosses[0];
        
        QuestConditionCounterCondition elimCondition = baseQuest.Conditions.AvailableForFinish![0].Counter!.Conditions![0];
        elimCondition.SavageRole!.Add(selectedBoss);

        return baseQuest;
    }
    
    public Quest? GenerateExfilQuest(MongoId template, MongoId locationId)
    {
        Quest? newQuest = cloner.Clone(dataService.ReplaceableQuests[template]);
        
        if (newQuest == null)
        {
            logger.Error($"[KoT] Dynamic quest template: {template} does not exist.");
            return null;
        }
        
        //correct up the data
        newQuest.Id = new MongoId();
        newQuest.Location = locationId;
        newQuest.Name = newQuest.Name.Replace("{locationId}", locationId);
        
        //add location corrections
        string locationName = locationUtil.GetMapKey(locationId);
            
        QuestCondition finishCondition = newQuest.Conditions.AvailableForFinish![0];
        finishCondition.Id = new MongoId();
        finishCondition.Counter!.Id = new MongoId();

        List<string> locationNames = [locationName];
        finishCondition.Counter.Conditions!.Add(new QuestConditionCounterCondition()
        {
            Id = new MongoId(),
            DynamicLocale = true,
            Target = new ListOrT<string>(locationNames, null),
            ConditionType = "Location"
        });
        
        return newQuest;
    }
}