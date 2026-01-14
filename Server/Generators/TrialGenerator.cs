using KingOfTarkov.Models.Database;
using KingOfTarkov.Models.Save;
using KingOfTarkov.Services;
using KingOfTarkov.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;

namespace KingOfTarkov.Generators;

[Injectable(InjectionType.Singleton)]
public class TrialGenerator(TrialService trialService,
    RandomUtil randomUtil,
    KingMathUtil kingMathUtil,
    ISptLogger<TrialGenerator> logger)
{
    public TrialState GenerateTrial(int trialNum)
    {
        TrialData trialConfig = trialService.TrialConfig;

        List<MongoId> typePool;
        TrialNumData newTrialConfig = trialConfig.Trials[trialNum];

        if ((newTrialConfig.TypeWhitelist?.Count ?? 0) > 0)
        {
            typePool = newTrialConfig.TypeWhitelist!;
        }
        else
        {
            typePool = trialConfig.Types.Where(type => type.Value.Min == trialNum)
                .Select(p => p.Key).ToList();
        }


        MongoId typeId = randomUtil.DrawRandomFromList(typePool)[0];
        
        TrialTypeData selectedType = trialService.TrialConfig.Types[typeId];
        
        return new TrialState
        {
            TrialId = new MongoId(),
            TrialType = typeId,
            TrialNum = trialNum,
            Mods = GenerateMods(selectedType, newTrialConfig.GlobalModCount, [])
        };
    }

    public LocationState GenerateLocationState(int trialNum, TrialState trialSave, List<MongoId> previous)
    {
        LocationState locationState = new();

        TrialTypeData currentType = trialService.TrialConfig.Types[trialSave.TrialType];
        TrialNumData newData = trialService.TrialConfig.Trials[trialNum];

        List<MongoId> locations = GenerateLocations(trialNum, newData.LocationCount, previous);
        foreach (MongoId id in locations)
        {
            locationState.Active.Add(id, new LocationDataState
            {
                Mods = GenerateMods(currentType, newData.LocationModCount, trialSave.Mods)
            });
        }

        return locationState;
    }

    private List<MongoId> GenerateMods(TrialTypeData trialType, int number, List<MongoId> blacklist)
    {
        List<MongoId> modPool = trialType.ModPool.Except(blacklist).ToList();
        
        if (modPool.Count < number)
        {
            logger.Warning($"[KoT] Not enough modifiers available for Trial {trialType.Name}, clamping.");
            number = modPool.Count;
        }
        
        return randomUtil.DrawRandomFromList(modPool, number, false);
    }

    private List<MongoId> GenerateLocations(int trialNum, int locationCount, List<MongoId> previousLocs)
    {
        List<MongoId> originalPool = [];

        foreach ((MongoId id, LocationData data) in trialService.TrialConfig.Locations)
        {
            if (data.Max < trialNum || data.Min > trialNum)
                continue;
            
            originalPool.Add(id);
        }

        List<MongoId> finalPool = originalPool.Except(previousLocs).ToList();

        if (finalPool.Count < trialNum)
        {
            logger.Debug("[KoT] Not enough locations for an unique selection.");
            finalPool = originalPool;
        }

        double totalWeight = 0.0;
        Dictionary<MongoId, double> weights = new();
        foreach (MongoId id in finalPool)
        {
            LocationData data = trialService.TrialConfig.Locations[id];
            
            double weight = kingMathUtil.MapToRangeInv(trialNum, 0, 10, data.MinWeight, data.MaxWeight);
            totalWeight += weight;
            
            weights.Add(id, weight);
        }

        List<MongoId> results = [];
        
        //weighted generation
        for (int i = 0; i < locationCount; i++)
        {
            double randomWeight = randomUtil.GetDouble(0.0, totalWeight);

            bool generated = false;
            foreach (MongoId id in finalPool)
            {
                double weight = weights[id];
                randomWeight -= weight;

                if (randomWeight > 0.0) continue;
                
                results.Add(id);
                finalPool.Remove(id);
                totalWeight -= weight;

                generated = true;
                break;
            }

            if (!generated)
            {
                logger.Error("[KoT] Couldn't randomly pick weighted Location!");
            }
        }
        
        return results;
    }
}