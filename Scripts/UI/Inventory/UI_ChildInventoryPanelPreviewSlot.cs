using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using NoManual.Managers;

namespace NoManual.UI
{
    /// <summary>
    /// 아이템 UI CIP (CIP item)
    /// </summary>
    public class UI_ChildInventoryPanelPreviewSlot : MonoBehaviour
    {
        private int _slotId;
        public InventoryItem InventoryItemInfo { get; private set; }
        private UnityAction<string> _itemAmountTextUpdate;
        
        [SerializeField] private Image itemIcon;
        [SerializeField] private Image shortCutIcon;
        
        private int _itemAmount;
        public int ItemAmount
        {
            get => _itemAmount;
            private set
            {
                _itemAmount = value;
                _itemAmountTextUpdate?.Invoke(_itemAmount.ToString());
            }
        }
        
        // 퀵슬롯 UI 표시 용
        public string ShortCut { get; set; } = string.Empty;

        public void SetCipPreviewSlotId(int id)
        {
            _slotId = id;
        }

        public void Initialized(UnityAction<string> itemAmountTextEvent)
        {
            _itemAmountTextUpdate -= itemAmountTextEvent;
            _itemAmountTextUpdate += itemAmountTextEvent;
        } 
        
        public void UpdateCipPreviewSlot(InventoryItem inventoryItem, int itemAmount)
        {
            this.InventoryItemInfo = inventoryItem;
            SetCipPreviewSlotIcon(InventoryItemInfo.item.itemIcon);
            SetCipPreviewSlotAmount(itemAmount);
        }

        /// <summary>
        /// 아이템 이미지 할당
        /// </summary>
        private void SetCipPreviewSlotIcon(Sprite icon)
        {
            this.itemIcon.sprite = icon;
        }

        public Sprite GetCipPreviewSlotIcon()
        {
            return this.itemIcon.sprite;
        }

        /// <summary>
        /// 아이템 수량 할당
        /// </summary>
        public void SetCipPreviewSlotAmount(int amount)
        {
            this.ItemAmount = amount;
        }

        /// <summary>
        /// 아이템 퀵슬롯 이미지 할당
        /// </summary>
        public void SetCipPreviewShortCutIcon(Sprite shortCut)
        {
            this.shortCutIcon.sprite = shortCut;
            this.shortCutIcon.enabled = shortCut;
        }

        /// <summary>
        /// 아이템 이미지 색상 변경
        /// </summary>
        public void SetCipPreviewSlotIconImage(Color color)
        {
            this.itemIcon.color = color;
        }
        
        private void OnDestroy()
        {
            InventoryItemInfo = null;
            // 이벤트 초기화
            _itemAmountTextUpdate = null;
        }
    }
}


