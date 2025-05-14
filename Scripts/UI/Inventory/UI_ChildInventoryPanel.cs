using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NoManual.Inventory;
using NoManual.Managers;
using TMPro;

namespace NoManual.UI
{

    /// <summary>
    /// UI CIP
    /// </summary>
    public class UI_ChildInventoryPanel : MonoBehaviour
    {
        private InventorySlotType _inventorySlotType;

        [HideInInspector]
        public List<UI_ChildInventoryPanelEmptySlot> cipSlots = new List<UI_ChildInventoryPanelEmptySlot>();

        [SerializeField] private TextMeshProUGUI cipPanelNameText;
        [SerializeField] private Image cipIconImage;
        [SerializeField] private Transform cipSlotRoot;

        public void InitializeCIPHeader(string cipName, Sprite cipIcon, InventorySlotType type)
        {
            _inventorySlotType = type;
            cipPanelNameText.text = cipName;
            cipIconImage.sprite = cipIcon;
        }

        public void AddCipEmptySlot(UI_ChildInventoryPanelEmptySlot cipEmptySlot, int slotFrameId)
        {
            cipEmptySlot.SetCipEmptySlotId(slotFrameId, _inventorySlotType);
            cipSlots.Add(cipEmptySlot);

            // 부모 설정
            cipEmptySlot.transform.SetParent(cipSlotRoot);
            cipEmptySlot.transform.localScale = Vector3.one;
        }

        /// <summary>
        /// CIP Empty Slot 모두 막기
        /// </summary>
        public void AllCipSlotLock()
        {
            foreach (var cipEmptySlot in cipSlots)
            {
                cipEmptySlot.CipEmptySlotLock();
            }
        }

        /// <summary>
        /// CIP Empty Slot 모두 초기화
        /// </summary>
        public void AllResetCipSlots()
        {
            foreach (var cipEmptySlot in cipSlots)
            {
                cipEmptySlot.ResetCipSlotProperty();
            }
        }
        
        /// <summary>
        /// 특정 CIP Empty Slot 막기
        /// </summary>
        public void TargetClipSlotLock(int itemId)
        {
            foreach (var cipEmptySlot in cipSlots)
            {
                InventoryItem inventoryItem = cipEmptySlot.GetCipPanelPreviewSlotItem();
                if (inventoryItem == null) continue;
                if (inventoryItem.item.itemId == itemId)
                {
                    cipEmptySlot.CipEmptySlotLock();
                }
            }
        }

        /// <summary>
        /// 특정 CIP Empty Slot 초기화
        /// </summary>
        public void TargetClipSlotReset(int itemId)
        {
            foreach (var cipEmptySlot in cipSlots)
            {
                InventoryItem inventoryItem = cipEmptySlot.GetCipPanelPreviewSlotItem();
                if (inventoryItem == null) continue;
                if (inventoryItem.item.itemId == itemId)
                {
                    cipEmptySlot.ResetCipSlotProperty();
                }
            }
        }

    
    }
}
