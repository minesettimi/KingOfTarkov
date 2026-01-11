using KingOfTarkov.Models.Save;
using KingOfTarkov.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;

namespace KingOfTarkov.Helpers;

[Injectable(InjectionType.Singleton)]
public class TrialHelper(TrialService trialService,
    SaveService save)
{
    
    //LINQ is super lengthy with dictionaries, do this the classic way
    public List<MongoId> GetActiveMaps()
    {
        List<MongoId> result = [];

        foreach ((MongoId id, LocationDataState data) in save.CurrentSave.Location.Active)
        {
            if (data.Completed) continue;
            
            result.Add(id);
        }

        return result;
    }
}