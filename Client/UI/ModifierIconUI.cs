using EFT.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KoTClient.UI;

public class ModifierIconUI : InteractableElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] 
    public Image ModSprite;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        GClass3746.SetCursor(ECursorType.Hover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GClass3746.SetCursor(ECursorType.Idle);
    }
}