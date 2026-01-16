using System.Reflection;
using KingOfTarkov.Helpers;
using KingOfTarkov.Services;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Match;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Services;

namespace KingOfTarkov.Overrides.Services;

public class HandlePostRaidOverride : AbstractPatch
{
    private static LocationController _locationController;
    
    protected override MethodBase? GetTargetMethod()
    {
        _locationController = ServiceLocator.ServiceProvider.GetService<LocationController>();
        return typeof(LocationLifecycleService).GetMethod("HandlePostRaidPmc",
            BindingFlags.Instance | BindingFlags.NonPublic);
    }

    [PatchPostfix]
    public static void Postfix(MongoId sessionId,
        SptProfile fullServerProfile,
        PmcData scavProfile,
        bool isDead,
        bool isSurvived,
        bool isTransfer,
        EndLocalRaidRequestData request,
        string locationName)
    {
        _locationController.HandlePostRaid(sessionId, fullServerProfile, isDead, isSurvived, locationName);
    }
}