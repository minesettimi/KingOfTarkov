using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;

namespace KingOfTarkov.OnLoad;

[Injectable(TypePriority = OnLoadOrder.PostSptModLoader + 1)]
public class PostLoad
{
    
}