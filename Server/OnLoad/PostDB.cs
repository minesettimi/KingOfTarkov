using KingOfTarkov.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;

namespace KingOfTarkov.OnLoad;

//override SVM
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 8)]
public class PostDB(QuestService questService,
    LocaleService localeService, 
    ConfigService configService,
    CustomProfileService profileService): IOnLoad
{
    public async Task OnLoad()
    {
        await questService.Load();
        await localeService.Load();
        await configService.PostDBLoad();
        await profileService.Load();
    }
}