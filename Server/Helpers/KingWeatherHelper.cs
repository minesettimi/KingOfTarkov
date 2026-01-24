using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Eft.Weather;
using SPTarkov.Server.Core.Utils;

namespace KingOfTarkov.Helpers;

[Injectable(InjectionType.Singleton)]
public class KingWeatherHelper(TimeUtil timeUtil, WeatherHelper weatherHelper)
{
    public Weather Blizzard = new()
    {
        Cloud = 4.0,
        Fog = 2.0,
        Pressure = 770,
        Temperature = -22,
        Rain = 5,
        RainIntensity = 1,
        WindGustiness = 1,
        WindSpeed = 3
    };

    //use a provided weather
    public Weather GetForcedWeather(Weather weather)
    {
        DateTime raidTime = weatherHelper.GetInRaidTime();
        string date = DateTime.UtcNow.FormatToBsgDate();
        string time = $"{date} {raidTime.GetBsgFormattedWeatherTime()}";

        weather.Timestamp = timeUtil.GetTimeStamp();
        weather.Date = date;
        weather.Time = time;

        return weather;
    }
}