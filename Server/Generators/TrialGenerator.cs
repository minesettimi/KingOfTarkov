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
    public void GenerateTrial(SaveState currentSave)
    {
        TrialState trialSave = currentSave.Trial;
        TrialData trialConfig = trialService.TrialConfig;
        int trialNum = ++trialSave.TrialNum;

        List<MongoId> typePool;
        TrialNumData currentTrial = trialConfig.Trials[trialNum];

        if ((currentTrial.TypeWhitelist?.Count ?? 0) > 0)
        {
            typePool = currentTrial.TypeWhitelist!;
        }
        else
        {
            typePool = trialConfig.Types.Where(type => type.Value.Min == trialNum)
                .Select(p => p.Key).ToList();
        }

        trialSave.trialId = new MongoId();

        MongoId typeId = randomUtil.DrawRandomFromList(typePool)[0];
        trialSave.TrialType = typeId;
        
        TrialTypeData selectedType = trialService.TrialConfig.Types[typeId];

        trialSave.mods = GenerateMods(selectedType, currentTrial.GlobalModCount, []);

        LocationState locationState = currentSave.Location;
        
        locationState.Previous.Clear();
        
        //backup mods
        locationState.Previous.AddRange(locationState.Active.Keys);
        
        locationState.Active.Clear();

        List<MongoId> locations = GenerateLocations(trialNum, currentTrial.LocationCount, currentSave);
        foreach (MongoId id in locations)
        {
            locationState.Active.Add(id, new LocationDataState
            {
                Mods = GenerateMods(selectedType, currentTrial.LocationModCount, trialSave.mods)
            });
        }
        
        logger.Info($"[KoT] Generated new Trial {trialNum}");
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

    private List<MongoId> GenerateLocations(int trialNum, int locationCount, SaveState currentSave)
    {
        List<MongoId> originalPool = [];

        foreach ((MongoId id, LocationData data) in trialService.TrialConfig.Locations)
        {
            if (data.Max < trialNum || data.Min > trialNum)
                continue;
            
            originalPool.Add(id);
        }

        List<MongoId> finalPool = originalPool.Except(currentSave.Location.Previous).ToList();

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