using System;
using System.IO;
using System.Reflection;
using BepInEx;
using KoTClient.Bundles;
using KoTClient.Patches;
using SPT.Reflection.Patching;

namespace KoTClient
{
    
    [BepInPlugin("com.minesettimi.kingoftarkov", "King Of Tarkov", "0.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        private PatchManager _patchManager;
        private BundleLoader _loader;
        
        protected void Awake()
        {
            _patchManager = new PatchManager(this, true);
            _patchManager.EnablePatches();
            
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            Logger.LogWarning(modPath);
            
            _loader = new BundleLoader();
        }
    }
}