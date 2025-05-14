using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoManual.Inventory;
using NoManual.Managers;
using ThunderWire.Utility;

namespace NoManual.Interaction
{
    /// <summary>
    /// 아이템 컴포넌트
    /// </summary>
    public class ItemComponent : InteractionBase
    {
        // 상호작용 아이템 종류
        public enum ItemType
        {
            None = -1,
            InventoryItem = 0, // 인벤토리 아이템
            OnlyPaperExamine = 1,  // 종이 아이템 (UI 호출)
        }
        [Header("상호작용 종류가 Item인 경우 등록")]
        public ItemType itemType;
        
        [Header("인벤토리 아이템인 경우 등록")]
        public ItemScriptable inventoryItem;
        public int InventoryItemId { get; private set;  }

        [Header("읽기만 가능한 아이템인 경우 등록")]
        public ManualBookScriptable manualBookItem;

        [Serializable]
        public struct ItemSettings
        {
            [Tooltip("획득 아이템 개수")] public int itemAmount;
            [Tooltip("획득 시 아이템 자동 교체 (퀵슬롯 등록 아님)")]  public bool autoSwitch;
        }
        
        [Serializable]
        public struct ItemSounds
        {
            [Header("아이템 상호작용 사운드")]
            public AudioClip pickUpSound;
            [Header("아이템 조사 사운드 (데모 X)")]
            public AudioClip examineSound;
        }

        [Header("인벤토리 아이템 세팅")]
        public ItemSettings itemSettings = new ItemSettings();
        [Header("아이템 상호작용 효과음")]
        public ItemSounds itemSounds = new ItemSounds();
        
        // R.S 트레이더스 배송품 확인용
        public bool IsDeliveryItem { get; set; } = false;
        
        [Space(10)]
        [Header("상호작용 아이콘 위치")]
        [SerializeField] private Transform floatingIconRoot;

        private void Awake()
        {
            if (inventoryItem != null)
            {
                interactionType = InteractionType.Item;
                this.InventoryItemId = inventoryItem.itemId;
                // Raycast 이름을 위한 로컬라이징
                title = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.Inventory_Item_Table,
                    LocalizationTable.Inventory_Item_TableTextKey.Inventory_Item_.ToString() + InventoryItemId.ToString());
            }
            else if (manualBookItem != null)
            {
                interactionType = InteractionType.Read;
                title =  GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.Manual_Book_Item_Table,
                    LocalizationTable.Manual_Book_Item_TableTextKey.Manual_Book_.ToString() + manualBookItem.manualBookId.ToString());
            }
        }

        public void InitializedItemComponent(int inventoryItemId, int itemAmount)
        {
            if (manualBookItem) return;
            
            this.InventoryItemId = inventoryItemId;
            itemSettings.itemAmount = itemAmount;
        }

        /// <summary>
        /// (추상화) 아이템 상호작용
        /// </summary>
        public override void Interact()
        {
            bool successFullItemInteract = true;
            
            // 인벤토리 아이템 
            if (itemType == ItemType.InventoryItem)
            {
                successFullItemInteract =  NoManualHotelManager.Instance.InventoryManager.PickUpAddItem(this);

                if (successFullItemInteract)
                {
                    ItemCloneData itemCloneData =  NoManualHotelManager.Instance.InventoryManager.GetItemCloneData(InventoryItemId);

                    // 착용 가능한 아이템인 경우 아이템 장착
                    if (itemCloneData.InventoryItemUseType == InventoryItemUseType.Equipment)
                    {
                        if (itemSettings.autoSwitch)
                        {
                            NoManualHotelManager.Instance.ItemShortCutManager.EquipItemSwitch(itemCloneData.itemSettings.equipItemId);
                        }
                    }
                }
            }
            // 책 아이템
            else if (itemType == ItemComponent.ItemType.OnlyPaperExamine)
            {
                if (manualBookItem is null) return;
                NoManualHotelManager.Instance.UiNoManualUIManager.CreatPaperUI(manualBookItem.manualBookId);
            }

            // 아이템 상호작용 성공 
            if (successFullItemInteract)
            {
                // Pick Up 사운드 재생
                if (itemSounds.pickUpSound)
                {
                    Utilities.PlayOneShot2D(transform.position, OptionHandler.AudioMixerChanel.SFX, itemSounds.pickUpSound);
                }
                
                // 배낭에 수집이 가능한 아이템이 아닌경우 무시
                if(itemType != ItemType.InventoryItem) return;

                // 아이템 줍기에 성공하면 상호작용된 아이템 삭제 // 
                Destroy(this.gameObject);
            }
        }
        
        public override GameObject GetAnotherInteractionObject()
        {
            return floatingIconRoot ? floatingIconRoot.gameObject : base.GetAnotherInteractionObject();
        }

        public override void SetNoInteractive()
        {
            this.gameObject.layer = (int)Utils.Layer.LayerIndex.Default;
            
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.layer = (int)Utils.Layer.LayerIndex.Default;
            }
        }

        /// <summary>
        /// 아이템 오브젝트 물리 비활성화
        /// </summary>
        public void RemovePhysicsComponent()
        {
            // 부모나 자신으로부터 Rigidbody를 찾아 비활성화 (isKinematic = true)
            Rigidbody rb = GetComponentInChildren<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; 
            }

            // 자식 및 자신으로부터 모든 Collider 컴포넌트를 찾아 비활성화
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }
        }
    }
}
