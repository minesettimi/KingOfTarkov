using System.Reflection;
using HarmonyLib;
using KingOfTarkov.Services;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Services;

namespace KingOfTarkov.Overrides.Services;

public class CreateProfileOverride : AbstractPatch
{
    private static ProfileService _profileService;
    
    protected override MethodBase? GetTargetMethod()
    {
        _profileService = ServiceLocator.ServiceProvider.GetRequiredService<ProfileService>();
        return typeof(CreateProfileService).Method(nameof(CreateProfileService.CreateProfile));
    }

    [PatchPostfix]
    public static async void Postfix(ValueTask<string> __result, MongoId sessionId, ProfileCreateRequestData request)
    {
        await __result;
        
        _profileService.InitializeProfile(sessionId, __result.Result);
    }
}