using NoManual.Interaction;
using NoManual.Managers;
using UnityEngine;
using UnityEngine.Playables;

namespace NoManual.Tutorial
{
    public class HandoverStep2 : HandoverStep
    {
        [Header("Handover Ÿ�Ӷ���")] 
        [SerializeField] private PlayableAsset handover1VideoTimeline;
        
        [Space(10)]
        [Header("�� ������Ʈ")]
        [SerializeField] private DoorComponent cDoor;
        [SerializeField] private DoorComponent dDoor;
        [SerializeField] private DoorComponent eDoor;
        [SerializeField] private DoorComponent door1;
        [SerializeField] private DoorComponent door2;
        [SerializeField] private DoorComponent door3;
        [SerializeField] private DoorComponent door4;

        [Space(10)] 
        [Header("�ݶ��̴� ������Ʈ")]
        [SerializeField] private Collider collider1;
        [SerializeField] private Collider collider2;
        [SerializeField] private Collider collider3;

        [Space(10)] 
        [Header("Put, Interact, Get, Patrol ������Ʈ")]
        [SerializeField] private InteractObjectBaseComponent aInteract;
        [SerializeField] private GameObject b_1Get;
        [SerializeField] private GameObject b_2Get;
        [SerializeField] private PutBaseComponent cPut;
        [SerializeField] private PutBaseComponent dPut;
        [SerializeField] private HandoverPatrolFieldComponent officePatrol;

        private bool _b_1Item = false;
        private bool _b_2Item = false;
        private bool _c_Put = false;
        private bool _d_Put = false;

        public override void Init(HandoverManager handoverManager)
        {
            base.Init(handoverManager);

            // �� �ʱ�ȭ
            cDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
            dDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
            eDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
            door1.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
            door2.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
            door3.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
            door4.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
            
            // ����Ʈ �ʱ�ȭ
            aInteract.TaskIdleMode(handoverManager.TaskHandler.InteractTaskCheckHandler);
            if (cPut.putInterface == null) cPut.Awake();
            cPut.RequestItemId = new int[] { 4003 };
            cPut.putInterface.SwapPutMode();
            
            if(dPut.putInterface == null) dPut.Awake();
            dPut.RequestItemId = new int[] { 4015 };
            dPut.putInterface.SwapPutMode();
            
            officePatrol.Init(handoverManager.TaskHandler.PatrolTaskCheckHandler);
        }

        private void Update()
        {
            if (_b_1Item && _b_2Item)
            {
                B_ObjectGetTrigger();
                _b_1Item = _b_2Item = false;
            }

            if (cPut.PutComplete && !_c_Put)
            {
                C_ObjectPutReturnTrigger();
                _c_Put = true;
            }

            if (dPut.PutComplete && !_d_Put)
            {
                D_ObjectPutRelayTrigger();
                _d_Put = true;
            }
        }

        public override void StartStep()
        {
            cDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Close);
        }

        protected override void FirstStartTriggerStep()
        {
            cDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
            
            // TV Ÿ�Ӷ��� ����
            HandoverManager.PlayTimeLine(handover1VideoTimeline, OnVideoEnd);
        }

        /// <summary>
        /// handover ���� ���� �̺�Ʈ
        /// </summary>
        private void OnVideoEnd()
        {
            // d �� ����
            dDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Open);

            // HandoverHint2 ǥ��
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(LocalizationTable.TextTable.Hint_Table, LocalizationTable.HintTableTextKey.HandoverHint_ + "2");
        }

        private void Collider2Trigger()
        {
            // HandoverHint3 ǥ��
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(LocalizationTable.TextTable.Hint_Table, LocalizationTable.HintTableTextKey.HandoverHint_ + "3");
        }

        private void Collider3Trigger()
        {
            // HandoverHint4 ǥ��
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(LocalizationTable.TextTable.Hint_Table, LocalizationTable.HintTableTextKey.HandoverHint_ + "4");
            door1.DoorHandler(DoorBaseComponent.DoorStateEnum.Open);
        }

        /// <summary>
        /// A ������Ʈ UnityEvent �ν����Ϳ��� ȣ��
        /// </summary>
        public void A_ObjectInteractTrigger()
        {
            // HandoverHint5 ǥ��
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(LocalizationTable.TextTable.Hint_Table, LocalizationTable.HintTableTextKey.HandoverHint_ + "5");
            door2.DoorHandler(DoorBaseComponent.DoorStateEnum.Open);

            // ������ üũ Ȯ�� �̺�Ʈ ��� (�̺�Ʈ �۵� �� ������ü �˾Ƽ� ����)
            NoManualHotelManager.Instance.InventoryManager.RegisterItemCheckAction(4003, () => _b_1Item = true);
            NoManualHotelManager.Instance.InventoryManager.RegisterItemCheckAction(4015, () => _b_2Item = true);
        }

        public void B_ObjectGetTrigger()
        {
            // HandoverHint6 ǥ��
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(LocalizationTable.TextTable.Hint_Table, LocalizationTable.HintTableTextKey.HandoverHint_ + "6");
            door3.DoorHandler(DoorBaseComponent.DoorStateEnum.Open);
        }
        
        public void C_ObjectPutReturnTrigger()
        {
            // HandoverHint7 ǥ��
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(LocalizationTable.TextTable.Hint_Table, LocalizationTable.HintTableTextKey.HandoverHint_ + "7");
            door4.DoorHandler(DoorBaseComponent.DoorStateEnum.Open);
        }
        
        public void D_ObjectPutRelayTrigger()
        {
            eDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Open);
            EndStep();
        }

        /// <summary>
        /// ���� ����
        /// </summary>
        protected override void EndStep()
        {
            // HandoverHint_Next ǥ��
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(
                LocalizationTable.TextTable.Hint_Table,
                LocalizationTable.HintTableTextKey.HandoverHint_Next.ToString());

            _StepClearEvent?.Invoke(this);
            RemoveEvent();
        }

        public override void OnTriggerEvent(Collider other)
        {
            if (other == collider1)
            {
                collider1.enabled = false;
                FirstStartTriggerStep();
            }
            else if (other == collider2)
            {
                collider2.enabled = false;
                Collider2Trigger();
            }
            else if (other == collider3)
            {
                collider3.enabled = false;
                Collider3Trigger();
            }
        }
    }
}
