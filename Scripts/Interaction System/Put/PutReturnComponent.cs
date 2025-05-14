using System.Collections;
using System.Collections.Generic;
using NoManual.Managers;
using NoManual.Task;
using UnityEngine;

namespace NoManual.Interaction
{
    public class PutReturnComponent : PutBaseComponent, IPut
    {
        public void InitializationPutMode()
        {
            SetNoInteractive();
            CurrentPutMode = IPut.PutMode.None;
            RequestItemId = null;
            PutComplete = false;
        }

        public IPut.PutMode GetMode => IPut.PutMode.Put;
        
        protected override IPut InitializationPutInterface()
        {
            return this;
        }

        public void PutInteraction()
        {
            PutCallBackMapper putCallBack = new PutCallBackMapper(TaskHandler.TaskID.Put_Inventory_Item_Return.ToString(), 
                TaskTargetId, 
                RequestItemId, 
                () => PutComplete = true,
                this);
            NoManualHotelManager.Instance.InventoryManager.ShowPutInventory(putCallBack);
        }

        public void SwapPutMode()
        {
            /*
            // 현재 모드가 None이면 Put으로 변경하고, 그렇지 않으면 None으로 설정
            CurrentPutMode = CurrentPutMode == IPut.PutMode.None ? IPut.PutMode.Put : IPut.PutMode.None;

            // 모드에 따라 상호작용 가능 여부를 설정
            if (CurrentPutMode == IPut.PutMode.Put) SetYesInteractive();
            else  SetNoInteractive();
            */

            if (CurrentPutMode == IPut.PutMode.None)
            {
                SetYesInteractive();
                CurrentPutMode = IPut.PutMode.Put;
            }
        }

        public bool RemoveInventoryItem() => RemoveRequestItem;
    }
}


