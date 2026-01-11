using System.Reflection;
using KingOfTarkov.Services;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Controllers;
using SPTarkov.Server.Core.DI;

namespace KingOfTarkov.Overrides.Controllers;

public class CanProfileAccessRepeatableOverride : AbstractPatch
{
    private static ConfigService _configService;
    
    protected override MethodBase? GetTargetMethod()
    {
        _configService = ServiceLocator.ServiceProvider.GetRequiredService<ConfigService>();
        return typeof(RepeatableQuestController).GetMethod("CanProfileAccessRepeatableQuests",
            BindingFlags.Instance | BindingFlags.NonPublic);
    }

    [PatchPrefix]
    public static bool Prefix(ref bool __result)
    {
        if (!_configService.KingConfig.DisableRepeatable)
            return true;

        __result = false;
        
        return false;
    }
}