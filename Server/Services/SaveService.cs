using KingOfTarkov.Models.Save;
using SPTarkov.Server.Core.Utils;

namespace KingOfTarkov.Services;

public class SaveService(ConfigService config,
    JsonUtil jsonUtil)
{
    public SaveState CurrentSave;
    
    private readonly string _savePath = Path.Join(config.ModPath, "save.json");
    
    public async Task Load()
    {
        SaveState? tempSave = await jsonUtil.DeserializeFromFileAsync<SaveState>(_savePath);
        tempSave ??= NewSave();
        
        CurrentSave = tempSave;
    }

    private SaveState NewSave()
    {
        SaveState newSave = new();
        
        return newSave;
    }
}