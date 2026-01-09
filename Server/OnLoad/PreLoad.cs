using KingOfTarkov.Overrides.Controllers;
using SPTarkov.DI.Annotations;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.DI;

namespace KingOfTarkov.OnLoad;

[Injectable(TypePriority = OnLoadOrder.PreSptModLoader)]
public class PreLoad : IOnLoad
{
    private readonly List<AbstractPatch> _patches =
    [
        new GenerateAllOverride()
    ];
    
    public Task OnLoad()
    {
        foreach (AbstractPatch patch in _patches)
            patch.Enable();
        
        return Task.CompletedTask;
    }
}