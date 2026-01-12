using KingOfTarkov.Helpers;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Spt.Server;
using SPTarkov.Server.Core.Services;

namespace KingOfTarkov.OnLoad;

[Injectable(TypePriority = OnLoadOrder.PostSptModLoader + 1)]
public class PostLoad(DatabaseService dbService) : IOnLoad
{
    public Task OnLoad()
    {
        AdjustDefaultLocations();
        return Task.CompletedTask;
    }

    private void AdjustDefaultLocations()
    {
        Locations locations = dbService.GetLocations();

        //remove labs locks
        LocationBase labsBase = locations.Laboratory.Base;
        labsBase.IsSecret = false;
        labsBase.AccessKeys = [];
        labsBase.AccessKeysPvE = [];
        
        //enable labyrinth
        LocationBase labyrinthBase = locations.Labyrinth.Base;
        labyrinthBase.Enabled = true;
        labyrinthBase.IsSecret = false;
        labyrinthBase.AccessKeys = [];
        labyrinthBase.AccessKeysPvE = [];
        
        //fix icon
        labyrinthBase.IconY = 350;
        
        //remove terminal
        locations.Terminal.Base.Enabled = false;
    }
}