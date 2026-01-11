using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Servers;

namespace KingOfTarkov.OnLoad;

//before the weekly spawn is setup
[Injectable(TypePriority = OnLoadOrder.GameCallbacks - 2)]
public class PreCallbacks(ConfigServer configServer) : IOnLoad
{
    public Task OnLoad()
    {
        //disable weekly bosses and custom goon system
        BotConfig botConfig = configServer.GetConfig<BotConfig>();

        botConfig.WeeklyBoss.Enabled = false;
        botConfig.GoonSpawnSystem.Enabled = false;
        
        return Task.CompletedTask;
    }
}