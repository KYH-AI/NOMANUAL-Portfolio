using NoManual.Interaction;
using NoManual.Managers;
using UnityEngine;
using UnityEngine.Playables;

namespace NoManual.Tutorial
{
    public class HandoverStep4 : HandoverStep
    {
        [Header("Handover Ÿ�Ӷ���")] 
        [SerializeField] private PlayableAsset handover3VideoTimeline;

        [Space(10)]
        [Header("�� ������Ʈ")]
        [SerializeField] private DoorComponent iDoor;
        [SerializeField] private DoorComponent jDoor;

        [Space(10)] 
        [Header("�ݶ��̴� ������Ʈ")]
        [SerializeField] private Collider collider7;

        [Space(10)] 
        [Header("��ǻ�� ������Ʈ")] 
        [SerializeField] private HandoverPC_OS_Component computer;

        // ���� ����Ʈ �Ϸ� ����
        private bool _checkReservation = false;
        // ���� ����Ʈ �Ϸ� ����
        private bool _checkReport = false;

        public override void Init(HandoverManager handoverManager)
        {
            base.Init(handoverManager);
            
            iDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
            jDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
            
            // ��ǻ�� ���� ����Ʈ �̺�Ʈ ���
            handoverManager.pcOSManager.ohHomePage.occasionHotelHomePageHelper.AllClearReserveTaskHanlder -= handoverManager.TaskHandler.ManualTaskClearHandler;
            handoverManager.pcOSManager.ohHomePage.occasionHotelHomePageHelper.AllClearReserveTaskHanlder += handoverManager.TaskHandler.ManualTaskClearHandler;
            
            // ���� �� ���� �̺�Ʈ ���
            handoverManager.pcOSManager.InitializationAppEvent(Report);
        }
        
        public override void StartStep()
        {
            iDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Close);
        }

        private void Update()
        {
            if (_checkReservation)
            {
                // ��� ������ ó�� Ȯ��
                if (HandoverManager.pcOSManager.ohHomePage.occasionHotelHomePageHelper.reservationCustomers.Count <= 0)
                {
                    // ����ó�� �Ϸ� �˾� UI ���
                    computer.CreateNotification(true);
                    _checkReservation = false;
                }
            }
        }

        protected override void FirstStartTriggerStep()
        {
            iDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
            computer.CompleteReportEvent -= EndStep;
            computer.CompleteReportEvent += EndStep;
            computer.SetNoInteractive();
            HandoverManager.PlayTimeLine(handover3VideoTimeline, OnVideoEnd);
        }

        protected override void EndStep()
        {
            if (!_checkReport) return;
            Utils.OutLineUtil.RemoveOutLine(computer.gameObject);
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(LocalizationTable.TextTable.Hint_Table, LocalizationTable.HintTableTextKey.HandoverHint_Next.ToString());
            computer.SetNoInteractive();
            jDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Open);
            
            // ���� �Ϸ� �̺�Ʈ ����
            computer.CompleteReportEvent -= EndStep;
            _checkReport = false;
            
            _StepClearEvent?.Invoke(this);
            RemoveEvent();
        }

        /// <summary>
        /// handover ���� ���� �̺�Ʈ
        /// </summary>
        private void OnVideoEnd()
        {
            // ������ ���� �ʱ�ȭ
            HandoverManager.pcOSManager.SetReservedCustomer(null);
            // �� ����Ʈ �ʱ�ȭ
            HandoverManager.pcOSManager.SetGuestRoomData(null);
            // ���� ������ ����
            HandoverManager.pcOSManager.AddRandomReservationItem();
            // ���� ����Ʈ ����
            _checkReservation = true;
            computer.SetYesInteractive();
            Utils.OutLineUtil.AddOutLine(computer.gameObject, QuickOutline.Mode.OutlineAll, Color.yellow, 1.5f);
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(LocalizationTable.TextTable.Hint_Table, LocalizationTable.HintTableTextKey.HandoverHint_ + "9");
        }

        /// <summary>
        /// ���� ���� Ȯ�� (���� ��ư �̺�Ʈ)
        /// </summary>
        private bool Report(bool checkOnly)
        {
            /* 1. �⺻ �ٹ� ��� Ŭ���� Ȯ�� */
            var stdRoundTaskList = HandoverManager.TaskHandler.GetRoundStandardTask(1);
            foreach (var standardTask in stdRoundTaskList)
            {
                if (!standardTask.isClear)
                {
                    if (checkOnly) return false; // ���� �������� Ȯ�� ����� ��� �ٷ� ��ȯ
                }
            }
            if (checkOnly) return true;

            // ���� ���� �Ϸ� �˾� UI ���
            computer.CreateNotification(false);
            // ��� �⺻ �ٹ��� Ŭ����� ��쿡�� ���� ����
            return _checkReport = true;
        }

        public override void OnTriggerEvent(Collider other)
        {
            if (other == collider7)
            {
                collider7.enabled = false;
                FirstStartTriggerStep();
            }
        }
    }
}
