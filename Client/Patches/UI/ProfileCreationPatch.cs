using System;
using System.Reflection;
using EFT.UI;
using HarmonyLib;
using KoTClient.Models;
using Newtonsoft.Json;
using SPT.Common.Http;
using SPT.Reflection.Patching;
using UnityEngine;

namespace KoTClient.Patches;

public class ProfileCreationPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(GClass2305), nameof(GClass2305.method_2));
    }

    [PatchPrefix]
    public static bool Prefix()
    {
        string? data = RequestHandler.GetJson("/kot/profile/locked");
        
        ProfileLockedResponse? response = JsonConvert.DeserializeObject<ProfileLockedResponse>(data);

        if (response.Locked)
        {
            MonoBehaviourSingleton<PreloaderUI>.Instance.ShowErrorScreen("PROFILE_BLOCKED",
                "KoTProfileLocked".Localized(), Application.Quit);
            throw new Exception("KotProfileLocked".Localized());
        }

        return true;
    }
}