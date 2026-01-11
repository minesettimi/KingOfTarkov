using System.Reflection;
using KingOfTarkov.Helpers;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Controllers;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace KingOfTarkov.Overrides.Controllers;

public class GenerateAllOverride : AbstractPatch
{
    private static TrialHelper _trialHelper;
    
    protected override MethodBase? GetTargetMethod()
    {
        _trialHelper = ServiceLocator.ServiceProvider.GetService<TrialHelper>()!;
        return typeof(LocationController).GetMethod(nameof(LocationController.GenerateAll));
    }

    [PatchPostfix]
    public static LocationsGenerateAllResponse GenerateAll(LocationsGenerateAllResponse __result)
    {
        List<MongoId> activeMaps = _trialHelper.GetActiveMaps();

        foreach ((MongoId id, LocationBase location) in __result.Locations)
        {
            if (activeMaps.Contains(id))
                location.IsSecret = true;
            else
                location.Locked = true;
        }
        
        return __result;
    }
}