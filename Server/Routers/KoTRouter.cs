using KingOfTarkov.Controllers;
using KingOfTarkov.Helpers;
using KingOfTarkov.Models.Response;
using KingOfTarkov.Services;
using KoTServer.Controllers;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Utils;

namespace KingOfTarkov.Routers;

[Injectable]
public class KoTRouter(JsonUtil jsonUtil, KoTCallbacks koTCallbacks) : StaticRouter(jsonUtil, [
    new RouteAction<EmptyRequestData>(
        "/kot/state/data",
        async (
            url,
            info,
            sessionId,
            output
        ) => await koTCallbacks.HandleGetState()
    ),
    new RouteAction<EmptyRequestData>(
        "/kot/profile/data",
        async (
            url,
            info,
            sessionId,
            output
        ) => await koTCallbacks.HandleGetProfile(sessionId)
    ),
    new RouteAction<EmptyRequestData>(
        "/kot/profile/locked",
        async (
            url,
            info,
            sessionId,
            output
        ) => await koTCallbacks.HandleProfileLocked()
    ),
    new RouteAction<EmptyRequestData>(
        "/kot/modifier/list",
        async (
            url,
            info,
            sessionId,
            output
        ) => await koTCallbacks.HandleModifierList()
    ),
    new RouteAction<MatchEndRequest>(
        "/kot/match/end",
        async (
            url,
            info,
            sessionId,
            output
        ) => await koTCallbacks.HandleMatchEnd(info, sessionId)
    )
])
{ }

[Injectable]
public class KoTCallbacks(HttpResponseUtil httpResponseUtil,
    ProfileController profileController,
    SaveService save,
    DataService dataService,
    LocationController locationController,
    TrialController trialController)
{
    public ValueTask<string> HandleGetState()
    {
        return new ValueTask<string>(httpResponseUtil.NoBody(trialController.GetTrialData()));
    }

    public ValueTask<string> HandleGetProfile(MongoId sessionId)
    {
        return new ValueTask<string>(httpResponseUtil.NoBody(profileController.GetProfile(sessionId)));
    }

    public ValueTask<string> HandleProfileLocked()
    {
        return new ValueTask<string>(httpResponseUtil.NoBody(new { locked = save.CurrentSave.Profile.Locked }));
    }

    public ValueTask<string> HandleModifierList()
    {
        return new ValueTask<string>(httpResponseUtil.NoBody(dataService.Mods));
    }

    public ValueTask<string> HandleMatchEnd(MatchEndRequest info, MongoId sessionId)
    {
        locationController.HandleMatchEnd(info, sessionId);
        return new ValueTask<string>(httpResponseUtil.NullResponse());
    }
}