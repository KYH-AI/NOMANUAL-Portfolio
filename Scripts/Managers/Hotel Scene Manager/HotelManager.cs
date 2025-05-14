using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.DreamOS;
using NoManual.CutScene;
using NoManual.Interaction;
using NoManual.Task;
using NoManual.UI;
using NoManual.Utils;
using UnityEngine;

namespace NoManual.Managers
{
    public class HotelManager : Singleton<HotelManager>
    {
        private NoManualHotelManager _noManualManager;
        public bool DebugMode = false;
        private bool _debugNewDayStart = true;
        private bool _debugNewRoundStart = true;
        public PlayerSaveData DebugSaveData = null;
        [SerializeField] private EndingType debugEndingType;
        
        
        #region ANO 매니저
        
        public ANO_Manager ANO { get; private set; }

        #endregion

        #region Object 매니저

        public ObjectManager ObjectManager { get; private set; }

        #endregion
        
        #region 라운드 관련 변수 (Legacy)
        
        [Header("============== Legacy (사용 X) ==============")]
        public RoundSetting gameRound = new RoundSetting();

        #endregion

        #region 튜토리얼 관련 변수

        [Header("튜토리얼")]
        [SerializeField] private HotelLobbyTutorial hotelLobbyTutorial;
        
        // 튜토리얼 클리어 확인
        public bool TutorialClear { get; set; } = false;
        

        #endregion

        #region 근무목록(퀘스트)

        public TaskHandler TaskHandler { get; private set; } = new NoManual.Task.TaskHandler();

        #endregion

        #region PC OS

        [SerializeField] private Nomanual_PC_OS_Manager pcOSManager;
        public Nomanual_PC_OS_Manager PcOSManager => pcOSManager;

        #endregion

        #region 게임 일차 및 라운드

        public DayAndRound DayAndRound { get; private set; }

        #endregion

        #region 보고 컷신

        public ReportCutScene ReportCutScene { get; private set; }
        
        public bool IsPlayReportCutScene { get; set; } = false;

        #endregion


        #region 강제 엔딩 이동

        [ContextMenu("기본 엔딩 이동")]
        public void EndingTypeNoneSaveData()
        {
            DayAndRound.DebugModeDay(DayAndRound.MAX_DAY);
            TaskHandler.Scenario.LastClearScType = EndingType.None;
   
            _noManualManager.GetCurrentSaveFile().Day = DayAndRound.MAX_DAY;
            ExitHotel(true, false, GameManager.SceneName.Monologue);
        }
        
        [ContextMenu("A 엔딩 이동")]
        public void EndingTypeASaveData()
        {
            DayAndRound.DebugModeDay(DayAndRound.MAX_DAY);
            TaskHandler.Scenario.LastClearScType = EndingType.A;
            TaskHandler.Scenario.ProcessType = ProcessType.End;
            _noManualManager.GetCurrentSaveFile().Day = DayAndRound.MAX_DAY;
            ExitHotel(true, false, GameManager.SceneName.Monologue);
        }
        
        [ContextMenu("B 엔딩 이동")]
        public void EndingTypeBSaveData()
        {
             DayAndRound.DebugModeDay(DayAndRound.MAX_DAY);
            TaskHandler.Scenario.LastClearScType = EndingType.B;
            TaskHandler.Scenario.ProcessType = ProcessType.End;
            _noManualManager.GetCurrentSaveFile().Day = DayAndRound.MAX_DAY;
            ExitHotel(true, false, GameManager.SceneName.Monologue);
        }

        #endregion
        
        #region 게임 시작 전 데이터 초기화
        
        private void Awake()
        {
            StartCoroutine(InitializedProcess());
        }
        
        private void Start()
        {
            // 게임 시작 Fade Out
            NoManualHotelManager.Instance.PlayerStartFade(false, 15f, true, 0.45f);

            if (!TutorialClear && hotelLobbyTutorial)
            { 
                hotelLobbyTutorial.StartTutorial();
            }
        }
        
        private IEnumerator InitializedProcess()
        {
            // NoManualHotelManager의 인스턴스를 얻기 위해 대기
            while (NoManualHotelManager.Instance == null || !NoManualHotelManager.Instance.OnInitialized)
            {
                yield return null;  // 다음 프레임까지 대기
            }

            _noManualManager = NoManualHotelManager.Instance;
            
            // 호텔 세이브 데이터 이벤트 등록
            _noManualManager.HotelSaveDataOvreWriteEvent -= HotelSaveData;
            _noManualManager.HotelSaveDataOvreWriteEvent += HotelSaveData;
            
            
            Initialization();  // 초기화 시작
        }

        /// <summary>
        /// 게임 시작 전 데이터 초기화 (일차 호출)
        /// </summary>
        private void Initialization()
        {
            ObjectManager = GetComponentInChildren<ObjectManager>();
            ANO = GetComponentInChildren<ANO_Manager>();
            ReportCutScene = GetComponentInChildren<ReportCutScene>();

            /* 퀘스트 트리거 이벤트 등록 */
            InitializationTaskEvent();
            /* 일차 시작 전 준비 */
            InitializationDay();
            /* 라운드 시작 전 세팅 */
            InitializationRound();
        }
        
        #endregion

        #region 일차 초기화

        /// <summary>
        /// 근무목록 퀘스트 클리어 이벤트 등록 (일차 호출)
        /// </summary>
        private void InitializationTaskEvent()
        {
            // 근무 Json 읽어오기
            TaskHandler.TaskHandlerInitialization(PcOSManager.UpdateRecordItem_Hotel);
            
            ANO.InitializationAnomalyManager(TaskHandler.BonusTaskCheckHandler);
            
            ObjectManager.InitializationObject(TaskHandler.InteractTaskCheckHandler, TaskHandler.PatrolTaskCheckHandler);
            
            _noManualManager.InventoryManager.GetInventoryItemTaskHandler -= TaskHandler.GetTaskCheckHandler;
            _noManualManager.InventoryManager.GetInventoryItemTaskHandler += TaskHandler.GetTaskCheckHandler;
            _noManualManager.InventoryManager.TradersDeliveryTaskHandler -= TaskHandler.ManualTaskClearHandler;
            _noManualManager.InventoryManager.TradersDeliveryTaskHandler += TaskHandler.ManualTaskClearHandler;
            _noManualManager.InventoryManager.PutInventoryItemTaskHandler -= TaskHandler.PutTaskCheckHandler;
            _noManualManager.InventoryManager.PutInventoryItemTaskHandler += TaskHandler.PutTaskCheckHandler;
            
            PcOSManager.InitializationTaskEvent(TaskHandler.PopUpCheckHandler, 
                                                TaskHandler.TradersBuyTaskCheckHandler, 
                                                TaskHandler.ReserveTaskCheckHandler,
                                                TaskHandler.ManualTaskClearHandler);
        }

        /// <summary>
        /// ANO 데이터 초기화 (일차 호출)
        /// </summary>
        private void InitializationAnomaly(int[] anoLink, TaskHandler.BonusTaskEventHandler createTaskEventHandler)
        {
            ANO.InitializationAnomaly(anoLink, createTaskEventHandler);
        }
        
        /// <summary>
        /// 객실 룸 데이터 및 과거 예약자 초기화 (일차 호출)
        /// </summary>
        private void InitializationRoomState(GuestRoom[] saveRoomData, string[] reservedId)
        {
            PcOSManager.SetReservedCustomer(reservedId);
            PcOSManager.SetGuestRoomData(saveRoomData);
            UpdateRoomDoor();
        }
        
        /// <summary>
        /// 모든 객실 문 상태 업데이트 (일차 호출)
        /// </summary>
        private void UpdateRoomDoor()
        {
            // 객실 문 State 업데이트
            ObjectManager.GuestRoomDoorHandler(PcOSManager.SaveGuestRoomData());// PcOSManager.ohHomePage.occasionHotelHomePageHelper.guestRoomData.Values.ToArray());
        }

        /// <summary>
        /// 시나리오 엔딩 기준확인 (일차 호출)
        /// </summary>
        private void InitializationScenarioEnding(EndingType endingType)
        {
            TaskHandler.Scenario.LastClearScType = endingType;
        }

        /// <summary>
        /// PC OS 메신저 불러오기 (일차 호출)
        /// </summary>
        private void InitializationMessenger(MessagingManager.ChatItemData[] chatMessageId, 
                                            MailData[] mailId, 
                                            int[] noteId, NotepadLibrary.NoteItem[] customNoteId, 
                                            int[] pictureId, 
                                            int[] musicId, 
                                            int[] videoId) 
        {
            /*
                - 로컬라이징
                - 메세지 (부팅 성공 시 생성)
                - 메일  (부팅 성공 시 생성)
                - 메모장
                - 이미지
                - 음악
                - 영상
            */
            
            
            PcOSManager.MessengerLocalization();
            pcOSManager.LoadMessage(chatMessageId);
            pcOSManager.LoadMail(mailId);
            pcOSManager.LoadNotePad(noteId);
            pcOSManager.LoadCustomNotePad(customNoteId);
            pcOSManager.LoadPicture(pictureId);
            pcOSManager.LoadMusic(musicId);
            pcOSManager.LoadVideo(videoId);
        }

        /// <summary>
        /// R.S 트레이더스 초기화 (일차 호출)
        /// </summary>
        private void InitializationRS_Trades(int rsCash, RS_TradersDeliveryMapper[] deliveryItemId)
        { 
            // 1. 트레이더스 판매목록 초기화
            // 2. 배송물품 
            // 3. 플레이어 소지금
            PcOSManager.Init_RS_TradersItem(DayAndRound.CurrentDay);
            PcOSManager.Send_RS_TradersDelivery(deliveryItemId);
            PcOSManager.Add_RS_Cash(rsCash);
        }

        /// <summary>
        /// 파밍 아이템 초기화 (일차 호출)
        /// </summary>
        private void InitializationFarm(int day)
        {
            ObjectManager.CreateFarmingObject(day);
        }

        /// <summary>
        /// PC OS App 관련 이벤트 초기화 (일차 호출)
        /// </summary>
        private void InitializationAppEvent()
        {
            pcOSManager.InitializationAppEvent(Report);
        }

        #endregion
        
        #region 라운드 초기화

        /// <summary>
        /// ANO 오브젝트 라운드 초기화
        /// </summary>
        private void InitializationANO_Round()
        {
            TaskHandler.ResetBonusTaskData();
            ANO.ResetAno();
            ANO.PickUpAnoHandler();
        }
        
       /// <summary>
        /// 퀘스트 데이터 초기화 (라운드 초기화 [가장 마지막 호출])
        /// </summary>
        private void InitializationTaskData()
        {
            var taskList =  GetRoundStandardTask();
            if (taskList == null)
            {
                ErrorCode.SendError(ErrorCode.ErrorCodeEnum.GetTaskInfo);
                return;
            }          
            
            /*  1. Std Task 관련 오브젝트 생성, 데이터 준비 및 런타임 설정 */
            foreach (var task in taskList)
            {
                // 아래 조건은 일반 근무 런타임 특수 케이스
                if (task.taskType == TaskHandler.TaskType.Patrol &&
                    task.taskData.taskId.Equals(TaskHandler.TaskID.Patrol_Room_Field.ToString()))
                {
                    if (task.taskData is PatrolTask patrolTask)
                    {
                        // Patrol_Room_Field 퀘스트 동적생성 완료 확인용
                        bool needPatrolRoomFieldTaskCreate = true;
                        // 객실 필터링에 사용될 수 있는 Room State 리스트
                        List<UI_GuestRoomItem.RoomState> otherStates = new List<UI_GuestRoomItem.RoomState> { UI_GuestRoomItem.RoomState.Empty,
                                                                                                                UI_GuestRoomItem.RoomState.Forbidden, 
                                                                                                                UI_GuestRoomItem.RoomState.Reservation, };
                        do
                        {
                            // Room State에 맞는 객실 필터링
                            List<GuestRoom> filteredRooms = PcOSManager.ohHomePage.occasionHotelHomePageHelper.GetFilteredGuestRoomData(patrolTask.roomState);
        
                            // 필터링된 객실 중 랜덤으로 선택
                            if (filteredRooms.Count > 0)
                            {
                                GuestRoom randomRoom = filteredRooms[UnityEngine.Random.Range(0, filteredRooms.Count)];
                                patrolTask.taskTargetId = randomRoom.roomNumber.ToString();
                                needPatrolRoomFieldTaskCreate = false; // Patrol_Room_Field 퀘스트가 성공적으로 생성됨
                                ObjectManager.DoorHandler(patrolTask.taskTargetId, DoorBaseComponent.DoorStateEnum.Close); // Patrol_Room_Field 퀘스트가 접근이 가능하도록 Door Lock 해체
                                SetRunTimeStdTaskData(task.taskType, task.taskData.taskId, task.taskData.taskTargetId); // Patrol_Room_Field 퀘스트 생성
                            }
                            else
                            {
                                // 객실 필터링이 2개 이상인 경우
                                if (otherStates.Count > 1)
                                {
                                    // 현재 퀘스트에 설정된 Room State는 제거
                                    otherStates.Remove(patrolTask.roomState); 
                                    // 다른 상태 중 랜덤으로 선택
                                    patrolTask.roomState = otherStates[UnityEngine.Random.Range(0, otherStates.Count)];
                                }
                                // 더 이상 필터링 할 수 없는 경우
                                else
                                {
                                    patrolTask.roomState = otherStates[0];
                                }
                            }
        
                        } while (needPatrolRoomFieldTaskCreate); // 객실이 성공적으로 선택될 때까지 반복
                    }
                }
                //투수객 선택
                else if (task.taskType == TaskHandler.TaskType.Reserve)
                {
                    // 특정 에약자 뽑기
                    if(task.taskData.taskId.Equals(TaskHandler.TaskID.Reserve_Target_Clear.ToString()))
                    {
                        PcOSManager.AddTargetReservationItem(task.taskData.taskTargetId);
                    }
                    // 랜덤(3~5명) 예약자 뽑기
                    else if (task.taskData.taskId.Equals(TaskHandler.TaskID.Reserve_All_Clear.ToString()))
                    {
                        PcOSManager.AddRandomReservationItem();
                    }
                    continue;
                }
                
                // Std 퀘스트에 알맞는 오브젝트 준비
                ObjectManager.SetStdTaskObject(task);
            }
        }

       
       /// <summary>
       /// 스토리 오브젝트 라운드 초기화
       /// </summary>
       private void InitializationScenarioObject()
       {
           // 엔딩 분기점 업데이트
           TaskHandler.Scenario.UpdateScenarioEndingType();
           
           var taskList =  GetRoundScenarioTask();
#if UNITY_EDITOR
           if(DebugMode)  NoManualHotelManager.Instance.UiNoManualUIManager.DebugTestUI.SetScText(-1, false);
#endif   
           
           // 이번 라운드에서 시나리오 퀘스트가 존재하지 않을 경우
           if (taskList == null || taskList.Count == 0)
           {
               TaskHandler.Scenario.isClear = true;
               return;
           }

           foreach (var scenarioTask in taskList)
           {
               if (scenarioTask.ProcessType == ProcessType.None)
               {
                   NoManualUtilsHelper.EditorDebugLog(NoManualUtilsHelper.LogTextColor.red, 
                       "시나리오 Task[RootId:" +scenarioTask.RootId+ "][Day:" +scenarioTask.RootDay+ "][Round:"+scenarioTask.RootRound+"] Process Type is None!");
                   continue;
               }
                
               // EndingType과 ProcessType을 구별해 알맞는 시나리오 Task가 생성
               if (scenarioTask.ProcessType != ProcessType.Start)
               {
                   if (scenarioTask.EndingType != TaskHandler.Scenario.LastClearScType) continue;
               }
               
               // 이번 라운드에서는 시나리오 관련 퀘스트가 존재함
               TaskHandler.Scenario.isClear = false;
#if UNITY_EDITOR               
               if(DebugMode)  NoManualHotelManager.Instance.UiNoManualUIManager.DebugTestUI.SetScText(scenarioTask.RootId, false);
#endif               
               
               // 투수객 선택
               if (scenarioTask.ScTask.taskType == TaskHandler.TaskType.Reserve && 
                   scenarioTask.ScTask.taskData.taskId.Equals(TaskHandler.TaskID.Reserve_Target_Clear.ToString()))
               {
                   // 특정 에약자 뽑기
                   PcOSManager.AddTargetReservationItem(scenarioTask.ScTask.taskData.taskTargetId);
                   continue;
               }
               ObjectManager.SetScenarioTaskObject(scenarioTask);
           }
       }

       /// <summary>
       /// 메일 및 메세지 라운드 초기화
       /// </summary>
       private void InitializationPcOsMailAndMsgItem()
       {
           // 메일 및 메세지 csv 파싱한 값으로 데이터 생성
           pcOSManager.ReadCsvMapper(DayAndRound.CurrentDay, DayAndRound.CurrentRound, EndingType.None);
           pcOSManager.CreateMailAndMsg();
       }

       /// <summary>
       /// Get 퀘스트 아이템이 인벤토리에 있는지 확인 라운드 초기화 (호출 보고 완료 시 진행)
       /// </summary>
       public void InventoryItemCheck()
       {
           var standardTasks = GetRoundStandardTask();
           Dictionary<int, int> duplicationGetStd = new Dictionary<int, int>();
           // 퀘스트 만큼 인벤토리 아이템 확인
           foreach (var stdTask in standardTasks)
           {
               if (stdTask.taskType == TaskHandler.TaskType.Get)
               {
                   int taskTargetId = Convert.ToInt32(stdTask.taskData.taskTargetId);
            
                   // 딕셔너리에 존재 여부 확인 후 초기화하여 중복 증가
                   if (!duplicationGetStd.ContainsKey(taskTargetId))
                   {
                       duplicationGetStd[taskTargetId] = 0;
                   }
                   duplicationGetStd[taskTargetId]++;
               }
           }

           // 퀘스트 클리어 확인
           foreach (var getItemId in duplicationGetStd)
           {
               int inventoryCount = NoManualHotelManager.Instance.InventoryManager.HasInventoryItem(getItemId.Key);
               int clearCount = Math.Min(getItemId.Value, inventoryCount);

               for (int i = 0; i < clearCount; i++)
               {
                   TaskHandler.GetTaskCheckHandlerNoClearAlarm(TaskHandler.TaskID.Get_Inventory_Item.ToString(), getItemId.Key.ToString());
               }
           }

           // ??? 시나리오도 Get 아이템이 있는지 미리 확인해야할까? (시나리오 오브젝트는 동적으로 생성되니깐 가지고 있을 수 가 없다)
       }

       #endregion
        
        #region 게임 준비 관련

        [ContextMenu("일차 넘기기")]
        public void DebugInitDay()
        {
            HotelSaveData();
            InitializationDay();
            InitializationRound();
        }
        
        /// <summary>
        /// 일차 시작 전 세팅
        /// </summary>
        private void InitializationDay()
        {
            /* 일차 시작 전 준비 */
            /* 플레이어 세이브 데이터 읽기
               1. PC OS (호텔 씬)
                - 메세지
                - 메일
                - 메모장
                - 이미지
                - 음악
                - 영상
               2. R.S Cash 재화 (호텔 씬) + R.S 트레이더스 장바구니 (호텔 씬) + R.S 트레이더스 배송물품 생성 (호텔 씬)
               3. 호텔 방 정보 (호텔 씬) +  과거 예약자 리스트 (호텔 씬) 
               4. ------------------ 플레이어 인벤토리 -> (NoManualHotelManager 에서 진행)
               5. 시나리오 상태, EndingType(호텔 씬)
               6. ANO Link Id (호텔 씬)
               7. ------------------ EndingProcess -> (NoManualHotelManager 에서 진행) ?
             */

#if !UNITY_EDITOR
            DebugMode = false;
#endif
            
            PlayerSaveData playerSaveData;
            if (DebugMode)
            {
                if (DebugSaveData == null) DebugSaveData = new PlayerSaveData();
                playerSaveData = DebugSaveData;
                Debug.Log("현재 디버그 세이브 파일 이용 중");
            }
            else
            {
                playerSaveData = _noManualManager.GetCurrentSaveFile();
            }

            if (playerSaveData == null) return;
            
            DayAndRound = new DayAndRound(playerSaveData.Day);

#if UNITY_EDITOR
            
            // 테스트 
            if (DebugMode)
            {
                if(!_debugNewDayStart) DayAndRound.IncreaseDay();
                _debugNewDayStart = false;
                _debugNewRoundStart = true;
                DayAndRound.DebugModeRound(1);

                NoManualHotelManager.Instance.UiNoManualUIManager.DebugTestUI.SetDayText(DayAndRound.CurrentDay);
                DebugSaveData.Day = DayAndRound.CurrentDay;
                
                Debug.Log("현재 디버그 모드");
            }
#endif
            

            InitializationMessenger(playerSaveData.Chat_ID, 
                                    playerSaveData.Mail_ID, 
                                    playerSaveData.NotePad_ID, playerSaveData.CustomNotePad,
                                    playerSaveData.Picture_ID, 
                                    playerSaveData.Music_ID, 
                                    playerSaveData.Video_ID);
            InitializationRoomState(playerSaveData.GuestRooms, playerSaveData.Reserved_ID);
            InitializationRS_Trades(playerSaveData.RS_Cash, playerSaveData.DeliveryItem);
            InitializationScenarioEnding(playerSaveData.EndingType);
            InitializationAnomaly(playerSaveData.AnoLink, SetRunTimeBonusTaskData);
            

            /* 퀘스트 */
            /* 해당 일차 Task Info 불러오기 (SetNewDayStandardTask) */
            SetNewDayStandardTask();
             
            /* 파밍 아이템 */
            /* 파밍 아이템 준비 (InitializationFarm)  */
            InitializationFarm(DayAndRound.CurrentDay);
            
            /* PC App  */
            InitializationAppEvent();
        }

        [ContextMenu("라운드 넘기기")]
        /// <summary>
        /// 라운드 시작 전 세팅
        /// </summary>
        public void InitializationRound()
        {
            /* 라운드 시작 전 세팅 */
            /*
               1. Object 매니저 초기화
               2. ANO 생성
               3. 시나리오 관련 생성
               4. Run Time Task 데이터 세팅
                 4-1. Std Run Time Task
                   - Patrol_Room_Field
                   - Reserve_Target
                   - Reserve_All_Clear
                 4-2. ------------------ Bonus Run Time Task -> (ANO Manager)
               5. PC OS 메일 및 메세지 생성  
    
             */
            
            // 테스트 
#if UNITY_EDITOR     
            if (DebugMode)
            {
                if(!_debugNewRoundStart) DayAndRound.IncreaseRound();
                _debugNewRoundStart = false;
                
                NoManualHotelManager.Instance.UiNoManualUIManager.DebugTestUI.SetRoundText(DayAndRound.CurrentRound);
            }
#endif            

            TaskHandler.CheckRoundTask(DayAndRound.CurrentRound);
            ObjectManager.ResetAllObject();
            InitializationANO_Round();
            InitializationScenarioObject();
            InitializationTaskData();
            InitializationPcOsMailAndMsgItem();
            
            // 0일차는 손전등만 팔기 (판매품록 제한시키기)
            if (DayAndRound.CurrentDay == DayAndRound.MIN_DAY)
            {
                if (DayAndRound.CurrentRound == DayAndRound.MIN_ROUND)
                {
                    ObjectManager.DoorHandler("Security Door", DoorBaseComponent.DoorStateEnum.Lock);
                    NoManualHotelManager.Instance.UiNoManualUIManager.DelayShowLeftTopGuideText(LocalizationTable.CheckList_UI_TableTextKey.Check_List_FirstDay, 3f);
                }
                else if (DayAndRound.CurrentRound == 3)
                {
                    ObjectManager.DoorHandler("Security Door", DoorBaseComponent.DoorStateEnum.Open);
                }
                else if (DayAndRound.CurrentRound == DayAndRound.MAX_ROUND)
                {
                    pcOSManager.Add_RS_Cash(50);
                }
            }

            InventoryItemCheck();
        }

        #endregion
        
        
        #region 최종 보고

        /// <summary>
        /// 라운드 보고 확인 (보고 버튼 이벤트)
        /// </summary>
        private bool Report(bool checkOnly)
        {
            /* 1. 기본 근무 모두 클리어 확인 */
            var stdRoundTaskList = GetRoundStandardTask();
            foreach (var standardTask in stdRoundTaskList)
            {
                if (!standardTask.isClear)
                {
                    if (checkOnly) return false; // 보고 가능한지 확인 모드일 경우 바로 반환
                }
            }

            if (checkOnly) return true;

            // 모든 기본 근무가 클리어된 경우에만 보고 가능
            return IsPlayReportCutScene = true;
        }

        /// <summary>
        /// 라운드 증가 (보고 연출 동시에 호출)
        /// </summary>
        public void SetNextRound()
        {  
            // 라운드 증가
            if (!DayAndRound.LastRoundCheck())
            {
                if (!DebugMode)
                {
                    DayAndRound.IncreaseRound();
                }
                InitializationRound();
            }
            // 라운드 모두 클리어
            else
            {
                if (!DebugMode)
                { 
                    ObjectManager.ResetAllObject();
                    ANO.ResetAno();
                }
                
#if  UNITY_EDITOR                
                // 테스트용
                if (DebugMode)
                {
                    NoManualHotelManager.Instance.SaveToOverWriteFile(true);
                    DebugInitDay();
                }
#endif                
            }
            
     
        }

        #endregion

        #region 퀘스트 근무목록 관련 함수
        
        /// <summary>
        /// 새로운 N일차 근무목록 발급
        /// </summary>
        public void SetNewDayStandardTask()
        { 
            TaskHandler.SetNewDayStandardTask(DayAndRound.CurrentDay);
        }
        
        /// <summary>
        /// 런타임 기본 퀘스트 데이터 설정
        /// </summary>
        private void SetRunTimeStdTaskData(TaskHandler.TaskType taskType, string taskId, string taskTargetId)
        {
            TaskHandler.SetRunTimeStdTaskData(DayAndRound.CurrentRound, taskType, taskId, taskTargetId);
        }

        /// <summary>
        /// 런타임 서브 퀘스트 데이터 설정
        /// </summary>
        private void SetRunTimeBonusTaskData(TaskHandler.TaskType taskType, string taskId)
        {
            TaskHandler.SetRunTimeBonusTaskData(taskType, taskId);
        }

        /// <summary>
        /// 라운드 시나리오 퀘스트 정보 얻기
        /// </summary>
        public List<ScenarioTask> GetRoundScenarioTask()
        {
            return TaskHandler.GetScenarioTask(DayAndRound.CurrentRound);
        }
        
        /// <summary>
        /// 라운드 기본 근무목록 정보 얻기
        /// </summary>
        public List<TaskHandler.StandardTask> GetRoundStandardTask()
        {
            return TaskHandler.GetRoundStandardTask(DayAndRound.CurrentRound);
        }

        /// <summary>
        /// 라운드 추가 근무목록 정보 얻기
        /// </summary>
        public List<TaskHandler.BonusTask> GetRoundBonusTask()
        {
            return TaskHandler.GetBonusTask(DayAndRound.CurrentRound);
        }

        #endregion

        #region 퇴근
        
            #region Hotel 세이브 데이터 파일

            
                /// <summary>
                /// 호텔 씬 세이브 데이터 (퇴근 문 상호작용 시 호출)
                /// </summary>
                private void HotelSaveData(bool allSaveData = true)
                {
                    PlayerSaveData currentSaveData;

                    if (DebugMode) currentSaveData = DebugSaveData;
                    else currentSaveData = _noManualManager.GetCurrentSaveFile();
                    if (currentSaveData != null)
                    {
                        // 엔딩 세이브 파일 
                        currentSaveData.EndingType = TaskHandler.Scenario.SaveScenarioEndingType();
                        ProcessType saveProcessType = TaskHandler.Scenario.SaveProcessType();
                        if (DayAndRound.LastDayCheck() && currentSaveData.EndingType == EndingType.None)  saveProcessType = ProcessType.End;
                        currentSaveData.EndingProcess = saveProcessType;
                        
                        // 정신력 세이브 파일
                        currentSaveData.Mentatilty = HFPS.Player.PlayerController.Instance.mentality.CurrentMentality;
                        
                        // 그 외 세이브 파일
                        if (allSaveData)
                        {
                            currentSaveData.Day = DayAndRound.CurrentDay;
                            currentSaveData.RS_Cash = pcOSManager.SaveRsChas();
                            currentSaveData.Chat_ID = pcOSManager.LocalSaveMessage();
                            currentSaveData.Mail_ID = pcOSManager.SaveMail();
                            currentSaveData.Picture_ID = pcOSManager.SavePicture();
                            currentSaveData.NotePad_ID = pcOSManager.SaveNotePad();
                            currentSaveData.CustomNotePad = pcOSManager.SaveCustomNotePad();
                            currentSaveData.Video_ID = pcOSManager.SaveVideo();
                            currentSaveData.Music_ID = pcOSManager.SaveMusic();
                            currentSaveData.DeliveryItem = pcOSManager.SaveDeliveryItem();
                            currentSaveData.GuestRooms = pcOSManager.SaveGuestRoomData();
                            currentSaveData.Reserved_ID = pcOSManager.SaveReservedCustomer();
                            currentSaveData.AnoLink = ANO.SaveAnoLink();
                            currentSaveData.ClearAnoCount = TaskHandler.DayClearAnoCount;
                        }
                    }
                }

                #endregion

            #region 이벤트 구독 해체 (호텔 씬에서 탈출)

                private void RemoveEventSubscribe()
                {
                    
                }

            #endregion

            #region 호텔 탈출

                public void ExitHotel(bool needSave, bool allSaveFile, GameManager.SceneName targetScene)
                {
                    RemoveEventSubscribe();
                    
                    /******************************* 베타 엔딩은 무조건 1일차에 종료 *******************************/
                    /*
                    if (DayAndRound.CurrentDay == 1)
                    {
                        Monologue.EndingCredit = true;
                        needSave = false;
                        allSaveFile = false;
                    }
                    */
                    NoManualHotelManager.Instance.SceneMove(needSave, allSaveFile, targetScene);
                }
                
            #endregion
            
        #endregion
    }

        /// <summary>
        /// 게임 라운드 (Legacy)
        /// </summary>
        [Serializable]
        public class RoundSetting
        {
            [Serializable]
            public struct GameRoundSettings
            {
                [Header("최소 라운드")]
                public int minRound;
                [Header("최대 라운드")]
                public int maxRound;
            }
            [Header("게임 라운드 설정")]
            public GameRoundSettings gameRoundSettings = new GameRoundSettings();

            [Header("에디터 확인용")]
            [SerializeField] public int cntRound;

            // 현재 게임 라운드
            public int CurrentRoundCount { get; set; } = 0;

            // 게임 클리어 종료 확인
            public bool IsGameClear { get; set; } = false;

            /// <summary>
            /// 현재 라운드 +1증가
            /// </summary>
            public void IncreaseRoundCount()
            {
                if (gameRoundSettings.maxRound > CurrentRoundCount)
                {
                   cntRound = CurrentRoundCount += 1;
                }
            }

            /// <summary>
            /// 현재 라운드 -1 감소
            /// </summary>
            public void DecreaseRoundCount()
            {
                if (gameRoundSettings.minRound < CurrentRoundCount)
                {
                    cntRound = CurrentRoundCount -= 1;
                }
            }

            /// <summary>
            /// 현재 라운드를 사용자 지정 라운드로 변경
            /// </summary>
            public void SetRoundCount(int roundCount)
            {
                cntRound = CurrentRoundCount = Mathf.Clamp(roundCount, gameRoundSettings.minRound, gameRoundSettings.maxRound);
            }

            /// <summary>
            /// 현재 라운드가 마지막 라운드인지 확인 
            /// </summary>
            /// <returns></returns>
            public bool IsLastRound()
            {
                 return CurrentRoundCount == gameRoundSettings.maxRound;
            }


            public int currentDay = 1;
        }

}

