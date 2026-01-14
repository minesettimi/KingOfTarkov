using KingOfTarkov.Overrides.Controllers;
using KingOfTarkov.Overrides.Services;
using KingOfTarkov.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.DI;

namespace KingOfTarkov.OnLoad;

[Injectable(TypePriority = OnLoadOrder.PreSptModLoader)]
public class PreLoad(ConfigService config) : IOnLoad
{
    private readonly List<AbstractPatch> _patches =
    [
        new GenerateAllOverride(),
        new CanProfileAccessRepeatableOverride(),
        new HandlePostRaidOverride()
    ];
    
    public async Task OnLoad()
    {
        await config.Load();
        
        foreach (AbstractPatch patch in _patches)
            patch.Enable();
    }
}