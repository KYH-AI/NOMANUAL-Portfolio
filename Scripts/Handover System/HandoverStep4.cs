using NoManual.Interaction;
using NoManual.Managers;
using UnityEngine;
using UnityEngine.Playables;

namespace NoManual.Tutorial
{
    public class HandoverStep4 : HandoverStep
    {
        [Header("Handover 타임라인")] 
        [SerializeField] private PlayableAsset handover3VideoTimeline;

        [Space(10)]
        [Header("문 컴포넌트")]
        [SerializeField] private DoorComponent iDoor;
        [SerializeField] private DoorComponent jDoor;

        [Space(10)] 
        [Header("콜라이더 컴포넌트")]
        [SerializeField] private Collider collider7;

        [Space(10)] 
        [Header("컴퓨터 컴포넌트")] 
        [SerializeField] private HandoverPC_OS_Component computer;

        // 예약 퀘스트 완료 여부
        private bool _checkReservation = false;
        // 보고 퀘스트 완료 여부
        private bool _checkReport = false;

        public override void Init(HandoverManager handoverManager)
        {
            base.Init(handoverManager);
            
            iDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
            jDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
            
            // 컴퓨터 예약 퀘스트 이벤트 등록
            handoverManager.pcOSManager.ohHomePage.occasionHotelHomePageHelper.AllClearReserveTaskHanlder -= handoverManager.TaskHandler.ManualTaskClearHandler;
            handoverManager.pcOSManager.ohHomePage.occasionHotelHomePageHelper.AllClearReserveTaskHanlder += handoverManager.TaskHandler.ManualTaskClearHandler;
            
            // 보고 앱 제출 이벤트 등록
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
                // 모든 예약자 처리 확인
                if (HandoverManager.pcOSManager.ohHomePage.occasionHotelHomePageHelper.reservationCustomers.Count <= 0)
                {
                    // 예약처리 완료 팝업 UI 출력
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
            
            // 보고 완료 이벤트 제거
            computer.CompleteReportEvent -= EndStep;
            _checkReport = false;
            
            _StepClearEvent?.Invoke(this);
            RemoveEvent();
        }

        /// <summary>
        /// handover 비디오 종료 이벤트
        /// </summary>
        private void OnVideoEnd()
        {
            // 예약자 정보 초기화
            HandoverManager.pcOSManager.SetReservedCustomer(null);
            // 방 리스트 초기화
            HandoverManager.pcOSManager.SetGuestRoomData(null);
            // 랜덤 예약자 생성
            HandoverManager.pcOSManager.AddRandomReservationItem();
            // 예약 퀘스트 추적
            _checkReservation = true;
            computer.SetYesInteractive();
            Utils.OutLineUtil.AddOutLine(computer.gameObject, QuickOutline.Mode.OutlineAll, Color.yellow, 1.5f);
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(LocalizationTable.TextTable.Hint_Table, LocalizationTable.HintTableTextKey.HandoverHint_ + "9");
        }

        /// <summary>
        /// 라운드 보고 확인 (보고 버튼 이벤트)
        /// </summary>
        private bool Report(bool checkOnly)
        {
            /* 1. 기본 근무 모두 클리어 확인 */
            var stdRoundTaskList = HandoverManager.TaskHandler.GetRoundStandardTask(1);
            foreach (var standardTask in stdRoundTaskList)
            {
                if (!standardTask.isClear)
                {
                    if (checkOnly) return false; // 보고 가능한지 확인 모드일 경우 바로 반환
                }
            }
            if (checkOnly) return true;

            // 보고서 제출 완료 팝업 UI 출력
            computer.CreateNotification(false);
            // 모든 기본 근무가 클리어된 경우에만 보고 가능
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
