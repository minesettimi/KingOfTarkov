using KingOfTarkov.Models.Database;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;
using Path = System.IO.Path;

namespace KingOfTarkov.Services;

[Injectable(InjectionType.Singleton)]
public class TrialService(ConfigService config,
    JsonUtil jsonUtil,
    ISptLogger<TrialService> logger)
{
    public TrialData TrialConfig;
    public Dictionary<MongoId, ModifierData> Mods;

    public async Task Load()
    {
        string dataPath = Path.Join(config.ModPath, "Assets", "Database");

        string trialPath = Path.Join(dataPath, "trials.json");
        string modPath = Path.Join(dataPath, "modifiers.json");
        
        TrialData? data = await jsonUtil.DeserializeFromFileAsync<TrialData>(trialPath);

        TrialConfig = data ?? throw new Exception("[KoT] TrialConfig data not found.");
        
        Dictionary<MongoId, ModifierData>? modData = 
            await jsonUtil.DeserializeFromFileAsync<Dictionary<MongoId, ModifierData>>(modPath);
        
        Mods = modData ?? throw new Exception("[KoT] Mod data not found.");
    }
}