using UnityEngine;
using UnityEngine.EventSystems;
using NoManual.Inventory;
using NoManual.Managers;

namespace NoManual.UI
{
    /// <summary>
    /// 인벤토리 BG 클릭 시 초기화
    /// </summary>
    public class UI_ParentInventoryPanelDeselect : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            if(!NoManualHotelManager.Instance.InventoryManager.IsCombineMode && !NoManualHotelManager.Instance.InventoryManager.IsPutMode)
                 NoManualHotelManager.Instance.InventoryManager.ResetInventory();
        }
    }
}


