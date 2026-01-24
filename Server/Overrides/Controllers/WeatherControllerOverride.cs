using System.Reflection;
using KoTServer.Controllers;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Controllers;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Spt.Weather;

namespace KingOfTarkov.Overrides.Controllers;

public class WeatherControllerOverride : AbstractPatch
{
    private static KingWeatherController _weatherController;
    
    protected override MethodBase? GetTargetMethod()
    {
        _weatherController = ServiceLocator.ServiceProvider.GetService<KingWeatherController>();
        return typeof(WeatherController).GetMethod(nameof(WeatherController.GenerateLocal));
    }

    [PatchPostfix]
    public static GetLocalWeatherResponseData Postfix(MongoId sessionId, GetLocalWeatherResponseData __result)
    {
        return _weatherController.HandleWeatherChanges(__result, sessionId);
    }
}