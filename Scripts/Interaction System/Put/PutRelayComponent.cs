using System.Collections;
using System.Collections.Generic;
using NoManual.Managers;
using NoManual.Task;
using UnityEngine;

namespace NoManual.Interaction
{
    public class PutRelayComponent : PutBaseComponent, IPut
    {
        [Header("Object 생성 할 위치")]
        [SerializeField] private Transform instantiatePrefabRoot;

        public void InitializationPutMode()
        {
            SetNoInteractive();
            CurrentPutMode = IPut.PutMode.None;
            RequestItemId = null;
            PutComplete = false;
        }

        public IPut.PutMode GetMode => CurrentPutMode;
        
        protected override IPut InitializationPutInterface()
        {
            return this;
        }

        public virtual void PutInteraction()
        {
            PutCallBackMapper putCallBack = new PutCallBackMapper(TaskHandler.TaskID.Put_Inventory_Item_Relay.ToString(), 
                TaskTargetId, 
                RequestItemId, 
                CreatePrefab,
                this);
            NoManualHotelManager.Instance.InventoryManager.ShowPutInventory(putCallBack);
        }

        private void CreatePrefab()
        {
            InventoryManager inventory = NoManualHotelManager.Instance.InventoryManager;
            if (inventory.SelectSlotId == -1 || inventory.SelectInventorySlotTypeSlotType == InventorySlotType.None)  return;

            InventoryItem inventoryItem = inventory.GetSlotIdToInventoryItem(inventory.SelectSlotId, inventory.SelectInventorySlotTypeSlotType);
            if (inventory == null) return;
            
            inventory.DropItemGround(inventory.SelectInventorySlotTypeSlotType, inventory.SelectSlotId, 1, inventoryItem, true, instantiatePrefabRoot);
            PutComplete = true;
        }

        public void SwapPutMode()
        {
            // 현재 모드가 None이면 Put으로 변경하고, 그렇지 않으면 None으로 설정
            CurrentPutMode = CurrentPutMode == IPut.PutMode.None ? IPut.PutMode.Put : IPut.PutMode.None;

            // 모드에 따라 상호작용 가능 여부를 설정
            if (CurrentPutMode == IPut.PutMode.Put)
            {
                if(highLightIcon) highLightIcon.SetItemHighLight();
                SetYesInteractive();
            }
            else
            {
                if(highLightIcon) highLightIcon.HideItemHighLight();
                SetNoInteractive();
            }
        }

        public bool RemoveInventoryItem() => RemoveRequestItem;
    }
}


