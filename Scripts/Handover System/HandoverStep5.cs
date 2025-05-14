using NoManual.Interaction;
using NoManual.Managers;
using UnityEngine;
using UnityEngine.Playables;

namespace NoManual.Tutorial
{
    public class HandoverStep5 : HandoverStep
    {
        [Header("Handover Ÿ�Ӷ���")] 
        [SerializeField] private PlayableAsset handover4PhoneTimeline;

        [Space(10)]
        [Header("�� ������Ʈ")]
        [SerializeField] private DoorComponent kDoor;
        [SerializeField] private ExitDoor lDoor;

        [Space(10)] 
        [Header("�ݶ��̴� ������Ʈ")]
        [SerializeField] private Collider collider8;
        
        private readonly int _MEDICINE_ITEM_ID = 4028;
        
        public override void Init(HandoverManager handoverManager)
        {
            base.Init(handoverManager);
            kDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
        }
        
        public override void StartStep()
        {
            kDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Close);
            lDoor.SetNoInteractive();
        }
        
        protected override void FirstStartTriggerStep()
        {
            kDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
            HandoverManager.PlayTimeLine(handover4PhoneTimeline, OnVideoEnd);
        }
        
        /// <summary>
        /// handover ���� ���� �̺�Ʈ
        /// </summary>
        private void OnVideoEnd()
        {
            Utils.OutLineUtil.AddOutLine(lDoor.gameObject, QuickOutline.Mode.OutlineAll, Color.yellow, 1.5f);
            lDoor.SetYesInteractive();
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(LocalizationTable.TextTable.Hint_Table, LocalizationTable.HintTableTextKey.HandoverHint_ + "10");
            // �κ��丮�� ������ ������ ���
            GameManager.Instance.SaveGameManager.CurrentPlayerSaveData.InventoryItems = new InventorySaveData[] { new InventorySaveData(_MEDICINE_ITEM_ID, 1, 0, InventorySlotType.BackPack) };
            // Ʃ�丮�� Ŭ���� 
            GameManager.Instance.SaveGameManager.CurrentPlayerSaveData.IsTutorialClear = true;
            // ���̺� ���� �����
            GameManager.Instance.SaveGameManager.SaveFile();
            
            EndStep();
        }

        protected override void EndStep()
        {
            _StepClearEvent?.Invoke(this);
            RemoveEvent();
        }

        public override void OnTriggerEvent(Collider other)
        {
            if (other == collider8)
            {
                collider8.enabled = false;
                FirstStartTriggerStep();
            }
        }
    }

}

