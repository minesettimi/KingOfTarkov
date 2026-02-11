using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Utils;
using SPTarkov.Server.Core.Utils.Json;

namespace KingOfTarkov.Services;

[Injectable(InjectionType.Singleton)]
public class LocaleService(FileUtil fileUtil, 
    JsonUtil jsonUtil, 
    ConfigService config,
    DatabaseServer databaseServer,
    ISptLogger<LocaleService> logger)
{
    public async Task Load()
    {
        string localeDir = Path.Join(config.ModPath, "Assets", "Database", "Locales");

        List<string> localeFiles = fileUtil.GetFiles(localeDir, false, "*.jsonc");

        Dictionary<string, Dictionary<string, string>?> locales = new();
        
        foreach (string file in localeFiles)
        {
            string langCode = Path.GetFileNameWithoutExtension(file);

            Dictionary<string, string>? localeData = await jsonUtil.DeserializeFromFileAsync<Dictionary<string, string>>(file);

            if (localeData == null)
            {
                logger.Warning($"[KoT] Failed to load locale file {file}");
                localeData = [];
            }
            
            locales[langCode] = localeData;
        }

        foreach ((string locale, LazyLoad<Dictionary<string, string>> lazyLoadedVal) in databaseServer.GetTables().Locales.Global)
        {
            lazyLoadedVal.AddTransformer(localeData =>
            {
                if (localeData == null) 
                    return localeData;

                locales.TryGetValue(locale, out Dictionary<string, string>? customLocales);

                if (customLocales == null)
                    return localeData;

                foreach ((string key, string value) in customLocales)
                    localeData[key] = value;

                return localeData;
            });
        }
    }
}