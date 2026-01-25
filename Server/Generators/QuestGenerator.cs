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
    ConfigService config,
    ISptLogger<QuestGenerator> logger)
{
    //mainly want to ban partisan because he doesn't spawn like regular bosses
    private readonly List<string> _bannedBosses =
    [
        "bossPartisan"
    ];
    
    public Quest? GenerateEliminationExfilQuest(MongoId locationId)
    {
        MongoId templateId = dataService.TrialConfig.Quests.Exfil.Elimination;
        
        Quest? baseQuest = GenerateExfilQuest(templateId, locationId);

        if (baseQuest == null)
        {
            logger.Error("[KoT] Failed to generate exfil elimination quest.");
            return null;
        }

        //TODO: More interesting elimination system
        int eliminationCount = 4 + saveService.CurrentSave.Trial.TrialNum;
        
        QuestCondition elimCondition = baseQuest.Conditions.AvailableForFinish![0];
        elimCondition.Value = eliminationCount;
        
        return baseQuest;
    }
    
    public KeyValuePair<string, Quest>? GenerateBossExfilQuest(MongoId locationId)
    {
        MongoId templateId = dataService.TrialConfig.Quests.Exfil.Boss;
        
        Quest? baseQuest = GenerateExfilQuest(templateId, locationId);

        if (baseQuest == null)
        {
            logger.Error("[KoT] Failed to generate exfil boss elimination quest.");
            return null;
        }
        
        //get the boss we want to task

        List<string> locationBosses = locationService.BossCache[locationId];
        
        List<string> possibleBosses = randomUtil.DrawRandomFromList(locationBosses.Except(_bannedBosses).ToList());
        string selectedBoss = possibleBosses[0];
        
        QuestConditionCounterCondition elimCondition = baseQuest.Conditions.AvailableForFinish![0].Counter!.Conditions![0];
        elimCondition.SavageRole!.Add(selectedBoss);

        return new KeyValuePair<string, Quest>(selectedBoss, baseQuest);
    }

    public Quest? GenerateReviveQuest()
    {
        MongoId templateId = dataService.TrialConfig.Quests.Exfil.Revive;

        Quest? baseQuest = GenerateQuest(templateId);

        if (baseQuest == null)
        {
            logger.Error("[KoT] Failed to generate revive quest.");
            return null;
        }

        //set currency
        QuestCondition currencyCondition = baseQuest.Conditions.AvailableForFinish![0];
        currencyCondition.Value = config.Difficulty.Core.ReviveCost;

        return baseQuest;
    }
    
    private Quest? GenerateExfilQuest(MongoId template, MongoId locationId)
    {
        Quest? newQuest = GenerateQuest(template);
        
        if (newQuest == null)
        {
            logger.Error($"[KoT] Dynamic quest template: {template} does not exist.");
            return null;
        }
        newQuest.Location = locationId;
        newQuest.Name = newQuest.Name.Replace("{locationId}", locationId);
        
        //add location corrections
        string locationName = locationUtil.GetMapKey(locationId);
            
        QuestCondition finishCondition = newQuest.Conditions.AvailableForFinish![0];

        List<string> locationNames = [locationName];
        
        //add alt maps
        if (locationName == "factory4_day")
            locationNames.Add("factory4_night");
        else if (locationName == "Sandbox_high")
            locationNames.Add("Sandbox_high");
        
        finishCondition.Counter!.Conditions!.Add(new QuestConditionCounterCondition()
        {
            Id = new MongoId(),
            DynamicLocale = true,
            Target = new ListOrT<string>(locationNames, null),
            ConditionType = "Location"
        });
        
        return newQuest;
    }

    public Quest? GenerateQuest(MongoId template)
    {
        Quest? newQuest = cloner.Clone(dataService.ReplaceableQuests[template]);
        
        if (newQuest == null)
        {
            logger.Error($"[KoT] Dynamic quest template: {template} does not exist.");
            return null;
        }
        
        //correct up the data
        newQuest.Id = new MongoId();
        
        QuestCondition finishCondition = newQuest.Conditions.AvailableForFinish![0];
        finishCondition.Id = new MongoId();
        finishCondition.Counter!.Id = new MongoId();

        return newQuest;
    }
}