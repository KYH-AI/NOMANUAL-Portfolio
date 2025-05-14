using System;
using System.Collections.Generic;
using NoManual.Managers;
using NoManual.Task;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Debug_TestUI : MonoBehaviour
{
    
    [Serializable]
    public struct PC_OS_Debug_Test
    {
       [Header("보고서 레코드 추가")] public Button addRecordBtn;
       [Header("R.S 트레이더스 제품 추가")] public Button rsTradersAddItemBtn;
       [Header("R.S 트레이더스 캐쉬 추가")] public Button rsTradersAdd_RS_MoneyBtn;
       [Header("호텔 예약자 추가")] public Button addHotelGuestBtn;
       [Header("호텔 객실 점검중 상태로 변경")] public Button setRandomHotelRoomBusyState;
       [Header("호텔 하루 넘기기")] public Button setNextDayHotelState;
       [Header("메일 추가")] public Button addMailItemBtn;
       [Header("새로운 메시지 추가")] public Button addMsgItemBtn;
       [Header("이어서 메시지 진행")] public Button continueMsgItemBtn;
    }
    
    [Serializable]
    public struct Task_Debug_Test
    {
        [Header("N일차 근무목록 발급")] public Button addNdayTaskListBtn;
        [Header("N라운드 근무목록 발급")] public Button addRoundTaskListBtn;
        [Header("추가 근무목록 발급")] public Button addBonusTaskBtn;
    }
    
    [Serializable]
    public struct DayAndRoundUI
    {
        [Header("일차")] public TextMeshProUGUI dayText;
        [Header("라운드")] public TextMeshProUGUI roundText;
    }
    
    [Serializable]
    public struct QuestListUI
    {
        [Header("일반 근무")] public TextMeshProUGUI stdText;
        [Header("추가 근무")] public TextMeshProUGUI bonusText;
        [Header("현재 일차 ANO LINK")] public TextMeshProUGUI anoLinkText;
        [Header("다음 일차 ANO LINK")] public TextMeshProUGUI nextAnoLinkText;
        [Header("시나리오 근무")] public TextMeshProUGUI scText;
        [Header("현재 일차 시나리오")] public TextMeshProUGUI currentScText;
        [Header("시나리오 엔딩")] public TextMeshProUGUI endingTypeText;
    }

    [Header("PC OS 관련 테스트")] public PC_OS_Debug_Test PCOSDebugTest = new PC_OS_Debug_Test();
    [Space(5)] 
    [Header("퀘스트 관련 테스트")] public Task_Debug_Test taskDebugTest = new Task_Debug_Test();
    [Space(5)] 
    [Header("일차 & 라운드 UI")] public DayAndRoundUI dayAndRoundUI = new DayAndRoundUI();
    [Space(5)]
    [Header("퀘스트 UI")] public QuestListUI questListUI = new QuestListUI();

    private HotelManager hotelManager;
    private Nomanual_PC_OS_Manager pcOSManager;
    
    private void Start()
    {
        
        if (!GameManager.Instance.DebugMode)
        {
            this.gameObject.SetActive(false);
            this.enabled = false;
            return;
        }
        hotelManager = HotelManager.Instance;
        if (!hotelManager)
        {
            Debug.LogError("Task 매니저 없음! Task(퀘스트) Debug UI 사용 불가능!");
        }
        else
        {
            taskDebugTest.addNdayTaskListBtn.onClick.AddListener( () => hotelManager.SetNewDayStandardTask());
            taskDebugTest.addRoundTaskListBtn.onClick.AddListener( null);
            taskDebugTest.addBonusTaskBtn.onClick.AddListener( null);
        }
        
        pcOSManager = hotelManager.PcOSManager;
        if (!pcOSManager)
        {
            Debug.LogError("PC OS 매니저 없음! PC OS Debug UI 사용 불가능!");
        }
        else
        {
            PCOSDebugTest.addRecordBtn.onClick.AddListener( () => pcOSManager.AddRecordItem("테스트 레코드"));
            PCOSDebugTest.rsTradersAddItemBtn.onClick.AddListener( () => pcOSManager.Add_RS_TradersItem(0));
            PCOSDebugTest.rsTradersAdd_RS_MoneyBtn.onClick.AddListener( () => pcOSManager.Add_RS_Cash(100));
        
            PCOSDebugTest.addHotelGuestBtn.onClick.AddListener( () => pcOSManager.AddReservationItem("예약자 이름 "));
            PCOSDebugTest.setRandomHotelRoomBusyState.onClick.AddListener( () => pcOSManager.SetRandomGuestRoomBusyState());
            PCOSDebugTest.setNextDayHotelState.onClick.AddListener( () => pcOSManager.UpdateGuestRoomState());
        
            PCOSDebugTest.addMailItemBtn.onClick.AddListener( () => pcOSManager.AddMailItem(0));
        
            PCOSDebugTest.addMsgItemBtn.onClick.AddListener( () => pcOSManager.AddMsgItem(0));
            PCOSDebugTest.continueMsgItemBtn.onClick.AddListener( () => pcOSManager.ContinueMsg(0));
        }
    }
    

    private void Update()
    {

        if (hotelManager && hotelManager.DebugMode)
        {
            SetStdText(hotelManager.GetRoundStandardTask());
            SetBonusText(hotelManager.GetRoundBonusTask());
            SetCurrentAnoLink(hotelManager.ANO.DebugGetCurrentSpawnAnoLink);
            SetNextAnoLink(hotelManager.ANO.SaveAnoLink());
            SetCurrentScTextAndEndingTypeText(hotelManager.TaskHandler.GetDebugModeScTaskInfo, hotelManager.TaskHandler.Scenario.LastClearScType);
        }
    }


    public void SetDayText(int day)
    {
        dayAndRoundUI.dayText.text = "Day : " + day;
       
    }
    
    public  void SetRoundText(int round)
    {
          
        dayAndRoundUI.roundText.text = "Round : " + round;
     
    }
    
    public  void SetStdText(List<TaskHandler.StandardTask> std)
    {
   
        string titleText = "Std Task : ";
        if (std == null)
        {
            questListUI.stdText.text = titleText;
            return;
        }
  
   
        string resultText = string.Empty;
        foreach (var stdTask in std)
        {
            resultText += GetResultText(stdTask.taskTableId.ToString(), stdTask.taskData.taskTargetId, stdTask.isClear);
        }

        titleText += resultText;
        questListUI.stdText.text = titleText;
      
    }
    
    public  void SetBonusText(List<TaskHandler.BonusTask> bonus)
    {
   
        string titleText = "Bonus Task : ";
        if (bonus == null)
        {
            questListUI.bonusText.text = titleText;
            return;
        }
        
        string resultText = string.Empty;
        foreach (var bonusTask in bonus)
        {
            resultText += GetResultText(bonusTask.taskId, bonusTask.isClear);
        }

        titleText += resultText;
        questListUI.bonusText.text = titleText;
     
    }

    public void SetCurrentAnoLink(Dictionary<int, int[]> anoLink)
    {
           
        string titleText = "Current ANO Link : ";
        if (anoLink == null)
        {
            questListUI.anoLinkText.text = titleText;
            return;
        }
        
        string resultText = string.Empty;
        foreach (var ano in anoLink)
        {
            resultText += GetResultText(ano.Key.ToString(), ano.Value);
        }

        titleText += resultText;
        questListUI.anoLinkText.text = titleText;
      
    }

    public void SetNextAnoLink(int[] anoLink)
    {
           
        string titleText = "Next ANO Link : ";
        if (anoLink == null)
        {
            questListUI.nextAnoLinkText.text = titleText;
            return;
        }
        
        string resultText = string.Empty;
        foreach (var id in anoLink)
        {
            resultText += GetResultText(id.ToString());
        }
        titleText += resultText;
        questListUI.nextAnoLinkText.text = titleText;
     
    }

    private Dictionary<int, bool> _roundScTask = new Dictionary<int, bool>();
    public void SetScText(int scRootId, bool isClear)
    {
           
        string titleText = "SC Task : ";
        if (scRootId == -1)
        {      
            _roundScTask.Clear();
            questListUI.scText.text = titleText;
            return;
        }
        
        _roundScTask[scRootId] = isClear;
        string resultText = string.Empty;
        foreach (var scTask in _roundScTask)
        {
            resultText += GetResultText(scTask.Key.ToString(), scTask.Value);
        }

        titleText += resultText;
        questListUI.scText.text = titleText;
        
    }

    public void SetCurrentScTextAndEndingTypeText(TaskHandler.ScenarioTaskInfo sc, EndingType endingType)
    {

        string titleText = "Current SC Task : ";
        string resultText = string.Empty;
        questListUI.endingTypeText.text = "Ending Type : " + endingType.ToString();
        if (sc == null)
        {
            questListUI.currentScText.text = titleText;
            return;
        }
        Dictionary<int, List<int>> dayScInfo = new Dictionary<int,List<int>>();
        foreach (var scRndTaskList in sc.scenarioRoundTaskList)
        {
            foreach (var scTask in scRndTaskList.scenarioTaskList)
            {
                if (scTask.ProcessType == ProcessType.Start || scTask.EndingType == endingType)
                {
                    if (!dayScInfo.ContainsKey(scTask.RootRound))
                    {
                        dayScInfo[scTask.RootRound] = new List<int>();
                    }
                    dayScInfo[scTask.RootRound].Add(scTask.RootId);
                }
            }
        }
        
        foreach (var scTask in dayScInfo)
        {
            resultText += GetResultText(scTask.Key.ToString(), scTask.Value.ToArray());
        }

        titleText += resultText;
        questListUI.currentScText.text = titleText;

    }

    public void SetEndingTypeText(string endingType)
    {
        string titleText = "Ending Type : " + endingType;
        questListUI.endingTypeText.text = titleText;
    }

    private string GetResultText(string id, bool isClear)
    {
        if (isClear)
        {
            return $"<color=green>{id}</color>, ";
        }
        else
        {
            return $"<color=red>{id}</color>, ";
        }
    }
    
    private string GetResultText(string id, string targetId, bool isClear)
    {
        if (isClear)
        {
            return $"<color=green>{id}({targetId})</color>, ";
        }
        else
        {
            return $"<color=red>{id}({targetId})</color>, ";
        }
    }
    
    private string GetResultText(string round, int[] ids)
    {
        // ids 배열을 쉼표로 구분하여 문자열로 변환
        string idText = string.Join(",", ids);
        return $"Round[{round}]ID[{idText}], ";
    }
    
    
    private string GetResultText(string id)
    {
        return $"{id}, ";
    }
}
