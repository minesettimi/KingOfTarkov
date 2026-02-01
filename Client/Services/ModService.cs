using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EFT;
using KoTClient.Models;
using Newtonsoft.Json;
using SPT.Common.Http;

namespace KoTClient.Services;

public class ModService
{
    public static readonly Dictionary<MongoID, ModifierData> ModifierCache = new();
    private Dictionary<MongoID, ModifierData> _modifierData = new();

    public void CacheModifiers(MongoID location)
    {
        ModifierCache.Clear();

        StateData currentData = Plugin.StateService.StateData!;
        
        foreach (MongoID globalMod in currentData.trial.mods)
        {
            ModifierCache.Add(globalMod, _modifierData[globalMod]);
        }

        foreach (MongoID locationId in currentData.location[location].mods)
        {
            ModifierCache.Add(locationId, _modifierData[locationId]);
        }
    }

    public Dictionary<MongoID, ModifierData> GetModifiersFromList(IEnumerable<MongoID> modIds)
    {
        Dictionary<MongoID, ModifierData> result = new();
        foreach (MongoID id in modIds)
        {
            if (!_modifierData.TryGetValue(id, out ModifierData? value))
            {
                Plugin.PluginLogger.LogError($"Failed to get modifier id: {id}");
                continue;
            }
            
            result.Add(id, value);
        }

        return result;
    }
    
    public async Task LoadModifierData()
    {
        try
        {
            string? data = await RequestHandler.GetJsonAsync("/kot/modifier/list");

            if (data == null) throw new Exception("Invalid modifier data");
            
            _modifierData = JsonConvert.DeserializeObject<Dictionary<MongoID, ModifierData>>(data)!;

            foreach ((MongoID id, ModifierData modData) in _modifierData)
            {
                modData._Id = id;
            }
        }
        catch (Exception e)
        {
            Plugin.PluginLogger.LogError($"Failed to request KoT state with error: {e.Message}");
            throw new Exception("Could not retrieve modifier data.");
            
        }
    }

    public static bool HasMod(MongoID id)
    {
        return ModifierCache.ContainsKey(id);
    }
}