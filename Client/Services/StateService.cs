using System;
using System.Threading.Tasks;
using KoTClient.Models;
using Newtonsoft.Json;
using SPT.Common.Http;
using SPT.Custom.Utils;

namespace KoTClient.Services;

public class StateService
{
    public StateData? stateData { get; private set; }
    public event Action? TrialUpdate;

    public bool IsStateOutdated()
    {
        //trial data hasn't initialized yet
        if (stateData == null)
            return true;
        
        //check if state is updated
        string? data = RequestHandler.GetJson("/kot/state/id");

        if (data == null)
        {
            Plugin.PluginLogger.LogError("[KoT] Failed to get latest state id.");
            return false;
        }
        
        IdData dataResult = JsonConvert.DeserializeObject<IdData>(data)!;
        
        return dataResult.Id != stateData.Id;
    }

    public async Task<bool> RequestState()
    {
        try
        {
            string? data = await RequestHandler.GetJsonAsync("/kot/state/data");

            if (data != null)
            {
                stateData = JsonConvert.DeserializeObject<StateData>(data)!;
                TrialUpdate?.Invoke();
                
                return true;
            }

            throw new Exception("Could not retrieve trial data");
        }
        catch (Exception e)
        {
            Plugin.PluginLogger.LogError($"Failed to request KoT state with error: {e.Message}");
            MessageBoxHelper.Show("Failed to get King Of Tarkov state.", "KOT_ERROR", MessageBoxHelper.MessageBoxType.OK);
        }

        return false;
    }
}