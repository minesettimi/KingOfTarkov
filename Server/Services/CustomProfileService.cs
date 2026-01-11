using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using Path = System.IO.Path;

namespace KingOfTarkov.Services;

[Injectable(InjectionType.Singleton)]
public class CustomProfileService(ConfigService config,
    JsonUtil jsonUtil,
    DatabaseService databaseService)
{
    public async Task Load()
    {
        string profilePath = Path.Join(config.ModPath, "Assets", "Database", "profiles.json");

        Dictionary<string, ProfileSides> customProfiles =
            await jsonUtil.DeserializeFromFileAsync<Dictionary<string, ProfileSides>>(profilePath) ?? [];
        
        foreach ((string name, ProfileSides profile) in customProfiles)    
        {
            databaseService.GetProfileTemplates().Add(name, profile);
        }
    }
    
    //TODO: Give players the EOD crown after winning
}