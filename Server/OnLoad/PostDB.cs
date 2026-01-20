using KingOfTarkov.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Utils;

namespace KingOfTarkov.OnLoad;

//override SVM
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 8)]
public class PostDB(QuestService questService,
    LocaleService localeService, 
    ConfigService configService,
    DataService dataService,
    SaveService saveService,
    TrialService trialService,
    LocationService locationService,
    ModifierService modifierService,
    ISptLogger<PostDB> logger): IOnLoad
{
    public async Task OnLoad()
    {
        await dataService.Load();
        await saveService.Load();
        
        await questService.Load();
        
        await localeService.Load();
        
        await locationService.Load();
        await configService.PostDBLoad();
        await trialService.Load();
        
        await modifierService.Load();
        
        logger.Success("[KoT] Finished initial loading.");
    }
}