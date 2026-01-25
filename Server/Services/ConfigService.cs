using System.Reflection;
using KingOfTarkov.Models.Config;
using KingOfTarkov.Models.Difficulty;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Utils;
using BaseConfig = KingOfTarkov.Models.Config.BaseConfig;

namespace KingOfTarkov.Services;

[Injectable(InjectionType.Singleton)]
public class ConfigService(ModHelper modHelper,
    ConfigServer configServer,
    DatabaseServer databaseServer,
    JsonUtil jsonUtil,
    ISptLogger<ConfigService> logger)
{
    public readonly string ModPath = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
    
    public BaseConfig BaseConfig;
    public BaseDifficulty BaseDifficulty;

    public async Task Load()
    {
        string configPath = Path.Join(ModPath, "config.json");
        string difficultyPath = Path.Join(ModPath, "Difficulties");
        
        BaseConfig = await jsonUtil.DeserializeFromFileAsync<BaseConfig>(configPath) ?? new BaseConfig();
        BaseDifficulty? selectedDifficulty =
            await jsonUtil.DeserializeFromFileAsync<BaseDifficulty>(Path.Join(difficultyPath,
                $"{BaseConfig.Difficulty}.jsonc"));

        if (selectedDifficulty == null)
        {
            logger.Error($"[KoT] Difficulty: \"{BaseConfig.Difficulty}.jsonc\" not found.");
            BaseDifficulty = new BaseDifficulty();
        }
        else
        {
            logger.Info($"[KoT] Initializing for {BaseConfig.Difficulty} difficulty.");
            BaseDifficulty = selectedDifficulty;
        }
        
        await File.WriteAllTextAsync(configPath, jsonUtil.Serialize(BaseConfig, true));
    }

    public async Task PostDBLoad()
    {
        EditSptConfig();
    }

    private string[] StockProfiles =
    [
        "Standard",
        "Left Behind",
        "Prepare To Escape",
        "Edge Of Darkness",
        "Unheard",
        "Tournament",
        "SPT Developer",
        "SPT Easy start",
        "SPT Zero to hero"
    ];

    private void EditSptConfig()
    {
        CoreConfig coreConfig = configServer.GetConfig<CoreConfig>();
        
        if (!BaseConfig.Developer)
        {
            //disable chat bots
            foreach (MongoId id in coreConfig.Features.ChatbotFeatures.EnabledBots.Keys)
            {
                coreConfig.Features.ChatbotFeatures.EnabledBots[id] = false;
            }
        }

        if (BaseConfig.DisableStockProfiles)
        {
            foreach (string stockProfile in StockProfiles)
            {
                coreConfig.Features.CreateNewProfileTypesBlacklist.Add(stockProfile);
            }
        }
        
        LocationConfig locationConfig = configServer.GetConfig<LocationConfig>();
        
        //increase loot multipliers
        foreach ((string key, double value) in locationConfig.LooseLootMultiplier)
        {
            locationConfig.LooseLootMultiplier[key] = value + BaseDifficulty.Location.LooseLootAdd;
        }

        foreach ((string key, double value) in locationConfig.StaticLootMultiplier)
        {
            locationConfig.StaticLootMultiplier[key] = value + BaseDifficulty.Location.StaticLootAdd;
        }
        
        TraderConfig traderConfig = configServer.GetConfig<TraderConfig>();

        //disable coop gift
        traderConfig.Fence.CoopExtractGift.SendGift = false;
        
        //disable cheese
        LostOnDeathConfig lostOnDeathConfig = configServer.GetConfig<LostOnDeathConfig>();

        lostOnDeathConfig.WipeOnRaidStart = !BaseConfig.Developer;
        
        //-----------------
        //database settings
        //-----------------
        
        Globals globals = databaseServer.GetTables().Globals;
        
        //disable transits
        globals.Configuration.TransitSettings.Active = false;
        
        //disable scav
        globals.Configuration.SavagePlayCooldown = 999999999;
        globals.Configuration.SavagePlayCooldownNdaFree = 999999999;
        
        //disable traditional level up
        globals.Configuration.Exp.MatchEnd.KilledMultiplier = 0.0;
        globals.Configuration.Exp.MatchEnd.MiaMultiplier = 0.0;
        globals.Configuration.Exp.MatchEnd.RunnerMultiplier = 0.0;
        globals.Configuration.Exp.MatchEnd.SurvivedMultiplier = 0.0;

        //FiR flea
        globals.Configuration.RagFair.IsOnlyFoundInRaidAllowed = true;
    }
}