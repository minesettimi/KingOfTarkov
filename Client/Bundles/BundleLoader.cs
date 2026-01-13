using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace KoTClient.Bundles;

//thanks to Lacyway
public class BundleLoader
{
    public AssetBundle Bundle;
    
    public BundleLoader()
    {
        Task.Run(LoadBundle);
    }
    
    public async Task LoadBundle()
    {
        string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        
        string bundlePath = Path.Combine(modPath, "Bundles", "kotbundle.bundle");
        AssetBundleCreateRequest? bundle = AssetBundle.LoadFromFileAsync(bundlePath);

        while (!bundle.isDone)
        {
            await Task.Yield();
        }
        
        Bundle = bundle.assetBundle;
    }
}