using KingOfTarkov.Models.Database;
using MonoMod.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
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
    DatabaseServer databaseServer,
    ISptLogger<DataService> logger)
{
    public TrialData TrialConfig;
    public Dictionary<MongoId, ModifierData> Mods;
    public Dictionary<MongoId, Quest> ReplaceableQuests = new();
    public Dictionary<string, List<BossLocationSpawn>> CustomBossSpawns = new();
    public Dictionary<string, IEnumerable<Buff>> CustomBuffs = new();
    
    private CustomQuestData _customQuestData;

    public async Task Load()
    {
        string dataPath = Path.Join(config.ModPath, "Assets", "Database");
        
        TrialData? data = await jsonUtil.DeserializeFromFileAsync<TrialData>(Path.Join(dataPath, "trials.jsonc"));

        TrialConfig = data ?? throw new Exception("[KoT] TrialConfig data not found.");
        
        Dictionary<MongoId, ModifierData>? modData = 
            await jsonUtil.DeserializeFromFileAsync<Dictionary<MongoId, ModifierData>>(Path.Join(dataPath, "modifiers.json"));
        
        Mods = modData ?? throw new Exception("[KoT] Mod data not found.");
        
        //custom profiles
        Dictionary<string, ProfileSides> customProfiles =
            await jsonUtil.DeserializeFromFileAsync<Dictionary<string, ProfileSides>>(Path.Join(dataPath, "profiles.json")) ?? [];
        
        foreach ((string name, ProfileSides profile) in customProfiles)    
        {
            databaseService.GetProfileTemplates().Add(name, profile);
        }
        
        //quest data
        _customQuestData = await jsonUtil.DeserializeFromFileAsync<CustomQuestData>(Path.Join(dataPath, "questdata.json")) 
                           ?? throw new Exception("[KoT] Quest data not found.");
        
        SetupCustomRepeatables();
        
        ReplaceableQuests = await jsonUtil.DeserializeFromFileAsync<Dictionary<MongoId, Quest>>(Path.Join(dataPath, "quests_dynamic.json")) ?? [];

        if (ReplaceableQuests.Count == 0)
        {
            logger.Warning("[KoT] There are no dynamic quests available.");
        }
        
        CustomBossSpawns = await jsonUtil.DeserializeFromFileAsync<Dictionary<string, List<BossLocationSpawn>>>(Path.Join(dataPath, "location_bosses.json")) ?? throw new Exception("[KoT] CustomBossSpawns data not found.");

        CustomBuffs =
            await jsonUtil.DeserializeFromFileAsync<Dictionary<string, IEnumerable<Buff>>>(Path.Join(dataPath,
                "buffs.json")) ?? throw new Exception("[KoT] CustomBuffs data not found.");
        
        databaseServer.GetTables().Globals.Configuration
            .Health.Effects.Stimulator.Buffs.AddRange(CustomBuffs);

        var buffs = databaseServer.GetTables().Globals.Configuration
            .Health.Effects.Stimulator.Buffs;
        
        logger.Info("[KoT] Finished loading data.");
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