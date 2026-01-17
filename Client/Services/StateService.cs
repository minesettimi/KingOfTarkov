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