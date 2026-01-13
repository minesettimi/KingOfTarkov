using KingOfTarkov.Models.Response;
using KingOfTarkov.Models.Save;
using KingOfTarkov.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;

namespace KingOfTarkov.Controllers;

[Injectable]
public class TrialController(SaveService saveService, TrialService trialService)
{
    public TrialDataResponse GetTrialData()
    {
        Dictionary<MongoId, List<MongoId>> editedData = new();

        foreach ((MongoId key, LocationDataState val) in saveService.CurrentSave.Location.Active)
        {
            editedData.Add(key, val.Mods);
        }

        TrialState currentState = saveService.CurrentSave.Trial;

        return new TrialDataResponse
        {
            Trial = currentState,
            Color = trialService.TrialConfig.Types[currentState.TrialType].Color, //messy but whatever
            Location = editedData
        };
    }
}