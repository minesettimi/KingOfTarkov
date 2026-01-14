using KingOfTarkov.Controllers;
using KingOfTarkov.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Utils;

namespace KingOfTarkov.Routers;

[Injectable]
public class TrialRouter(JsonUtil jsonUtil, TrialCallbacks trialCallbacks) : StaticRouter(jsonUtil, [
    new RouteAction<EmptyRequestData>(
        "/kot/state/id",
        async (
            url,
            info,
            sessionId,
            output
        ) => await trialCallbacks.HandleGetId()
    ),
    new RouteAction<EmptyRequestData>(
        "/kot/state/data",
        async (
            url,
            info,
            sessionId,
            output
        ) => await trialCallbacks.HandleGetData()
    )
])
{ }

[Injectable]
public class TrialCallbacks(HttpResponseUtil httpResponseUtil,
    SaveService saveService,
    TrialController trialController)
{
    public ValueTask<string> HandleGetId()
    {
        MongoId trialId = saveService.CurrentSave.Trial.TrialId;
        return new ValueTask<string>(httpResponseUtil.GetBody(trialId));
    }

    public ValueTask<string> HandleGetData()
    {
        return new ValueTask<string>(httpResponseUtil.NoBody(trialController.GetTrialData()));
    }
}