using System.Reflection;
using KingOfTarkov.Generators;
using KingOfTarkov.Helpers;
using KingOfTarkov.Models.Database;
using KingOfTarkov.Models.Save;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
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
public class QuestService(
    JsonUtil jsonUtil,
    DatabaseServer databaseServer,
    DatabaseService databaseService,
    ConfigService config,
    ISptLogger<QuestService> logger)
{

    public async Task Load()
    {
        //do it before so only custom quests can start
        DisableQuests();

        string questPath = Path.Join(config.ModPath, "Assets", "Database");

        string staticPath = Path.Join(questPath, "quests_static.json");

        Dictionary<MongoId, Quest> staticQuests =
            await jsonUtil.DeserializeFromFileAsync<Dictionary<MongoId, Quest>>(staticPath) ?? [];

        foreach ((MongoId key, Quest value) in staticQuests)
        {
            databaseServer.GetTables().Templates.Quests[key] = value;
        }
    }

    //disable vanilla default quests
    private void DisableQuests()
    {
        //get all quests with no conditions
        List<Quest> quests = databaseService.GetQuests().Values
            .Where(quest => quest.Conditions.AvailableForStart?.Count == 0).ToList();

        foreach (Quest quest in quests)
        {
            quest.Conditions.AvailableForStart ??= [];

            //create an unobtainable condition
            quest.Conditions.AvailableForStart.Add(new QuestCondition
            {
                Id = new MongoId(),
                DynamicLocale = false,
                ConditionType = "Level",
                CompareMethod = ">=",
                Value = 999
            });
        }
    }
}