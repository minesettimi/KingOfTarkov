using KingOfTarkov.Models.Database;
using KingOfTarkov.Models.Save;
using KingOfTarkov.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;

namespace KingOfTarkov.Generators;

[Injectable(InjectionType.Singleton)]
public class TrialGenerator(TrialService trialService,
    RandomUtil randomUtil,
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

        MongoId typeId = randomUtil.DrawRandomFromList(typePool)[0];
        trialSave.TrialType = typeId;
        
        TrialTypeData selectedType = trialService.TrialConfig.Types[typeId];

        trialSave.mods = GenerateMods(selectedType, currentTrial.GlobalModCount, []);

        LocationState locationState = currentSave.Location;
        
        locationState.Previous.Clear();
        
        //backup mods
        locationState.Previous.AddRange(locationState.Active.Keys);
        
        locationState.Active.Clear();

        List<MongoId> locations = GenerateLocations(currentTrial.LocationCount, currentSave);
        foreach (MongoId id in locations)
        {
            locationState.Active.Add(id, new LocationDataState
            {
                Mods = GenerateMods(selectedType, currentTrial.LocationModCount, trialSave.mods)
            });
        }
        
        logger.Info($"[KoT] Generated new trial {trialNum}");
    }

    private List<MongoId> GenerateMods(TrialTypeData trialType, int number, List<MongoId> blacklist)
    {
        List<MongoId> modPool = trialType.ModPool.Except(blacklist).ToList();
        
        if (modPool.Count < number)
        {
            logger.Warning($"[KoT] Not enough modifiers available for trial {trialType.Name}, clamping.");
            number = modPool.Count;
        }
        
        return randomUtil.DrawRandomFromList(modPool, number, false);
    }

    private List<MongoId> GenerateLocations(int trialNum, SaveState currentSave)
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
        
        return randomUtil.DrawRandomFromList(finalPool, trialNum, false);
    }
}