using KingOfTarkov.Generators;
using KingOfTarkov.Helpers;
using KingOfTarkov.Models.Save;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;

namespace KingOfTarkov.Services;

[Injectable(InjectionType.Singleton)]
public class SaveService(ConfigService config,
    JsonUtil jsonUtil,
    ISptLogger<SaveService> logger)
{
    public SaveState CurrentSave;
    public int RemainingRaids = 0;
    public bool NewTrial = false;
    
    private readonly string _savePath = Path.Join(config.ModPath, "save.json");
    
    public async Task Load()
    {
        SaveState? tempSave = await jsonUtil.DeserializeFromFileAsync<SaveState>(_savePath);

        if (tempSave == null)
        {
            logger.Info("[KoT] Starting new save");
            CurrentSave = new SaveState();
            NewTrial = true;
        }
        else
        {
            CurrentSave = tempSave;
            RemainingRaids = CurrentSave.Location.Active.Count(i => !i.Value.Completed);
            logger.Info($"[KoT] Loaded save on Trial {tempSave.Trial.TrialNum}.");
        }

        SaveCurrentState();
    }
    public void SaveCurrentState()
    {
        File.WriteAllTextAsync(_savePath, jsonUtil.Serialize(CurrentSave, config.KingConfig.Developer));
    }
    
    //TODO: Save historical data for some kind of stats system
}