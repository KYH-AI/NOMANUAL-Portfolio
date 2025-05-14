using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NoManual.Managers;
using NoManual.UI;
using NoManual.Utils;

namespace NoManual.Task
{
    
    /// <summary>
    /// 호텔 씬에서 진행되는 메인 퀘스트
    /// </summary>
    public class TaskHandler
    {
        // 퀘스트 타입
        public enum TaskType
        {
            None = -1,
            
            PopUp = 0, // UI 뛰우기
            Patrol = 1, // 순찰
            Interaction = 2, // 상호작용
            Get = 3, // 아이템 얻기
            Put = 4, // 아이템 버리기
            TradersBuy = 5, // R.S 트레이더스 쇼핑
            Reserve = 6, // 객실 예약
            ANO = 7, // ANO (Task Test 씬 디버깅용)
        }

        // 퀘스트 ID
        public enum TaskID
        {
            None = -1,
            
            PC_Connect,
            App_Connect,
            
            Patrol_Floor_Field,
            Patrol_Facility_Field, // 번역 csv는 Object Table 이용
            Patrol_Room_Field,

            Interact_Object,

            Put_Inventory_Item_Return,
            Put_Inventory_Item_Relay,
            Put_Inventory_Lock_Door,
            
            Get_Inventory_Item,
            Get_Delivery_Item,
            
            Traders_Buy,
            
            Reserve_All_Clear,
            Reserve_Target_Clear,
        }
        
        
        // 기본근무 Json 파일 경로
        private readonly string _STANDARD_TASK_JSON_PATH = "JsonData/CheckList/Standard_Task_List";
        // 시나리오 Json 파일 경로
        private readonly string _SCENARIO_TASK_JSON_PATH = "JsonData/Scenario/Scenario_Task_List";
        // 인수인계 근무 Json 파일 경로
        private readonly string _HANDOVER_STANDARD_TASK_JSON_PATH = "JsonData/CheckList/Handover_Standard_Task_List";

        // 모든 라운드 근무목록 원본 데이터 정보
        private readonly Dictionary<int, TaskInfo> _allStandardTaskList = new Dictionary<int, TaskInfo>();
        // 현재 N일차 근무목록
        public TaskInfo CurrentStandardTask { get; private set; }
        // 현재 라운드 근무목록 
        private StandardRoundTask _currentRoundTask;
        // 현재 라운드 Std 근무목록 리스트 캐싱
        private List<StandardTask> _currentRoundStdTaskList;
        
        
        public ScenarioHandler Scenario { get; private set; } = new ScenarioHandler();
        // 시나리오 원본 데이터 정보
        private readonly Dictionary<int, ScenarioTaskInfo> _allScenarioTaskList = new Dictionary<int, ScenarioTaskInfo>();
        private ScenarioTaskInfo _currentScenarioTask;

        public ScenarioTaskInfo GetDebugModeScTaskInfo => _currentScenarioTask;

        private ScenarioRoundTask _currentScenarioRoundTask;

        // 마지막 라운드 캐싱
        private int _lastRound = 0;

        // 일차에 클리어 된 ANO 개수 (정산용)
        public int DayClearAnoCount { get; private set; } = 0;

        #region 퀘스트 레코드 업데이트 이벤트

        private event UI_Report.UpdateRecordEventHandler _updateRecordEventHandler;

        #endregion
        
        #region 퀘스트 클리어 Delegate 정의

        // 퀘스트 클리어 이벤트 Delegate 정의
        public delegate void TaskEventHandler(string taskId, string taskTargetId);
        public delegate void PatrolEventHandler(string taskId, string taskTargetId, UI_GuestRoomItem.RoomState roomState);
        public delegate void PutTaskEventHandler(string taskId, string taskTargetId, string putRequireItemId);
        
        // Get_Delivery_Item, Reserve_All_Clear 같은 특수 케이스
        public delegate void ManualTaskEventHandler(TaskType taskType, string taskId, string taskTargetId);
        
        // 추가근무 퀘스트 클리어 이벤트
        public delegate void BonusTaskEventHandler(TaskType taskType, string taskId);
        

        #endregion

        #region 퀘스트 초기화
        
        /// <summary>
        /// 근무목록 리스트 읽어오기 
        /// </summary>
        public void TaskHandlerInitialization(UI_Report.UpdateRecordEventHandler recordUpdate, bool isHotel = true)
        {
            _updateRecordEventHandler -= recordUpdate;
            _updateRecordEventHandler += recordUpdate;
            ReadTaskFile(isHotel);
            
            _lastRound = 0;
            DayClearAnoCount = 0;
        }
        
        /// <summary>
        ///  기본근무 Task Json 파일 읽기
        /// </summary>
        private void ReadTaskFile(bool isHotel)
        {
            /*  JSON 추상화 클래스를 역직렬화  참조 : https://appetere.com/blog/serializing-interfaces-with-jsonnet/ */
            
            // 기본근무 데이터 읽기
            List<TaskInfo> newStandardTasks =  JsonFileHelper.ReadJsonAssetFile<List<TaskInfo>>(isHotel ? _STANDARD_TASK_JSON_PATH : _HANDOVER_STANDARD_TASK_JSON_PATH, JsonTaskConvertSetting.GetTaskConvert());
            if (newStandardTasks != default)
            {
                foreach (var task in newStandardTasks)
                {
                    // 로컬라이징 및 Place Holder 설정
                    foreach (var standardRoundTaskList in task.standardRoundTaskList)
                    {
                        foreach (var stdTask in standardRoundTaskList.standardTaskList)
                        {
                            // 로컬라이징 데이터 얻기
                            stdTask.taskDescription = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.Standard_Task_Item_Table, 
                                LocalizationTable.Standard_Task_Item_TableTextKey.Standard_Task_.ToString() + stdTask.taskTableId);
                            // Place Holder 설정
                            if (stdTask.placeHolder && !stdTask.runtimeTask)
                            {
                                string taskTargetId = stdTask.taskData.taskTargetId;
                                if (stdTask.taskData is PutTask putTask)
                                {
                                    // {Item_ID} 물품보관함에 반환
                                    if( putTask.taskId.Equals(TaskID.Put_Inventory_Item_Return.ToString())) taskTargetId = putTask.putRequireItemId[0];
                                }
                                string placeHolderStr = TaskPlaceHolderMapper(taskTargetId, 
                                                                    stdTask.taskData.GetTaskTargetLocalization(),
                                                                                            stdTask.placeHolderTextTableKey);
                                stdTask.taskDescription = stdTask.taskDescription.Replace(LocalizationVariableHelper.GetPlaceHolderString(stdTask.taskDescription), placeHolderStr);
                            }
                        }
                    }
                    // 기본 근무 리스트 저장
                    _allStandardTaskList.Add(task.targetNday, task);
                }
            }

            if (!isHotel) return;
            
            // 시나리오 데이터 읽기
            List<ScenarioTaskInfo> newScenarioTasks =  JsonFileHelper.ReadJsonAssetFile<List<ScenarioTaskInfo>>(_SCENARIO_TASK_JSON_PATH, JsonTaskConvertSetting.GetTaskConvert());
            if (newScenarioTasks != default)
            {
                foreach (var scenarioTaskInfo in newScenarioTasks)
                {
                    _allScenarioTaskList.Add(scenarioTaskInfo.targetNday, scenarioTaskInfo);
                    foreach (var scRoundTaskList in scenarioTaskInfo.scenarioRoundTaskList)
                    {
                        foreach (var scenarioTask in scRoundTaskList.scenarioTaskList)
                        {
                            scenarioTask.RootDay = scenarioTaskInfo.targetNday;
                            scenarioTask.RootRound = scRoundTaskList.targetRound;
                        }
                    }
                }
            }
        }
        
     
        
        #endregion

        #region 퀘스트 데이터 설정
        
        /// <summary>
        /// Task Place Holder 설정
        /// </summary>
        private string TaskPlaceHolderMapper(string targetPlaceHolder, string targetPlaceHolderSmartStringKey, LocalizationTable.TextTable smartStringTableKey)
        {
            string placeHolderString = string.Empty;
            
            // 로컬라이징 Place Holder 매핑
            if (!NoManualUtilsHelper.FindStringEmptyOrNull(targetPlaceHolder))
            {
                if (smartStringTableKey != LocalizationTable.TextTable.None)
                    placeHolderString = GameManager.Instance.localizationTextManager.GetText(smartStringTableKey, targetPlaceHolderSmartStringKey + targetPlaceHolder);
                else
                    placeHolderString = targetPlaceHolder;
            }
            // 런타임 도중에 등록을 위해 Place Holder 유지
            else 
            {
                // 아래 경우가 아니면 게임 런타임 중에 Place Holder 등록 (ex. {room_number}호실 점검)
                if (targetPlaceHolder != "" || targetPlaceHolder != string.Empty)
                {
                    placeHolderString = targetPlaceHolder;
                }
            }
            return placeHolderString;
        }

        /// <summary>
        /// 런타임 근무목록 데이터 설정 (Patrol_Room_Field) 
        /// </summary>
        public void SetRunTimeStdTaskData(int currentRound, TaskType taskType, string taskId, string taskTargetId)
        {
            foreach (var standardRoundTask in CurrentStandardTask.standardRoundTaskList)
            {
                if (standardRoundTask.targetRound == currentRound)
                {
                    foreach (var task in standardRoundTask.standardTaskList)
                    {
                        if (task.taskType == taskType && task.runtimeTask && task.taskData.taskId.Equals(taskId))
                        {
                            // 목표 ID 설정
                            task.taskData.taskTargetId = taskTargetId;
                            // Place Holder 텍스트 설정
                            if (task.placeHolder)
                            {
                                if (task.taskData is PutTask putTask)
                                {
                                    if(putTask.taskId.Equals(TaskID.Put_Inventory_Item_Relay.ToString())) taskTargetId = putTask.putRequireItemId[0];
                                }
                                string placeHolderStr = TaskPlaceHolderMapper(task.taskData.taskTargetId, 
                                                                    task.taskData.GetTaskTargetLocalization(),
                                                                                            task.placeHolderTextTableKey);
                                task.taskDescription = task.taskDescription.Replace(LocalizationVariableHelper.GetPlaceHolderString(task.taskDescription), placeHolderStr);
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// 런타임 보너스 근무목록 데이터 초기화 (라운드 호출)
        /// </summary>
        public void ResetBonusTaskData()
        {
            _currentRoundTask?.bonusTaskList.Clear();
        }

        /// <summary>
        /// 런타임 보너스 근무목록 데이터 생성 1
        /// </summary>
        public void SetRunTimeBonusTaskData(TaskType taskType, string taskId)
        {
            if (_currentRoundTask == null) return;
            
            // 중복 확인
            if (ContainsBonusTask(taskType, taskId)) return;
            CreateBonusTaskData(taskType, taskId);
        }
        
        /// <summary>
        /// 런타임 보너스 근무목록 데이터 생성 2
        /// </summary>
        private void CreateBonusTaskData(TaskType taskType, string taskId)
        {
            BonusTask bonusTask = new BonusTask(taskType, taskId)
            {
                /*
                taskDescription = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.Bonus_Task_Item_Table, 
                 LocalizationTable.Bonus_Item_TableTextKey.Bonus_Task_.ToString() + taskId),
                */
                taskDescription = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.Standard_Task_Item_Table,
                                                                                    LocalizationTable.Standard_Task_Item_TableTextKey.Standard_Task_ + 15.ToString())
            };
            
            _currentRoundTask.bonusTaskList.Add(bonusTask);
        }

        /// <summary>
        /// 보너스 근무목록 중복확인
        /// </summary>
        private bool ContainsBonusTask(TaskType taskType, string taskId)
        {
            foreach (var bonusTaskData in _currentRoundTask.bonusTaskList)
            {
                if (bonusTaskData.taskType == taskType && 
                    bonusTaskData.taskId.Equals(taskId))
                {
                    return true;
                }
            }
            
            return false;
        }

        #endregion
        
        /// <summary>
        /// N일차 근무목록 발급
        /// </summary>
        public void SetNewDayStandardTask(int currentDay, bool isHotel = true)
        {
            /* Std 일차 근무목록 */
            CurrentStandardTask = _allStandardTaskList[currentDay];
            // 라운드 근무목록 1 라운드로 초기화
            _currentRoundTask = CurrentStandardTask.standardRoundTaskList.FirstOrDefault(roundTask => roundTask.targetRound == 1);
            _currentRoundStdTaskList = null;

          //  if (currentDay == 0) return; // 시나리오 0일차 예외처리 (25.01.13) 주석 해체

              if (isHotel)
              {
                  /* 시나리오 일차 */
                  _currentScenarioTask = _allScenarioTaskList[currentDay];
                  _currentScenarioRoundTask = null;
              }
        }

        /// <summary>
        /// 기본, 추가 근무 Task 정보 라운드 업데이트 확인
        /// </summary>
        public void CheckRoundTask(int currentRnd)
        {
            // 캐싱된 라운드와 현재 라운드가 다를 때만 작업 목록을 업데이트
            if (currentRnd != _lastRound || _currentRoundStdTaskList == null)
            {
                // 현재 라운드에 해당하는 작업 목록을 검색
                foreach (var stdRndTask in CurrentStandardTask.standardRoundTaskList)
                {
                    if (stdRndTask.targetRound == currentRnd)
                    {
                        // 라운드 정보를 업데이트
                        _lastRound = currentRnd;
                        _currentRoundTask = stdRndTask;
                        _currentRoundStdTaskList = _currentRoundTask.standardTaskList;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 해당 라운드 근무목록 정보 얻기 
        /// </summary>
        public List<StandardTask> GetRoundStandardTask(int currentRnd)
        {
            // _currentStandardTask 또는 해당 라운드의 작업 목록이 없는 경우 null을 반환
            if (CurrentStandardTask?.standardRoundTaskList == null) return null;
            CheckRoundTask(currentRnd);
            return _currentRoundStdTaskList;
        }
        
        /// <summary>
        /// 해당 라운드 시나리오 정보 얻기 
        /// </summary>
        public List<ScenarioTask> GetScenarioTask(int currentRnd)
        {
            // 라운드의 작업 목록이 없는 경우 null을 반환
            if (_currentScenarioTask?.scenarioRoundTaskList == null) return null;
            
            // 캐싱된 라운드와 현재 라운드가 다를 때만 작업 목록을 업데이트
            if (currentRnd != _lastRound || _currentScenarioRoundTask == null || _currentScenarioRoundTask.targetRound != currentRnd)
            {
                bool thisRoundNoScTask = true;
                // 현재 라운드에 해당하는 작업 목록을 검색
                foreach (var scenarioRoundTask in _currentScenarioTask.scenarioRoundTaskList)
                {
                    if (scenarioRoundTask.targetRound == currentRnd)
                    {
                        // 라운드 정보를 업데이트
                        _lastRound = currentRnd;
                        _currentScenarioRoundTask = scenarioRoundTask;
                        thisRoundNoScTask = false;
                        break;
                    }
                }
                // 해당 라운드에 시나리오 퀘스트가 없으면 null 처리
                if (thisRoundNoScTask) _currentScenarioRoundTask = null;
            }
            return _currentScenarioRoundTask?.scenarioTaskList;
        }

        /// <summary>
        ///  현재 추가근목 목록 정보 얻기
        /// </summary>
        public List<BonusTask> GetBonusTask(int currentRnd)
        {
            if (CurrentStandardTask == null || _currentRoundTask?.bonusTaskList == null || _currentRoundTask.bonusTaskList.Count == 0) return null;
            CheckRoundTask(currentRnd);
            return _currentRoundTask.bonusTaskList;
        }

        #region 퀘스트 클리어 핸들러
        
        /// <summary>
        /// 퀘스트 클리어 조건 확인
        /// </summary>
        private bool CheckTaskData(StandardTask standardTaskData, TaskType targetType, string targetTaskId, string targetTargetId)
        {
            var taskData = standardTaskData.taskData;
            // taskData의 taskTargetId가 비어있거나, 이미 클리어된 상태이고 비교 타입이 아닌 경우
            if (NoManualUtilsHelper.FindStringEmptyOrNull(taskData.taskTargetId) || standardTaskData.taskType != targetType || standardTaskData.isClear) 
            {
                return false;
            }
            // taskData의 taskId가 targetTaskId 일치하지 않고, taskData의 taskTargetId가 targetTargetId와 일치하지 않는 경우
            if (!taskData.taskId.Equals(targetTaskId) || !taskData.taskTargetId.Equals(targetTargetId)) 
            {
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// Std Task 클리어 처리 메서드
        /// </summary>
        private void ProcessTaskCheckHandler(TaskType taskType, string taskId, string taskTargetId, bool clearAlarmUi = true)
        {
            List<StandardTask> standardTasks = GetRoundStandardTask(_lastRound);
            if(standardTasks == null) return;
            foreach (var standardTask in standardTasks)
            {
                // 기본 클리어 조건 확인
                if (!CheckTaskData(standardTask, taskType, taskId, taskTargetId)) continue;
                if(clearAlarmUi) NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(standardTask.taskDescription, LocalizationTable.CheckList_UI_TableTextKey.Check_List_Clear);
                // 조건을 통과한 경우 isClear를 true로 설정
                standardTask.isClear = true;
                _updateRecordEventHandler?.Invoke();
                break;
            }
            ScenarioTaskCheckHandler(taskType, taskId, taskTargetId);
        }

        /// <summary>
        /// 시나리오 클리어 처리 메서드
        /// </summary>
        private void ScenarioTaskCheckHandler(TaskType taskType, string taskId, string taskTargetId)
        {
            List<ScenarioTask> scenarioTasks = GetScenarioTask(_lastRound);
            if(scenarioTasks == null) return;
            foreach (var scenario in scenarioTasks)
            {
                // 기본 클리어 조건 확인
                if (!CheckTaskData(scenario.ScTask, taskType, taskId, taskTargetId)) continue;
                
                // 조건을 통과한 경우 isClear를 true로 설정
                scenario.ScTask.isClear = true;
                Scenario.ScenarioTaskClear(scenario.EndingType, scenario.ProcessType, scenario.RootId);
                break;
            }
        }

        /// <summary>
        /// Bonus 관련 Task 클리어 처리 메서드
        /// </summary>
        private void BonusCheckHandler(TaskType taskType, string taskId)
        {
            List<BonusTask> bonusTasks = GetBonusTask(_lastRound);
            if (bonusTasks == null) return;
            foreach (var bonusTask in bonusTasks)
            {
                // 기본 클리어 조건 확인
                if (bonusTask.taskType != taskType || !bonusTask.taskId.Equals(taskId)) continue;
                
                // 조건을 통과한 경우 isClear를 true로 설정
                bonusTask.isClear = true;
                DayClearAnoCount++;
                _updateRecordEventHandler?.Invoke();
                break;
            }
        }

        /// <summary>
        /// 수동 퀘스트 클리어 (특수 케이스 string.empty 무시)
        /// </summary>
        public void ManualTaskClearHandler(TaskType taskType, string taskId, string taskTargetId)
        {
            List<StandardTask> standardTasks = GetRoundStandardTask(_lastRound);
            foreach (var standardTask in standardTasks)
            {
                // 기본 조건: Task가 이미 완료된 상태라면 건너뜀
                if (standardTask.isClear) continue;

                // Task 타입, ID, 타겟 ID가 모두 일치하는지 확인
                if( standardTask.taskType == taskType &&
                    standardTask.taskData.taskId == taskId &&
                    standardTask.taskData.taskTargetId == taskTargetId)
                { 
                    NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(standardTask.taskDescription, LocalizationTable.CheckList_UI_TableTextKey.Check_List_Clear);
                    standardTask.isClear = true;  // 조건을 통과한 경우 isClear를 true로 설정
                    _updateRecordEventHandler?.Invoke();
                    break;
                }
            }
            
            // 시나리오 퀘스트 클리어
            List<ScenarioTask> scenarioTasks = GetScenarioTask(_lastRound);
            if(scenarioTasks == null) return;
            foreach (var scenario in scenarioTasks)
            {
                if(scenario.ScTask.isClear) continue;
                
                if( scenario.ScTask.taskType == taskType &&
                    scenario.ScTask.taskData.taskId == taskId &&
                    scenario.ScTask.taskData.taskTargetId == taskTargetId)
                { 
                    scenario.ScTask.isClear = true;  // 조건을 통과한 경우 isClear를 true로 설정
                    Scenario.ScenarioTaskClear(scenario.EndingType, scenario.ProcessType, scenario.RootId);
                    break;
                }
            }
        }
        
        /// <summary>
        /// Put Task
        /// </summary>
        public void PutTaskCheckHandler(string taskId, string taskTargetId, string putRequireItemId)
        {
            List<StandardTask> standardTasks = GetRoundStandardTask(_lastRound);
            if(standardTasks == null) return;
            foreach (var standardTask in standardTasks)
            {
                // 퀘스트 클리어 기본조건 확인
                if(!CheckTaskData(standardTask, TaskType.Put, taskId, taskTargetId)) continue;

                if (standardTask.taskData is not PutTask putTaskData) continue;
                foreach (var itemId in putTaskData.putRequireItemId)
                { 
                    // 조건을 통과한 경우 isClear를 true로 설정
                    if (putRequireItemId.Equals(itemId))
                    {
                        NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(standardTask.taskDescription, LocalizationTable.CheckList_UI_TableTextKey.Check_List_Clear);
                        standardTask.isClear = true;
                        _updateRecordEventHandler?.Invoke();
                        break;
                    }
                }
                // 중복 퀘스트 허용
                if (standardTask.isClear) break;
            }
            
            // 시나리오 퀘스트 클리어
            List<ScenarioTask> scenarioTasks = GetScenarioTask(_lastRound);
            if(scenarioTasks == null) return;
            foreach (var scenario in scenarioTasks)
            {
                if (!CheckTaskData(scenario.ScTask, TaskType.Put, taskId, taskTargetId)) continue;
                
                if(scenario.ScTask.taskData is not PutTask putTaskData) continue;
                foreach (var itemId in putTaskData.putRequireItemId)
                {
                    if (putRequireItemId.Equals(itemId))
                    {
                        scenario.ScTask.isClear = true;
                        Scenario.ScenarioTaskClear(scenario.EndingType, scenario.ProcessType, scenario.RootId);
                        break;
                    }
                }
            }
        }
        
        /// <summary>
        /// Patrol Task
        /// </summary>
        public void PatrolTaskCheckHandler(string taskId, string taskTargetId, UI_GuestRoomItem.RoomState roomState)
        {
            List<StandardTask> standardTasks = GetRoundStandardTask(_lastRound);
            if(standardTasks == null) return;
            foreach (var standardTask in standardTasks)
            {
                // 퀘스트 클리어 기본조건 확인
                if(!CheckTaskData(standardTask, TaskType.Patrol, taskId, taskTargetId)) continue;

                if (standardTask.taskData is not PatrolTask patrolTask) continue;
                if (patrolTask.roomState == roomState)
                {
                    NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(standardTask.taskDescription, LocalizationTable.CheckList_UI_TableTextKey.Check_List_Clear);
                    standardTask.isClear = true;
                    _updateRecordEventHandler?.Invoke();
                    break;
                }
            }    
            
            // 시나리오 퀘스트 클리어
            List<ScenarioTask> scenarioTask = GetScenarioTask(_lastRound);
            if(scenarioTask == null) return;
            foreach (var scenario in scenarioTask)
            {
                // 퀘스트 클리어 기본조건 확인
                if(!CheckTaskData(scenario.ScTask, TaskType.Patrol, taskId, taskTargetId)) continue;

                if (scenario.ScTask.taskData is not PatrolTask patrolTask) continue;
                if (patrolTask.roomState == roomState)
                {
                    scenario.ScTask.isClear = true;
                    Scenario.ScenarioTaskClear(scenario.EndingType, scenario.ProcessType, scenario.RootId);
                    break;
                }
            }   
        }
        
        /// <summary>
        /// PopUp Task
        /// </summary>
        public void PopUpCheckHandler(string taskId, string taskTargetId)
        {
            ProcessTaskCheckHandler(TaskType.PopUp, taskId, taskTargetId);
        }

        /// <summary>
        /// Interaction Task
        /// </summary>
        public void InteractTaskCheckHandler(string taskId, string taskTargetId)
        {
            ProcessTaskCheckHandler(TaskType.Interaction, taskId, taskTargetId);
        }
        
        /// <summary>
        /// Get Task
        /// </summary>
        public void GetTaskCheckHandler(string taskId, string taskTargetId)
        {
            ProcessTaskCheckHandler(TaskType.Get, taskId, taskTargetId);
        }

        /// <summary>
        /// Get Task 클리어 알람 X
        /// </summary>
        public void GetTaskCheckHandlerNoClearAlarm(string taskId, string taskTargetId)
        {
            ProcessTaskCheckHandler(TaskType.Get, taskId, taskTargetId, false);
        }
        

        /// <summary>
        /// Traders Buy Task
        /// </summary>
        public void TradersBuyTaskCheckHandler(string taskId, string taskTargetId)
        {
            ProcessTaskCheckHandler(TaskType.TradersBuy, taskId, taskTargetId);
        }

        /// <summary>
        /// Reserve Task
        /// </summary>
        public void ReserveTaskCheckHandler(string taskId, string taskTargetId)
        {
            ProcessTaskCheckHandler(TaskType.Reserve, taskId, taskTargetId);
        }
        

        /// <summary>
        /// Bonus Task
        /// </summary>
        public void BonusTaskCheckHandler(TaskType taskType, string taskId)
        {  
           BonusCheckHandler(taskType, taskId); 
        }
            
        

        #endregion

        #region 퀘스트 디버깅용

        public void CreateTestStdTask(StandardTask stdTaskData)
        {
            if (CurrentStandardTask == null)
            {
                CurrentStandardTask = new TaskInfo();
                CurrentStandardTask.standardRoundTaskList = new List<StandardRoundTask>();
                CurrentStandardTask.standardRoundTaskList.Add(new StandardRoundTask()
                {
                    targetRound = 1,
                    standardTaskList = new List<StandardTask>(),
                });
            }
            
            // 로컬라이징 데이터 얻기
            stdTaskData.taskDescription = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.Standard_Task_Item_Table, 
                LocalizationTable.Standard_Task_Item_TableTextKey.Standard_Task_.ToString() + stdTaskData.taskTableId);
            // Place Holder 설정
            if (stdTaskData.placeHolder && !stdTaskData.runtimeTask)
            {
                string taskTargetId = stdTaskData.taskData.taskTargetId;
                if (stdTaskData.taskData is PutTask putTask)
                {
                    if(putTask.taskId.Equals(TaskID.Put_Inventory_Item_Return.ToString())) taskTargetId = putTask.putRequireItemId[0];
                }
                string placeHolderStr = TaskPlaceHolderMapper(taskTargetId, stdTaskData.taskData.GetTaskTargetLocalization(), stdTaskData.placeHolderTextTableKey);
                stdTaskData.taskDescription = stdTaskData.taskDescription.Replace(LocalizationVariableHelper.GetPlaceHolderString(stdTaskData.taskDescription), placeHolderStr);
            }
            
            CurrentStandardTask.standardRoundTaskList[0].standardTaskList.Add(stdTaskData);
            // "새 임무 할당 UI (LeftTopGuide)"
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(LocalizationTable.CheckList_UI_TableTextKey.Check_List_Update);
        }

        public void CreateTestBonusTask(BonusTask bonusTask)
        {
            if (CurrentStandardTask == null)
            {
                CurrentStandardTask = new TaskInfo();
                CurrentStandardTask.standardRoundTaskList = new List<StandardRoundTask>();
                CurrentStandardTask.standardRoundTaskList.Add(new StandardRoundTask()
                {
                    targetRound = 1,
                    standardTaskList = new List<StandardTask>()
                });

                _currentRoundTask = CurrentStandardTask.standardRoundTaskList[0];
            }
            
            bonusTask.taskDescription = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.Standard_Task_Item_Table,
                LocalizationTable.Standard_Task_Item_TableTextKey.Standard_Task_ + 15.ToString());
                
            CurrentStandardTask.standardRoundTaskList[0].bonusTaskList.Add(bonusTask);
        }

        public void RemoveTestStdTask()
        {
            CurrentStandardTask = null;
            _currentRoundTask = null;
            _currentRoundStdTaskList = null;
        }

        #endregion

        #region 퀘스트 데이터 클래스
        
        public class TaskInfo
        {
            public int targetNday;
            public List<StandardRoundTask> standardRoundTaskList;
        }
        
        public class StandardRoundTask
        {
            public int targetRound;
            public List<StandardTask> standardTaskList;
            public List<BonusTask> bonusTaskList = new List<BonusTask>();

            public bool isAllClear = false;
        }
        
        public class StandardTask
        {
            public int taskTableId;
            public bool runtimeTask = false;
            public bool placeHolder = false;
            [JsonConverter(typeof(StringEnumConverter))]
            public LocalizationTable.TextTable placeHolderTextTableKey = LocalizationTable.TextTable.None;
            [JsonConverter(typeof(StringEnumConverter))]
            public TaskType taskType = TaskType.None;
            public TaskData taskData = null;

            public string taskDescription;
            public bool isClear = false;
        }

        public class BonusTask
        {
            public string taskId;
            public string taskDescription;
            public TaskType taskType = TaskType.None;
            public bool isClear = false;
            
            public BonusTask(TaskType taskType, string taskId)
            {
                this.taskId = taskId;
                this.taskType = taskType;
            }
        }

        #endregion    
        
        #region 시나리오 퀘스트 데이터
        
        public class ScenarioTaskInfo
        {
            public int targetNday;
            public List<ScenarioRoundTask> scenarioRoundTaskList;
        }

        public class ScenarioRoundTask
        {
            public int targetRound;
            public List<ScenarioTask> scenarioTaskList;
        }
   
        #endregion
    }
}





