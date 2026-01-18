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

            throw new Exception("Could not retrieve trial data");
        }
        catch (Exception e)
        {
            Plugin.PluginLogger.LogError($"Failed to request KoT state with error: {e.Message}");
            MessageBoxHelper.Show("Failed to get King Of Tarkov state.", "KOT_ERROR", MessageBoxHelper.MessageBoxType.OK);
        }

        return false;
    }

    public async Task<bool> RequestPlayerState()
    {
        string? data = await RequestHandler.GetJsonAsync("/kot/profile/data");

        try
        {
            PlayerData = JsonConvert.DeserializeObject<PlayerData>(data)!;

            if (!PlayerData.Valid)
                throw new Exception("Player is not valid.");
        }
        catch (Exception e)
        {
            Plugin.PluginLogger.LogError($"Failed ot retrieve profile data with exception: {e.Message}");
            MonoBehaviourSingleton<PreloaderUI>.Instance.ShowErrorScreen("INVALID_PROFILE",
                "KoTProfileInvalid".Localized(), Application.Quit);

            return false;
        }

        return true;
    }
}