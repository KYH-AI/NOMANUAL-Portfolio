using NoManual.Interaction;
using NoManual.Managers;
using ThunderWire.Input;
using UnityEngine;
using UnityEngine.Playables;

namespace NoManual.Tutorial
{
    public class HandoverStep1 : HandoverStep
    {
        [SerializeField] private PlayableAsset handover0VideoTimeline;
        [SerializeField] private DoorComponent aDoor;
        [SerializeField] private DoorComponent bDoor;
        [SerializeField] private GameObject checkList;
        [SerializeField] private Collider collider0;
        
        private InventoryStep.InventoryTutorialState _inventoryTutorialTutorialState = InventoryStep.InventoryTutorialState.Closed;
        private int _checkListItemId;
        private bool _hasItem = false;
        
        public override void Init(HandoverManager handoverManager)
        {
            base.Init(handoverManager);
            
            // �� �ʱ�ȭ
            bDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
            _checkListItemId = checkList.GetComponent<ItemComponent>().inventoryItem.itemId;
        }
        
        public override void StartStep()
        {
            aDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Close);
        }
        
        protected override void FirstStartTriggerStep()
        {
            // üũ����Ʈ ������Ʈ ��ȣ�ۿ� ����
            checkList.GetComponent<ItemComponent>().SetNoInteractive();
            
            // a �� ���
            aDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
            
            // ������ üũ Ȯ�� �̺�Ʈ ��� (�̺�Ʈ �۵� �� ������ü �˾Ƽ� ����)
            NoManualHotelManager.Instance.InventoryManager.RegisterItemCheckAction(_checkListItemId, () => _hasItem = true);
            
            // TV Ÿ�Ӷ��� ����
            HandoverManager.PlayTimeLine(handover0VideoTimeline, OnVideoEnd);
        }

        private void Update()
        {
            if(InputHandler.ReadButtonOnce(this, "Inventory"))
            {
                switch (_inventoryTutorialTutorialState)
                {
                    case InventoryStep.InventoryTutorialState.Closed:
                        _inventoryTutorialTutorialState = InventoryStep.InventoryTutorialState.Opened;
                        break;
                    case InventoryStep.InventoryTutorialState.Opened:
                        if(_hasItem) EndStep(); // �賶�� üũ����Ʈ�� �����ϸ� ���� 1 ����
                        _inventoryTutorialTutorialState = InventoryStep.InventoryTutorialState.Closed;
                        break;
                }
            }
        }

        /// <summary>
        /// üũ����Ʈ UI�� ���� �� ����
        /// </summary>
        protected override void EndStep()
        {
            // b �� ����
            bDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Open);
            
            // HandoverHint_Next ǥ��
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(LocalizationTable.TextTable.Hint_Table, LocalizationTable.HintTableTextKey.HandoverHint_Next.ToString());
            
            _StepClearEvent?.Invoke(this);
            RemoveEvent();
        }

        /// <summary>
        /// ���� ���� �̺�Ʈ
        /// </summary>
        private void OnVideoEnd()
        {
            // üũ����Ʈ ������Ʈ �ƿ����� Ȱ��ȭ
            Utils.OutLineUtil.AddOutLine(checkList, QuickOutline.Mode.OutlineAll, Color.yellow, 1.5f);
            // üũ����Ʈ ������Ʈ ��ȣ�ۿ� Ȱ��ȭ
            checkList.GetComponent<ItemComponent>().SetYesInteractive();
            // HandoverHint0 ǥ��
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(LocalizationTable.TextTable.Hint_Table, LocalizationTable.HintTableTextKey.HandoverHint_ + "0");
        }

        /// <summary>
        /// Step1 Ʈ����
        /// </summary>
        public override void OnTriggerEvent(Collider other)
        {
            if (other == collider0)
            {
                collider0.enabled = false;
                FirstStartTriggerStep();
            }
        }
    }
}

