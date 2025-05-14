using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NoManual.UI
{
    /// <summary>
    /// PC OS에 있는 보고서 시스템
    /// </summary>
    public class UI_Report : MonoBehaviour
    {
        [Serializable]
        public struct ReportPanel
        {
            [Header("보고서 패널")] public GameObject reportPanel;
            [Header("보고서 보고내용 리스트 Root")] public UI_ReportRecordListSlot reportListPanel;
            [Header("보고서 체크박스")] public Toggle reportConfirmChkButton;
            [Header("보고서 확인 버튼")] public Button reportSendButton;
            [Header("보고서 취소 버튼")] public Button reportCancelButton;
        } 
        
        [Serializable]
        public struct RecordPanel
        {
            [Header("레코드 패널")] public GameObject recordPanel;
            [Header("레코드 아이템 리스트 Root")] public UI_ReportRecordListSlot recordListPanel;
            [Header("레코드 보고 작성 버튼")] public Button recordAllReportButton;
            [Header("레코드 보고 취소 버튼")] public Button recordAllDenyButton;
            [Header("레코드 아이템 프리팹")] public GameObject recordItem;
        }

        [Space(20)]
        public ReportPanel report = new ReportPanel();
        [Space(20)]
        public RecordPanel record = new RecordPanel();
        [Space(20)]
        [SerializeField] private GameObject reportPopup;
        
        // 레코드 아이템 리스트 정보
        private readonly List<IDragItem> _recordPanelInRecordItemList = new List<IDragItem>(8);
        private readonly List<IDragItem> _reportPanelInRecordItemList = new List<IDragItem>(8);
        // 보고서 제출 버튼 활성화 이벤트
        private UnityAction _reportSubmitEnableEvent;

        [Space(50)]
        [Header("보고서 제출취소 이벤트")] public UnityEvent reportCancelEvent;

        public delegate void UpdateRecordEventHandler();
        public delegate bool CanReportHandler(bool checkOnly);
        private event CanReportHandler _canReportHandler;
        
        private void Awake()
        { 
            // 레코드 Slot Drop 이벤트 등록
           record.recordListPanel.recordOnDropEvent.AddListener(MoveRecordItem);
           report.reportListPanel.recordOnDropEvent.AddListener(MoveRecordItem);
           
           // 레코드 패널 버튼 이벤트 등록
           record.recordAllReportButton.onClick.AddListener(RecordAllReport);
           record.recordAllDenyButton.onClick.AddListener(RecordAllDeny);
           
           // 보고서 패널 버튼 이벤트 등록
           report.reportSendButton.onClick.AddListener(ReportSubmit);
           report.reportCancelButton.onClick.AddListener(ReportCancel);

           // 보고서 제출 버튼 및 서약서 체크박스 비활성화
           report.reportConfirmChkButton.isOn = false;
           report.reportSendButton.interactable = false;
           
           // 보고서 제출 버튼 활성화 이벤트 등록
           _reportSubmitEnableEvent -= UpdateReportSubmitButton;
           _reportSubmitEnableEvent += UpdateReportSubmitButton;
           
           // 보고서 서약 체크박스 이벤트 등록
           report.reportConfirmChkButton.onValueChanged.AddListener(OnReportCheckBoxToggleChanged);
           
           // 보고서 제출 버튼 초기화
           UpdateReportSubmitButton();
           
           reportPopup.SetActive(false);
        }

        #region 이벤트

        public void InitializationReportEvent(CanReportHandler report)
        {
            _canReportHandler -= report;
            _canReportHandler += report;
        }
        
        /// <summary>
        /// 레코드 리스트 내용을 모두 리포트에게 전달
        /// </summary>
        private void RecordAllReport()
        {
            if (_recordPanelInRecordItemList.Count <= 0) return;
            
            foreach (var recordItem in _recordPanelInRecordItemList)
            {
                recordItem.SetParentAndPosition(report.reportListPanel.GetDropSlotTransform());
                _reportPanelInRecordItemList.Add(recordItem);
            }
            _recordPanelInRecordItemList.Clear();
            _reportSubmitEnableEvent?.Invoke();
        }

        /// <summary>
        /// 리포트에 있는 레코드 모두를 레코드 리스트에 전달
        /// </summary>
        private void RecordAllDeny()
        {
            if (_reportPanelInRecordItemList.Count <= 0) return;
            
            foreach (var recordItem in _reportPanelInRecordItemList)
            {
                recordItem.SetParentAndPosition(record.recordListPanel.GetDropSlotTransform());
                _recordPanelInRecordItemList.Add(recordItem);
            }
            _reportPanelInRecordItemList.Clear();
            _reportSubmitEnableEvent?.Invoke();
        }

        /// <summary>
        /// 보고서 제출 버튼
        /// </summary>
        private void ReportSubmit()
        {
            // 서약체크 확인
            if (!report.reportConfirmChkButton.isOn) return;
            // 보고서 제출
            _canReportHandler?.Invoke(false);
            // 모든 레코드 패널 초기화
            InitializationRecordPanel();
            InitializationReportPanel();
            report.reportConfirmChkButton.isOn = false;
            reportPopup.SetActive(true);
        }

        /// <summary>
        /// 보고서 취소 버튼
        /// </summary>
        public void ReportCancel()
        {
            // 보고서 제출 버튼 및 서약서 체크박스 비활성화
            report.reportConfirmChkButton.isOn = false;
            
            // 레코드 원상복구
            RecordAllDeny();
            
            // 보고서 앱 종료
            reportCancelEvent?.Invoke();
            
            reportPopup.SetActive(false);
        }
        
        /// <summary>
        /// 체크박스 갱신 이벤트 핸들러
        /// </summary>
        private void OnReportCheckBoxToggleChanged(bool isOn)
        {
            UpdateReportSubmitButton();
        }

        /// <summary>
        /// 보고서 제출 버튼 상태 업데이트
        /// </summary>
        private void UpdateReportSubmitButton()
        {
            // CanReport 허가도 받아서 제출 가능 여부 결정
            bool canSubmit = (_canReportHandler?.Invoke(true) ?? false) // 델리게이트 호출 결과
                             && _recordPanelInRecordItemList.Count == 0  // 기록 항목이 없는지 확인
                             && report.reportConfirmChkButton.isOn;      // 확인 체크박스가 켜져 있는지 확인
    
            // 제출 버튼의 활성화 상태 업데이트
            report.reportSendButton.interactable = canSubmit;
        }


        #endregion

        #region 레코드 CRUD

        /// <summary>
        /// 레코드 아이템 동적 생성
        /// </summary>
        public void CreateRecordItem(string title)
        {
            var item = Instantiate(record.recordItem, record.recordListPanel.GetDropSlotTransform());
            var recordItem = item.GetComponent<UI_RecordItem>();
            recordItem.InitializeRecord(title, report.reportPanel.transform);
            AddRecordItem(recordItem, true);
        }

        /// <summary>
        /// 레코드 아이템 퀘스트 클리어에 알맞게 생성 (호텔 맵)
        /// </summary>
        public void CreateRecordItem_Hotel()
        {
            InitializationRecordPanel();
            InitializationReportPanel();
            
            var stdTaskList = Managers.HotelManager.Instance.GetRoundStandardTask();
            var bonusTaskList =  Managers.HotelManager.Instance.GetRoundBonusTask();

            if (stdTaskList != null && stdTaskList.Count != 0)
            {
                foreach (var std in stdTaskList)
                {
                    if(!std.isClear) continue;
                    CreateRecordItem(std.taskDescription);
                }
            }
            if (bonusTaskList != null && bonusTaskList.Count != 0)
            {
                foreach (var bonus in bonusTaskList)
                {
                    if(!bonus.isClear) continue;
                    CreateRecordItem(bonus.taskDescription);
                }
            }
            if (report.reportConfirmChkButton.isOn) report.reportConfirmChkButton.isOn = false;
        }

        /// <summary>
        /// 레코드 아이템 퀘스트 클리어에 알맞게 생성 (인수인계 맵)
        /// </summary>
        public void CreateRecordItem_Handover()
        {
            InitializationRecordPanel();
            InitializationReportPanel();
            
            var stdTaskList = Tutorial.HandoverManager.Instance.TaskHandler.GetRoundStandardTask(1);
            var bonusTaskList = Tutorial.HandoverManager.Instance.TaskHandler.GetBonusTask(1);

            if (stdTaskList != null && stdTaskList.Count != 0)
            {
                foreach (var std in stdTaskList)
                {
                    if(!std.isClear) continue;
                    CreateRecordItem(std.taskDescription);
                }
            }
            if (bonusTaskList != null && bonusTaskList.Count != 0)
            {
                foreach (var bonus in bonusTaskList)
                {
                    if(!bonus.isClear) continue;
                    CreateRecordItem(bonus.taskDescription);
                }
            }
            if (report.reportConfirmChkButton.isOn) report.reportConfirmChkButton.isOn = false;
        }
        
        /// <summary>
        /// 레코드 아이템 데이터 Drag & Drop으로 이동 
        /// </summary>
        private void MoveRecordItem(IDragItem recordItem, IDropSlot dropSlot)
        {
            // 원래 Slot에 Drop 한 경우 return
            if (recordItem.GetCurrentParent() == dropSlot.GetDropSlotTransform())
            {
                return;
            }

            bool isRecordList;
            // 레코드 Slot에 Drop 한 경우
            if (!_recordPanelInRecordItemList.Contains(recordItem))
            {
                isRecordList = true;
            }
            // 보고서 Slot에 Drop 한 경우
            else
            {
                isRecordList = false;
            }
            
            AddRecordItem(recordItem, isRecordList);
            RemoveRecordItem(recordItem, !isRecordList);
        }

        /// <summary>
        /// 레코드 아이템 추가
        /// </summary>
        private void AddRecordItem(IDragItem recordItem, bool isRecordList)
        {
            if (isRecordList)
                _recordPanelInRecordItemList.Add(recordItem);
            else 
                _reportPanelInRecordItemList.Add(recordItem);
            
            _reportSubmitEnableEvent?.Invoke();
        }

        /// <summary>
        /// 레코드 아이템 제거
        /// </summary>
        private void RemoveRecordItem(IDragItem recordItem, bool isRecordList)
        {
            if (isRecordList)
                _recordPanelInRecordItemList.Remove(recordItem);
            else 
                _reportPanelInRecordItemList.Remove(recordItem);
            
            _reportSubmitEnableEvent?.Invoke();
        }

        /// <summary>
        /// 레코드 패널 초기화
        /// </summary>
        private void InitializationRecordPanel()
        {
            record.recordListPanel.AllRemoveRecordItem();
            _recordPanelInRecordItemList.Clear();
            
            _reportSubmitEnableEvent?.Invoke();
        }

        /// <summary>
        /// 보고서 패널 초기화
        /// </summary>
        private void InitializationReportPanel()
        {
            report.reportListPanel.AllRemoveRecordItem();
            _reportPanelInRecordItemList.Clear();
            
            _reportSubmitEnableEvent?.Invoke();
        }

        #endregion

        
        
    }
}


