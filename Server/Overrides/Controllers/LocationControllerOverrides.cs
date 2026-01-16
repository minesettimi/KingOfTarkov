using System.Reflection;
using KingOfTarkov.Helpers;
using KingOfTarkov.Utils;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Controllers;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using LocationController = KingOfTarkov.Helpers.LocationController;

namespace KingOfTarkov.Overrides.Controllers;

public class GenerateAllOverride : AbstractPatch
{
    private static LocationController _locationController;
    
    protected override MethodBase? GetTargetMethod()
    {
        _locationController = ServiceLocator.ServiceProvider.GetService<LocationController>()!;
        return typeof(SPTarkov.Server.Core.Controllers.LocationController).GetMethod(nameof(SPTarkov.Server.Core.Controllers.LocationController.GenerateAll));
    }

    [PatchPostfix]
    public static LocationsGenerateAllResponse Postfix(LocationsGenerateAllResponse __result)
    {
        return _locationController.HandleGetLocations(__result);
    }
}