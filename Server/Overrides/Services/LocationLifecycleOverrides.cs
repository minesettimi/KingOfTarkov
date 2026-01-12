using System.Reflection;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Services;

namespace KingOfTarkov.Overrides.Services;

public class HandlePostRaidOverride : AbstractPatch
{
    protected override MethodBase? GetTargetMethod()
    {
        return typeof(LocationLifecycleService).GetMethod("HandlePostRaidPmc",
            BindingFlags.Instance | BindingFlags.NonPublic);
    }
    
    
}