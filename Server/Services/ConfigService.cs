using System.Reflection;
using KingOfTarkov.Models.Config;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Utils;

namespace KingOfTarkov.Services;

[Injectable(InjectionType.Singleton)]
public class ConfigService(ModHelper modHelper,
    ConfigServer configServer,
    DatabaseServer databaseServer,
    JsonUtil jsonUtil)
{
    public readonly string ModPath = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
    
    public KingConfig KingConfig;

    public async Task Load()
    {
        string configPath = Path.Join(ModPath, "config.json");
        
        KingConfig = await jsonUtil.DeserializeFromFileAsync<KingConfig>(configPath) ?? new KingConfig();
        
        await File.WriteAllTextAsync(configPath, jsonUtil.Serialize(KingConfig, true));
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
        
        if (!KingConfig.Developer)
        {
            //disable chat bots
            foreach (MongoId id in coreConfig.Features.ChatbotFeatures.EnabledBots.Keys)
            {
                coreConfig.Features.ChatbotFeatures.EnabledBots[id] = false;
            }
        }

        if (KingConfig.DisableStockProfiles)
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
            locationConfig.LooseLootMultiplier[key] = value + KingConfig.ConfigEdits.LooseLootAdd;
        }

        foreach ((string key, double value) in locationConfig.StaticLootMultiplier)
        {
            locationConfig.StaticLootMultiplier[key] = value + KingConfig.ConfigEdits.StaticLootAdd;
        }
        
        TraderConfig traderConfig = configServer.GetConfig<TraderConfig>();

        //disable coop gift
        traderConfig.Fence.CoopExtractGift.SendGift = false;
        
        //disable cheese
        LostOnDeathConfig lostOnDeathConfig = configServer.GetConfig<LostOnDeathConfig>();

        lostOnDeathConfig.WipeOnRaidStart = true;
        
        //database settings
        
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