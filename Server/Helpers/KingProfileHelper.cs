using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Servers;

namespace KingOfTarkov.Helpers;

[Injectable(InjectionType.Singleton)]
public class KingProfileHelper(SaveServer saveServer)
{
    public void ResetAllRepeatables()
    {
        foreach ((MongoId session, SptProfile profile) in saveServer.GetProfiles())
        {
            if (saveServer.IsProfileInvalidOrUnloadable(session))
                continue;

            foreach (PmcDataRepeatableQuest quest in profile.CharacterData.PmcData.RepeatableQuests)
            {
                quest.EndTime = 0;
            }
        }
    }
}