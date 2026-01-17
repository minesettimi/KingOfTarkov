using KingOfTarkov.Controllers;
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
    )
])
{ }

[Injectable]
public class KoTCallbacks(HttpResponseUtil httpResponseUtil,
    ProfileController profileController,
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
}