using KingOfTarkov.Models.Database;
using KingOfTarkov.Models.Save;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Routers;
using SPTarkov.Server.Core.Utils;

namespace KingOfTarkov.Services;

[Injectable(InjectionType.Singleton)]
public class ModifierService(ConfigService configService,
    SaveService save,
    ImageRouter imageRouter,
    ISptLogger<ModifierService> logger)
{
    public Task Load()
    {
        string modImagePath = Path.Join(configService.ModPath, "Assets", "Images", "Mods");

        IEnumerable<string> imageFiles = Directory.EnumerateFiles(modImagePath, "*", SearchOption.TopDirectoryOnly);
        foreach (string imagePath in imageFiles)
        {
            string imageName = Path.GetFileNameWithoutExtension(imagePath);
            
            imageRouter.AddRoute($"/files/modifiers/icon/{imageName}", imagePath);
        }

        return Task.CompletedTask;
    }

    public bool HasMod(MongoId modId, MongoId locationId)
    {
        //first check global then local
        bool global = save.CurrentSave.Trial.Mods.Contains(modId);

        if (global)
            return true;

        LocationDataState? retrievedState = save.CurrentSave.Location.Active.GetValueOrDefault(locationId);

        if (retrievedState == null)
        {
            logger.Error($"[KoT] Attepted to retrieve modifier: {modId} from invalid location: {locationId}");
            return false;
        }

        return retrievedState.Mods.Contains(modId);
    }
}