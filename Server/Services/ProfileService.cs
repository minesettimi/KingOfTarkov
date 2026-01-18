using KingOfTarkov.Models.Save;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;

namespace KingOfTarkov.Services;

[Injectable(InjectionType.Singleton)]
public class ProfileService(SaveServer saveServer,
    ISptLogger<ProfileService> logger,
    SaveService save)
{
    public void InitializeProfile(MongoId sessionId, MongoId id)
    {
        if (save.CurrentSave.Profile.Locked)
        {
            logger.Info("[KoT] Player tried making profile while profiles are locked!");
            return;
        }
        
        SptProfile profile = saveServer.GetProfile(sessionId);

        if (profile.ProfileInfo!.Edition != "KingOfTarkov")
        {
            logger.Warning("[KoT] Player created non-KingOfTarkov profile! There may be issues!");
        }
        
        save.CurrentSave.Profile.Profiles.Add(id, new ProfileInfoState()
        {
            Lives = 3
        });
        
        //TODO: FIKA support
        save.CurrentSave.Profile.Locked = true;
        
        save.SaveCurrentState();
    }

    public ProfileInfoState? GetProfileFromSession(MongoId sessionId)
    {
        SptProfile profile = saveServer.GetProfile(sessionId);

        PmcData? pmcData = profile.CharacterData?.PmcData;

        save.CurrentSave.Profile.Profiles.TryGetValue(pmcData.Id.Value, out ProfileInfoState? state);

        return state;
    }
}