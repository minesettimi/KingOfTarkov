using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using KoTClient.Bundles;
using KoTClient.Patches;
using KoTClient.Services;
using SPT.Reflection.Patching;

namespace KoTClient
{
    
    [BepInPlugin("com.minesettimi.kingoftarkov", "King Of Tarkov", "0.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        public static BundleLoader BundleLoader;
        public static ManualLogSource PluginLogger;   
        public static StateService StateService;
        public static RaidService RaidService;
        public static ModService ModService;
        
        private PatchManager _patchManager;
        
        protected void Awake()
        {
            PluginLogger = Logger;
            
            _patchManager = new PatchManager(this, true);
            _patchManager.EnablePatches();
            
            BundleLoader = new BundleLoader();
            StateService = new StateService();
            RaidService = new RaidService();
            ModService = new ModService();
        }
    }
}