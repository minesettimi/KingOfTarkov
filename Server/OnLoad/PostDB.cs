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
    ProfileService profileService,
    TrialService trialService,
    SaveService saveService,
    ISptLogger<PostDB> logger): IOnLoad
{
    public async Task OnLoad()
    {
        await trialService.Load();
        await saveService.Load();
        await questService.Load();
        await localeService.Load();
        await configService.PostDBLoad();
        await profileService.Load();
        
        logger.Success("[KoT] Finished initial loading.");
    }
}