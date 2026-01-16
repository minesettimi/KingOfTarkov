using KingOfTarkov.Models.Database;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using Path = System.IO.Path;

namespace KingOfTarkov.Services;

[Injectable(InjectionType.Singleton)]
public class DataService(ConfigService config,
    JsonUtil jsonUtil,
    ConfigServer configServer,
    DatabaseService databaseService,
    ISptLogger<DataService> logger)
{
    public TrialData TrialConfig;
    public Dictionary<MongoId, ModifierData> Mods;
    public Dictionary<MongoId, Quest> ReplaceableQuests = new();
    
    private CustomQuestData _customQuestData;

    public async Task Load()
    {
        string dataPath = Path.Join(config.ModPath, "Assets", "Database");

        string trialPath = Path.Join(dataPath, "trials.json");
        string modPath = Path.Join(dataPath, "modifiers.json");
        string profilePath = Path.Join(dataPath, "profiles.json");
        string questPath = Path.Join(dataPath, "questdata.json");
        string dynamicPath = Path.Join(dataPath, "quests_dynamic.json");
        
        TrialData? data = await jsonUtil.DeserializeFromFileAsync<TrialData>(trialPath);

        TrialConfig = data ?? throw new Exception("[KoT] TrialConfig data not found.");
        
        Dictionary<MongoId, ModifierData>? modData = 
            await jsonUtil.DeserializeFromFileAsync<Dictionary<MongoId, ModifierData>>(modPath);
        
        Mods = modData ?? throw new Exception("[KoT] Mod data not found.");
        
        //custom profiles
        Dictionary<string, ProfileSides> customProfiles =
            await jsonUtil.DeserializeFromFileAsync<Dictionary<string, ProfileSides>>(profilePath) ?? [];
        
        foreach ((string name, ProfileSides profile) in customProfiles)    
        {
            databaseService.GetProfileTemplates().Add(name, profile);
        }
        
        //quest data
        _customQuestData = await jsonUtil.DeserializeFromFileAsync<CustomQuestData>(questPath) ?? throw new Exception("[KoT] Quest data not found.");
        
        SetupCustomRepeatables();
        
        ReplaceableQuests = await jsonUtil.DeserializeFromFileAsync<Dictionary<MongoId, Quest>>(dynamicPath) ?? [];

        if (ReplaceableQuests.Count == 0)
        {
            logger.Warning("[KoT] There are no dynamic quests available.");
        }
    }
    
    private void SetupCustomRepeatables()
    {
        QuestConfig questConfig = configServer.GetConfig<QuestConfig>();
        
        //remove vanilla repeatables
        questConfig.RepeatableQuests.Clear();
        
        //add custom
        questConfig.RepeatableQuests.Add(_customQuestData.CustomRepeatable);
    }
}