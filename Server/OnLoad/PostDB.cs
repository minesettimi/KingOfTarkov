using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;

namespace KingOfTarkov.OnLoad;

[Injectable(TypePriority = OnLoadOrder.PreSptModLoader)]
public class PostDB
{
    
}