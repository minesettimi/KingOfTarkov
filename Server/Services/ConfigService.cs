using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;

namespace KingOfTarkov.Services;

[Injectable(InjectionType.Singleton)]
public class ConfigService(ModHelper modHelper)
{
    public readonly string ModPath = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
}