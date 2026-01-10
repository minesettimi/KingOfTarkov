using KingOfTarkov.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;

namespace KingOfTarkov.OnLoad;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 6)]
public class PostDB(QuestService questService, LocaleService localeService): IOnLoad
{
    public async Task OnLoad()
    {
        await questService.Load();
        await localeService.Load();
    }
}