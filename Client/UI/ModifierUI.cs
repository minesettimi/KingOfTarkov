using System.Collections.Generic;
using System.Threading.Tasks;
using EFT;
using EFT.InputSystem;
using EFT.UI;
using KoTClient.Models;
using UnityEngine;

namespace KoTClient.UI;

public class ModifierUI : UIElement
{
    [SerializeField] 
    public ModifierIconUI ModifierTemplate;

    [SerializeField] 
    public RectTransform ModifierHolder;

    private Dictionary<MongoID, ModifierData> _modList;
    private List<GameObject> _modIcons;
    private ISession _session;

    public void Show(ISession session, List<MongoID> modifiers)
    {
        UI.Dispose();

        ShowGameObject();
        _session = session;
        
        if (modifiers.Count == 0)
             return;
        
        _modList = Plugin.ModService.GetModifiersFromList(modifiers);
        
        UI.AddViewList(_modList, ModifierTemplate, ModifierHolder, ShowMod);
    }

    public void ShowMod(KeyValuePair<MongoID, ModifierData> modData, ModifierIconUI modIcon)
    {
        modIcon.SetEnabledTooltip($"{modData.Key} name".Localized() +
                                  "<br><color=#535353>" + $"{modData.Key} description".Localized() + "</color>");
        
        if (modData.Value.Sprite == null)
        {
            GetModSprite(modData.Value, modIcon);
            return;
        }
        
        modIcon.ModSprite.sprite = modData.Value.Sprite;
        modIcon.ShowGameObject();
    }

    private async Task GetModSprite(ModifierData modifierData, ModifierIconUI modIcon)
    {
        await modifierData.LoadIconSprite(_session);
        
        modIcon.ModSprite.sprite = modifierData.Sprite;
        modIcon.ShowGameObject();
    }
    
    // private async Task ShowModList(ISession session)
    // {
    //     Plugin.PluginLogger.LogInfo("Test");
    //     
    //     foreach ((MongoID id, ModifierData modData) in _modList)
    //     {
    //         if (modData.Sprite == null)
    //         {
    //             await modData.LoadIconSprite(session);
    //         }
    //         
    //         GameObject newModImg = Instantiate(ModifierTemplate, transform);
    //         ModifierIconUI modIcon = newModImg.GetComponent<ModifierIconUI>();
    //
    //         modIcon.ModSprite.sprite = modData.Sprite;
    //         modIcon.SetEnabledTooltip($"{id} name".Localized());
    //         
    //         _modIcons.Add(newModImg);
    //         newModImg.SetActive(true);
    //     }
    // }
}