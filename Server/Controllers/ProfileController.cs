using KingOfTarkov.Models.Response;
using KingOfTarkov.Models.Save;
using KingOfTarkov.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;

namespace KoTServer.Controllers;

[Injectable]
public class ProfileController(ProfileService profileService, ConfigService configService)
{
    public ProfileDataResponse GetProfile(MongoId sessionId)
    {
        ProfileInfoState? profileState = profileService.GetProfileFromSession(sessionId);

        return new ProfileDataResponse
        {
            Lives = profileState?.Lives ?? 0,
            Revives = configService.Difficulty.Core.Revives ? profileState?.Revives ?? 0 : 0,
            Valid = profileState != null
        };
    }
}