using KingOfTarkov.Models.Response;
using KingOfTarkov.Models.Save;
using KingOfTarkov.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;

namespace KingOfTarkov.Controllers;

[Injectable]
public class TrialController(SaveService saveService, DataService dataService)
{
    public TrialDataResponse GetTrialData()
    {
        TrialState currentState = saveService.CurrentSave.Trial;

        return new TrialDataResponse
        {
            Id = saveService.CurrentSave.Id,
            Trial = currentState,
            Color = dataService.TrialConfig.Types[currentState.TrialType].Color, //messy but whatever
            Location = saveService.CurrentSave.Location.Active
        };
    }
}