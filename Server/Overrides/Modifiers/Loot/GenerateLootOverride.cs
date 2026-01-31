using System.Reflection;
using KingOfTarkov.Models;
using KingOfTarkov.Services;
using KingOfTarkov.Utils;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Generators;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;

namespace KingOfTarkov.Overrides.Modifiers.Loot;

public class GenerateLootOverride : AbstractPatch
{
    private static ModifierService _modifierService;
    private static LocationUtil _locationUtils;
    
    protected override MethodBase? GetTargetMethod()
    {
        _modifierService = ServiceLocator.ServiceProvider.GetService<ModifierService>();
        _locationUtils = ServiceLocator.ServiceProvider.GetService<LocationUtil>();
        return typeof(LocationLootGenerator).GetMethod(nameof(LocationLootGenerator.GenerateLocationLoot));
    }

    [PatchPrefix]
    public static bool Prefix(string locationId, ref List<SpawnpointTemplate> __result)
    {
        MongoId location = _locationUtils.GetMapId(locationId);

        if (!_modifierService.HasMod(ModIds.PICKED_DRY, location))
            return true;
        
        List<SpawnpointTemplate> result = [];
        __result = result;

        return false;
    }
}