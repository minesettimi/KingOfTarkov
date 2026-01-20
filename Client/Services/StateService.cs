using System;
using System.Threading.Tasks;
using EFT.UI;
using KoTClient.Models;
using Newtonsoft.Json;
using SPT.Common.Http;
using SPT.Custom.Utils;
using UnityEngine;

namespace KoTClient.Services;

public class StateService
{
    public StateData? StateData { get; private set; }
    public PlayerData PlayerData { get; private set; }
    
    public event Action? TrialUpdate;

    public async Task<bool> RequestState()
    {
        try
        {
            string? data = await RequestHandler.GetJsonAsync("/kot/state/data");

            if (data != null)
            {
                StateData = JsonConvert.DeserializeObject<StateData>(data)!;
                TrialUpdate?.Invoke();
                
                return true;
            }

        }
        catch (Exception e)
        {
            Plugin.PluginLogger.LogError($"Failed to request KoT state with error: {e.Message}");
            throw new Exception("Could not retrieve trial data");
        }

        return false;
    }

    public async Task RequestPlayerState()
    {
        string? data = await RequestHandler.GetJsonAsync("/kot/profile/data");

        try
        {
            PlayerData = JsonConvert.DeserializeObject<PlayerData>(data)!;

            if (!PlayerData.Valid)
                throw new Exception();
        }
        catch (Exception e)
        {
            Plugin.PluginLogger.LogError($"Failed ot retrieve profile data with exception: {e.Message}");
            throw new Exception("KoTProfileInvalid".Localized());
        }
    }
}