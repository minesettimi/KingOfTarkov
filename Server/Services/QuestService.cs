using System.Reflection;
using KingOfTarkov.Helpers;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Utils;
using Path = System.IO.Path;

namespace KingOfTarkov.Services;

[Injectable(InjectionType.Singleton)]
public class QuestService(ModHelper helper,
    JsonUtil jsonUtil,
    DatabaseServer databaseServer,
    KingQuestHelper kotQuestHelper,
    ConfigService config,
    ISptLogger<QuestService> logger)
{
    
    
    private Dictionary<MongoId, Quest> _replaceableQuests = new();
    
    public async Task Load()
    {
        string questPath = Path.Join(config.ModPath, "Assets", "Database", "Quests");

        string staticPath = Path.Join(questPath, "quests_static.json");
        string dynamicPath = Path.Join(questPath, "quests_dynamic.json");

        Dictionary<MongoId, Quest> staticQuests =
            await jsonUtil.DeserializeFromFileAsync<Dictionary<MongoId, Quest>>(staticPath) ?? [];
        
        foreach ((MongoId key, Quest value) in staticQuests)
        {
            //custom conditions
            if (value.Conditions.AvailableForFinish != null)
                kotQuestHelper.CacheTrialQuest(value);
            
            databaseServer.GetTables().Templates.Quests[key] = value;
        }
        
        _replaceableQuests = await jsonUtil.DeserializeFromFileAsync<Dictionary<MongoId, Quest>>(dynamicPath) ?? [];

        if (_replaceableQuests.Count == 0)
        {
            logger.Warning("[KoT] There are no dynamic quests available.");
        }
    }
}