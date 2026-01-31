using KingOfTarkov.Models.Database;
using MonoMod.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
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
    IReadOnlyList<SptMod> modList,
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

        TrialConfig = await jsonUtil.DeserializeFromFileAsync<TrialData>(Path.Join(dataPath, "trials.jsonc")) ??
                      throw new Exception("[KoT] TrialConfig data not found.");
        
        Mods = await jsonUtil.DeserializeFromFileAsync<Dictionary<MongoId, ModifierData>>(Path.Join(dataPath, "modifiers.json")) 
               ?? throw new Exception("[KoT] Mod data not found.");
        
        FilterModRequirements();
        
        //custom profiles
        Dictionary<string, ProfileSides> customProfiles =
            await jsonUtil.DeserializeFromFileAsync<Dictionary<string, ProfileSides>>(Path.Join(dataPath, "profiles.json")) ?? [];
        
        databaseService.GetProfileTemplates().AddRange(customProfiles);
        
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

    private void FilterModRequirements()
    {
        int modCount = Mods.Count;
        Mods = Mods.Where(p => p.Value.ModRequirement == null || 
            modList.Any(m => m.ModMetadata.ModGuid == p.Value.ModRequirement))
            .ToDictionary(p => p.Key, p => p.Value);

        int dif = modCount - Mods.Count;
        if (dif > 0)
            logger.Debug($"[KoT] Removed {dif} modifier(s) requiring non-present mods");
    }
}