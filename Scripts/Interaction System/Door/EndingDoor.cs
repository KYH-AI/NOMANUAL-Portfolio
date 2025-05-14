using System.Collections;
using System.Collections.Generic;
using NoManual.Managers;
using NoManual.Task;
using UnityEngine;

namespace NoManual.Interaction
{
    /// ---------- 24.12.12 ExitDoor ������Ʈ�� ��� �����    
    public class EndingDoor : DoorComponent
    {
        public override void PutInteraction()
        {
            PutCallBackMapper putCallBack = new PutCallBackMapper(TaskHandler.TaskID.Put_Inventory_Lock_Door.ToString(), 
                TaskTargetId, 
                RequestItemId, 
                EndingScene,
                this);
            NoManualHotelManager.Instance.InventoryManager.ShowPutInventory(putCallBack);
        }

        private void EndingScene()
        {
            HotelManager.Instance.ExitHotel(true, false, GameManager.SceneName.Monologue);
        }
    }

}

