using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using NoManual.Managers;

namespace NoManual.UI
{

    /// <summary>
    /// 인벤토리 아이템 CIP PreviewSlot 상호작용 버튼
    /// </summary>
    public class UI_UsageOpContextButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
    
        [SerializeField] private Image contextButtonImage;
        [SerializeField] private UnityEvent onSelectEvent;
        private bool _onPointerDown = false;

        private void OnDisable()
        {
            Deselect();
        }


        private void Select()
        {
            contextButtonImage.color = NoManualHotelManager.Instance.InventoryManager
                .inventoryItemContextBackGroundColor
                .usageOpContextSelectedBackGroundColor;
        }

        public void Deselect()
        {
            contextButtonImage.color = NoManualHotelManager.Instance.InventoryManager
                .inventoryItemContextBackGroundColor
                .usageOpContextDefaultBackGroundColor;
        }



        public void OnPointerEnter(PointerEventData eventData)
        {
            Select();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Deselect();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_onPointerDown) _onPointerDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_onPointerDown)
            {
                onSelectEvent?.Invoke();
                _onPointerDown = false;
            }
        }
    }
}
