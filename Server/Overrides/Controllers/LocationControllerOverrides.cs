using System.Reflection;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Controllers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace KingOfTarkov.Overrides.Controllers;

public class GenerateAllOverride : AbstractPatch
{
    private static int test = 0;
    
    protected override MethodBase? GetTargetMethod()
    {
        return typeof(LocationController).GetMethod(nameof(LocationController.GenerateAll));
    }

    [PatchPostfix]
    public static LocationsGenerateAllResponse GenerateAll(LocationsGenerateAllResponse __result)
    {
        test++;
        foreach ((MongoId id, LocationBase location) in __result.Locations)
        {
            location.Enabled = test % 2 == 0;
        }
        
        return __result;
    }
}