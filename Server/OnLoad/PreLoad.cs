using KingOfTarkov.Overrides.Controllers;
using KingOfTarkov.Overrides.Generators;
using KingOfTarkov.Overrides.Helpers;
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
        new HandlePostRaidOverride(),
        new GetClientQuestsOverride(),
        new CreateProfileOverride(),
        new TryGetLocationInfoOverride(),
        new WeatherControllerOverride(),
        new GetNumberOfExitsOverride(),
        new GetQuestFromDbOverride(),
        new CompleteQuestOverride(),
        new GetQuestDbRewardOverride()
    ];
    
    public async Task OnLoad()
    {
        await config.Load();
        
        foreach (AbstractPatch patch in _patches)
            patch.Enable();
    }
}