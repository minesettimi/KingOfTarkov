using KingOfTarkov.Helpers;
using KingOfTarkov.Models;
using KingOfTarkov.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Weather;

namespace KoTServer.Controllers;

[Injectable(InjectionType.Singleton)]
public class KingWeatherController(KingProfileHelper profileHelper, KingWeatherHelper weatherHelper, ModifierService modService)
{
    public GetLocalWeatherResponseData HandleWeatherChanges(GetLocalWeatherResponseData weatherData, MongoId sessionId)
    {
        MongoId? locationId = profileHelper.GetLocationFromSession(sessionId);

        if (locationId == null || !modService.HasMod(ModIds.SUDDEN_BLIZZARD, locationId.Value))
            return weatherData;
        
        weatherData.Season = Season.WINTER;

        weatherData.Weather ??= [];
        weatherData.Weather.Clear();

        weatherData.Weather.Add(weatherHelper.GetForcedWeather(weatherHelper.Blizzard));
        
        return weatherData;
    }
}