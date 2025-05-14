using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using NoManual.Managers;

namespace NoManual.UI
{
    /// <summary>
    /// 아이템 UI 슬롯 Frame (CIP item slot)
    /// </summary>
    public class UI_ChildInventoryPanelEmptySlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private InventorySlotType _slotType;
        private UI_ChildInventoryPanelPreviewSlot _uiChildInventoryPanelPreviewSlot;
        private bool _isSelected = false;       // 선택됨
        private bool _isSelectable = true;      // 선택 가능 여부
        private bool _isPointerDown = false;
        public int SlotId { get; private set; }

        [SerializeField] private Transform cipEmptySlotRoot;
        [SerializeField] private Text itemAmountText;
        [Tooltip("아이템 선택 시 배경 이미지")] [SerializeField] private Image cipSlotBackGround;
        [Tooltip("아이템 Frame 배경 이미지")] [SerializeField] private Image cipSlotFrame;

        /// <summary>
        ///  CIP Empty Slot 초기 생성
        /// </summary>
        public void SetCipEmptySlotId(int id, InventorySlotType slotType)
        {
            cipSlotBackGround.enabled = false;
            cipSlotBackGround.sprite = null;
            _slotType = slotType;
            SlotId = id;
        }
        
        /// <summary>
        /// CIP PreViewSlot 생성과 동시에 초기화
        /// </summary>
        /// <param name="cipPreviewSlot"></param>
        public void InitializeCipPreviewSlot(UI_ChildInventoryPanelPreviewSlot cipPreviewSlot)
        {
            cipSlotBackGround.enabled = true;
            cipSlotBackGround.sprite =  NoManualHotelManager.Instance.InventoryManager.inventoryCipEmptySlotBackGroundImages.cipEmptySlotWithItemSprite;
            cipSlotBackGround.color =  NoManualHotelManager.Instance.InventoryManager.inventoryItemContextBackGroundColor.cipEmptyDefaultBackGroundColor;
            
            this._uiChildInventoryPanelPreviewSlot = cipPreviewSlot;
            _uiChildInventoryPanelPreviewSlot.transform.SetParent(cipEmptySlotRoot);
            _uiChildInventoryPanelPreviewSlot.transform.localPosition = Vector3.one;
            _uiChildInventoryPanelPreviewSlot.Initialized(UpdateCipPreviewSlotItemAmountText);
            _uiChildInventoryPanelPreviewSlot.SetCipPreviewSlotId(SlotId);
        }

        /// <summary>
        /// 아이템 정보 및 CIP PreviewSlot 삭제
        /// </summary>
        public void DropSlotItem()
        {
            if (_uiChildInventoryPanelPreviewSlot != null)
            {
                NoManualHotelManager.Instance.InventoryManager.DropItemGround(_slotType, SlotId, _uiChildInventoryPanelPreviewSlot.ItemAmount, _uiChildInventoryPanelPreviewSlot.InventoryItemInfo);
            }
        }
        

        /// <summary>
        /// CIP PreViewSlot 삭제
        /// </summary>
        public void RemoveCipPreviewSlot()
        {
            if (_uiChildInventoryPanelPreviewSlot != null)
            {
                cipSlotBackGround.enabled = false;
                itemAmountText.text = string.Empty;
                Destroy(_uiChildInventoryPanelPreviewSlot.gameObject);
                _uiChildInventoryPanelPreviewSlot = null;
            }
        }
        
        /// <summary>
        /// CIP PreViewSlot 아이템 정보 얻기
        /// </summary>
        public InventoryItem GetCipPanelPreviewSlotItem()
        {
            InventoryItem slotInventoryItem = null;
            if (_uiChildInventoryPanelPreviewSlot)
                slotInventoryItem = _uiChildInventoryPanelPreviewSlot.InventoryItemInfo;

            return slotInventoryItem;
        }

        /// <summary>
        /// CIP PreViewSlot UI 얻기
        /// </summary>
        public UI_ChildInventoryPanelPreviewSlot GetCipPanelPreviewSlot()
        {
            return _uiChildInventoryPanelPreviewSlot;
        }

        /// <summary>
        /// CIP PreViewSlot 존재 여부 확인
        /// </summary>
        public bool CheckCipPreviewSlot()
        {
            return (bool)_uiChildInventoryPanelPreviewSlot;
        }
        
        /// <summary>
        /// CIP PreViewSlot 아이템 정보 할당 (아이템)
        /// </summary>
        public void UpdateCipPreviewSlot(InventoryItem item, int itemAmount)
        {
            _uiChildInventoryPanelPreviewSlot.UpdateCipPreviewSlot(item, itemAmount);
        }

        /// <summary>
        /// CIP PreViewSlot 아이템 개수 할당 (아이템)
        /// </summary>
        public void UpdateCipPreviewSlotItemAmount(int itemAmount)
        {
            _uiChildInventoryPanelPreviewSlot.SetCipPreviewSlotAmount(itemAmount);
        }
        
        private void UpdateCipPreviewSlotItemAmountText(string itemAmount)
        {
            itemAmountText.text = itemAmount;
        }

        /// <summary>
        /// CIP Empty Slot 클릭 막기
        /// </summary>
        public void CipEmptySlotLock()
        {
            Disable();
            
            _isSelected = false;
            _isSelectable = false;
        }

        /// <summary>
        /// CIP Empty Slot 초기화
        /// </summary>
        public void ResetCipSlotProperty()
        {
            Deselect();
            
            _isSelected = false;
            _isSelectable = true;
            _isPointerDown = false;
        }

        private void Select()
        {
            NoManualHotelManager.Instance.InventoryManager.LastSelectedSlotToReset(); // 마지막에 선택된 Slot Deselect 진행
            
            NoManualHotelManager.Instance.InventoryManager.IsSelecting = _isSelected = true;
            NoManualHotelManager.Instance.InventoryManager.SelectSlotId = SlotId;
            NoManualHotelManager.Instance.InventoryManager.SelectInventorySlotTypeSlotType = _slotType;

            cipSlotBackGround.color =  NoManualHotelManager.Instance.InventoryManager.inventoryItemContextBackGroundColor.cipEmptySelectedBackGroundColor;
            cipSlotBackGround.sprite =  NoManualHotelManager.Instance.InventoryManager.inventoryCipEmptySlotBackGroundImages.cipEmptySlotSelectSprite;

            NoManualHotelManager.Instance.InventoryManager.ShowItemDetail(_isSelected, _uiChildInventoryPanelPreviewSlot.InventoryItemInfo.item.itemTitle, _uiChildInventoryPanelPreviewSlot.InventoryItemInfo.item.description);
        }

        private void Deselect()
        { 
            cipSlotBackGround.color =  NoManualHotelManager.Instance.InventoryManager.inventoryItemContextBackGroundColor.cipEmptyDefaultBackGroundColor;
            cipSlotBackGround.sprite =  NoManualHotelManager.Instance.InventoryManager.inventoryCipEmptySlotBackGroundImages.cipEmptySlotWithItemSprite;
            // 아이템 이미지 배경 색 변경
            if(_uiChildInventoryPanelPreviewSlot)
                _uiChildInventoryPanelPreviewSlot.SetCipPreviewSlotIconImage(NoManualHotelManager.Instance.InventoryManager.inventoryItemContextBackGroundColor.cipEmptyDefaultBackGroundColor);
        }

        private void Disable()
        {
            cipSlotBackGround.color =  NoManualHotelManager.Instance.InventoryManager.inventoryItemContextBackGroundColor.cipEmptyDisableBackGroundColor;
            // 아이템 이미지 배경 색 변경
            if(_uiChildInventoryPanelPreviewSlot)
                _uiChildInventoryPanelPreviewSlot.SetCipPreviewSlotIconImage(NoManualHotelManager.Instance.InventoryManager.inventoryItemContextBackGroundColor.cipPreviewSlotDisableBackGroundColor);
        }

        private void ShowContextMenu()
        {
            NoManualHotelManager.Instance.InventoryManager.ShowItemContextMenu(true, _uiChildInventoryPanelPreviewSlot.InventoryItemInfo, _slotType, SlotId);
        }

        private void CombineItem()
        {
            NoManualHotelManager.Instance.InventoryManager.CombineItem();
        }

        private void PutItem()
        {
            NoManualHotelManager.Instance.InventoryManager.PutInventoryItem();
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_uiChildInventoryPanelPreviewSlot != null)
            {
               // Debug.Log(_uiChildInventoryPanelPreviewSlot.InventoryItemInfo.item.itemTitle +" OnPointerDown!");
               if (!NoManualHotelManager.Instance.InventoryManager.IsContextMenuVisible)
               {
                   if (_isSelectable)
                   {
                       _isPointerDown = true;
                   }
               }
               else
               {
                   NoManualHotelManager.Instance.InventoryManager.ResetInventory();
                   Select();
               }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_uiChildInventoryPanelPreviewSlot != null)
            {
                //Debug.Log(_uiChildInventoryPanelPreviewSlot.InventoryItemInfo.item.itemTitle +" OnPointerUp!");
                if (_isPointerDown)
                {
                    if (!_isSelected) Select(); // 슬롯 1회 클릭시 
                    else if(!NoManualHotelManager.Instance.InventoryManager.IsCombineMode && !NoManualHotelManager.Instance.InventoryManager.IsPutMode) ShowContextMenu(); // 슬롯 2회 클릭시
                    else if(NoManualHotelManager.Instance.InventoryManager.IsCombineMode) CombineItem(); // 아이템 조합
                    else if(NoManualHotelManager.Instance.InventoryManager.IsPutMode) PutItem(); // 아이템 내려놓기
                    
                    _isPointerDown = false;
                }
            }
        }
    }

}

