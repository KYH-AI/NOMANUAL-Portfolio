using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NoManual.Interaction;
using NoManual.Patrol;
using NoManual.Task;
using NoManual.UI;
using NoManual.Utils;
using UnityEngine;

namespace NoManual.Managers
{
    public class ObjectManager : MonoBehaviour
    {
        /*
           1. 문, Put 관련 오브젝트 
           2. IPut Mode를 관리해야함
           3. Put Task에 맞는 Put 관련된 오브젝트만 Put Mode => Put 으로 변경
           4. 그 외에 Put 관련 오브젝트들은 모두 상호작용이 불가능하게  Put Mode => None 으로 변경
           
           1. Interact 오브젝트 
           2. interact 오브젝트 InitializationInteractObject 실행
           3. Interact  Task에 맞는 오브젝트를 찾아 TaskIdleMode 실행
           
           1. 파밍 시스템 초기화
           
           
           == 다시 정리 (09.02) ==
           
           1. Patrol
           2. 문
           3. Put
           4. Interact
           5. 파밍
           6. ANO (ANO 매니저)
           7. 시나리오
           8. 크리처 (ANO 매니저)
         */

        private readonly string _FARMING_JSON_PATH = "JsonData/Farming/Farming_List";
        private readonly Dictionary<int, Farming.Item[]> _framingParse = new Dictionary<int, Farming.Item[]>(DayAndRound.MAX_DAY);
        private readonly Dictionary<Farming.SpawnFacility, List<Transform>> _framingPos =  new Dictionary<Farming.SpawnFacility, List<Transform>>();


        [Header("순찰")] [SerializeField] private Transform patrolRoot;
        [Space(5)]
        [Header("문")] [SerializeField] private Transform doorRoot;
        [Space(5)]
        [Header("Put")] [SerializeField] private Transform putRoot;
        [Space(5)]
        [Header("Interact")] [SerializeField] private Transform interactRoot;
        [Space(5)]
        [Header("시나리오")] [SerializeField] private Transform scenarioRoot;
        [Space(20)] 
        [Header("=== 파밍 ===")]
        [SerializeField] private Transform AlwaysSpawnArea;
        [SerializeField] private Transform SecurityRoom;
        [SerializeField] private Transform MainHall;
        [SerializeField] private Transform MainLobby;
        [SerializeField] private Transform Office;
        [SerializeField] private Transform Main3FHall;
        [SerializeField] private Transform Sub3FHall;
        [SerializeField] private Transform Main2FHall;
        [SerializeField] private Transform Sub2FHall;
        [SerializeField] private Transform DiningRoom;
        [SerializeField] private Transform SubLobby;
        [SerializeField] private Transform Cafe;
        

        #region 원본 오브젝트 정보

        private PatrolFieldComponent[] _patrolFieldComponents;
        private DoorComponent[] _doorComponents;
        private ExitDoor _exitDoor;
        private PutBaseComponent[] _putBaseComponents;
        private InteractObjectBaseComponent[] _interactObjectBaseComponents;
        [Space(30)]
        [SerializeField] private ScenarioScriptableDataBase scenarioObjectDB;

        #endregion
        
        #region 활성화된 오브젝트 정보

        private readonly List<IPut> _putObjects = new List<IPut>();
        private readonly List<PatrolFieldComponent> _patrolFieldObjects = new List<PatrolFieldComponent>();
        private readonly List<InteractObjectBaseComponent> _interactObjects = new List<InteractObjectBaseComponent>();
        private readonly List<GameObject> _scenarioObjects = new List<GameObject>();

        #endregion

        #region 퀘스트 이벤트 핸들러
        
        private event TaskHandler.TaskEventHandler _interactEventHandler;
        private event TaskHandler.PatrolEventHandler _patrolEventHandler;
        
        #endregion

        /// <summary>
        /// 이벤트 초기화 및 이벤트 관련 오브젝트 초기화 (1회 호출)
        /// </summary>
        public void InitializationObject(TaskHandler.TaskEventHandler interactEventHandler, TaskHandler.PatrolEventHandler patrolEventHandler)
        {
            _interactEventHandler -= interactEventHandler;
            _interactEventHandler += interactEventHandler;

            _patrolEventHandler -= patrolEventHandler;
            _patrolEventHandler += patrolEventHandler;

            // 파밍 JSON 읽기
            List<Farming.DayData> framingData = JsonFileHelper.ReadJsonAssetFile<List<Farming.DayData>>(_FARMING_JSON_PATH);
            foreach (var day in framingData)
            {
                if (!_framingParse.ContainsKey(day.Nday))
                {
                    _framingParse.Add(day.Nday, day.Items.ToArray());
                }
                else
                {
                    NoManualUtilsHelper.EditorDebugLog(NoManualUtilsHelper.LogTextColor.red, $"Framing JSON Day 값 : {day.Nday} 중복!");
                }
            }

            // 파밍 위치 캐싱
            _framingPos.Add(Farming.SpawnFacility.AlwaysSpawnArea, GetChildTransforms(AlwaysSpawnArea));
            _framingPos.Add(Farming.SpawnFacility.SecurityRoom, GetChildTransforms(SecurityRoom));
            _framingPos.Add(Farming.SpawnFacility.MainHall, GetChildTransforms(MainHall));
            _framingPos.Add(Farming.SpawnFacility.MainLobby, GetChildTransforms(MainLobby));
            _framingPos.Add(Farming.SpawnFacility.Office, GetChildTransforms(Office));
            _framingPos.Add(Farming.SpawnFacility.Main3FHall, GetChildTransforms(Main3FHall));
            _framingPos.Add(Farming.SpawnFacility.Sub3FHall, GetChildTransforms(Sub3FHall));
            _framingPos.Add(Farming.SpawnFacility.Main2FHall, GetChildTransforms(Main2FHall));
            _framingPos.Add(Farming.SpawnFacility.Sub2FHall, GetChildTransforms(Sub2FHall));
            _framingPos.Add(Farming.SpawnFacility.DiningRoom, GetChildTransforms(DiningRoom));
            _framingPos.Add(Farming.SpawnFacility.SubLobby, GetChildTransforms(SubLobby));
            _framingPos.Add(Farming.SpawnFacility.Cafe, GetChildTransforms(Cafe));


            if (patrolRoot)
            {
                _patrolFieldComponents = patrolRoot.GetComponentsInChildren<PatrolFieldComponent>(true);
                foreach (var patrolFieldComponent in _patrolFieldComponents) patrolFieldComponent.InitializationPatrolFieldObject();

                if (_patrolFieldComponents.Length != 0)
                {
                    NoManualUtilsHelper.EditorDebugLog(NoManualUtilsHelper.LogTextColor.cyan,"Patrol 초기화 완료");
                }
            }
            if (doorRoot)
            {
                DoorBaseComponent[] doorBaseComponents = doorRoot.GetComponentsInChildren<DoorBaseComponent>(true);
                _doorComponents = doorBaseComponents.OfType<DoorComponent>().ToArray();
                _exitDoor = doorBaseComponents.OfType<ExitDoor>().FirstOrDefault();
                
                // 퇴근 문 초기화
                if(_exitDoor) _exitDoor.SetNoInteractive();
                foreach (var door in _doorComponents) door.InitializationPutMode();
                
                if (_doorComponents.Length != 0 && _exitDoor)
                {
                    NoManualUtilsHelper.EditorDebugLog(NoManualUtilsHelper.LogTextColor.cyan, "Door 초기화 완료");
                }
            }
            if (putRoot)
            {
                _putBaseComponents = putRoot.GetComponentsInChildren<PutBaseComponent>(true);
                foreach(var putObject in _putBaseComponents) putObject.InitializationPutMode();
                
                if (_putBaseComponents.Length != 0)
                {
                    NoManualUtilsHelper.EditorDebugLog(NoManualUtilsHelper.LogTextColor.cyan, "Put 초기화 완료");
                }
            }
            if (interactRoot)
            {
                _interactObjectBaseComponents = interactRoot.GetComponentsInChildren<InteractObjectBaseComponent>(true);
                foreach (var interactObject in _interactObjectBaseComponents)
                {
                    interactObject.InitHighLightIcon();
                    interactObject.InitializationInteractObject();
                }
                
                if (_interactObjectBaseComponents.Length != 0)
                {
                    NoManualUtilsHelper.EditorDebugLog(NoManualUtilsHelper.LogTextColor.cyan, "Interact 초기화 완료");
                }
            }
            
        }
        
        /// <summary>
        /// 객실 상태에 따라 Door 관리 (1회 호출 일차마다 달라짐)
        /// </summary>
        public void GuestRoomDoorHandler(GuestRoom[] guestRoom)
        {
            foreach (var room in guestRoom)
            {
                string roomNumber = room.roomNumber.ToString();
                DoorComponent door =_doorComponents.FirstOrDefault(doorComponent => doorComponent.TaskTargetId.Equals(roomNumber));
                if(!door) continue;
                var roomState = (UI_GuestRoomItem.RoomState)room.roomState;
                switch (roomState)
                {
                    // 문 잠그기
                    case UI_GuestRoomItem.RoomState.Stay or UI_GuestRoomItem.RoomState.Forbidden: door.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock); break;
                    // 문 닫기
                    default: door.DoorHandler(DoorBaseComponent.DoorStateEnum.Close); break;
                }
               // Debug.Log($"Door({roomState} = {room.roomNumber} = {door.doorState})");
            }
        }

        #region 호텔 맵 오브젝트 초기화 (매 라운드 마다 호출)
        
            /// <summary>
            /// 모든 오브젝트 초기화
            /// </summary>
            public void ResetAllObject()
            {
                AllResetIPutObject();
                ResetScenarioObject();
                ResetPatrol();
                ResetInteract();

                NoManualUtilsHelper.EditorDebugLog(NoManualUtilsHelper.LogTextColor.yellow, "Put / 시나리오 / Patrol / Interact 리셋 완료");
            }
        
            /// <summary>
            /// Std 퀘스트에 필요한 오브젝트 할당
            /// </summary>
            public void SetStdTaskObject(TaskHandler.StandardTask stdTask)
            {
                switch (stdTask.taskType)
                {
                    case TaskHandler.TaskType.Patrol : SetPatrolObject(stdTask.taskData as PatrolTask); break;
                    
                    case TaskHandler.TaskType.Put : // Door 과 Put 동시에 처리
                        if (stdTask.taskData is PutTask putTask)
                        {
                            if (putTask.taskId.Equals(TaskHandler.TaskID.Put_Inventory_Lock_Door.ToString()))
                            {
                                SetDoorObject(putTask.taskTargetId, putTask.putRequireItemId);
                            }
                            else
                            {
                                SetPutObject(putTask.taskTargetId, putTask.putRequireItemId);
                            }
                        }
                        break;
                    
                    case TaskHandler.TaskType.Interaction : SetInteractObject(stdTask.taskData.taskTargetId); break;
                    default: return;
                }
            }

            /// <summary>
            /// 시나리오 퀘스트에 필요한 오브젝트 할당
            /// </summary>
            public void SetScenarioTaskObject(ScenarioTask scenarioTask)
            {
                SetScenarioObject(scenarioTask);
            }
            

            #region Iput

            /// <summary>
            /// Put 오브젝트 초기화
            /// </summary>
            private void AllResetIPutObject()
            {
                foreach (var iPut in _putObjects) iPut.InitializationPutMode();
                _putObjects.Clear();
                
            }
            
            /// <summary>
            /// Put 오브젝트 할당
            /// </summary>
            private void SetIPutObject(IPut putObject)
            {
                _putObjects.Add(putObject);
                putObject.SwapPutMode();
            }

            #endregion

            #region Patrol

            /// <summary>
            /// Patrol 오브젝트 할당
            /// </summary>
            public void SetPatrolObject(PatrolTask task)
            {
                if (task == null) return;
                PatrolFieldComponent patrolObject = _patrolFieldComponents.FirstOrDefault(patrolFieldComponent => patrolFieldComponent.TargetTaskID.Equals(task.taskTargetId));
                if (patrolObject)
                {
                    patrolObject.gameObject.SetActive(true);
                    patrolObject.InitializationPatrolFieldObject();
                    if(task.taskId.Equals(TaskHandler.TaskID.Patrol_Room_Field.ToString())) patrolObject.RoomState = task.roomState;
                    _patrolFieldObjects.Add(patrolObject);
                }
            }
            
            /// <summary>
            /// Patrol 오브젝트 초기화
            /// </summary>
            public void ResetPatrol()
            {
                foreach (var patrolObject in _patrolFieldObjects)
                {
                    patrolObject.gameObject.SetActive(false);
                    patrolObject.InitializationPatrolFieldObject();
                }
                _patrolFieldObjects.Clear();
            }

            #endregion
            
            #region Door

            /// <summary>
            /// Door 오브젝트 할당 (초기화는 IPut에서 진행)
            /// </summary>
            private void SetDoorObject(string doorId, string[] requestItemId)
            {
                DoorComponent door = _doorComponents.FirstOrDefault(doorComponent => doorComponent.TaskTargetId.Equals(doorId));
                if (door)
                {
                    // string 배열을 int 배열로 변환
                    door.RequestItemId = requestItemId.Select(int.Parse).ToArray();
                    SetIPutObject(door);
                }
            }

            /// <summary>
            /// 특정 Door 관리
            /// </summary>
            public void DoorHandler(string doorId, DoorBaseComponent.DoorStateEnum targetState)
            {
                DoorComponent door =_doorComponents.FirstOrDefault(doorComponent => doorComponent.TaskTargetId.Equals(doorId));
                if (door)
                {
                    door.DoorHandler(targetState);
                }
            }

            #endregion

            #region Put

            /// <summary>
            /// Put 컴포넌트 오브젝트 할당 (초기화는 IPut에서 진행)
            /// </summary>
            public void SetPutObject(string putId, string[] requestItemId)
            {
                PutBaseComponent putObject = _putBaseComponents.FirstOrDefault(putBaseComponent => putBaseComponent.TaskTargetId.Equals(putId));
                if (putObject)
                {
                    // 이미 요구되는 오브젝트가 있는경우 배열을 합침
                    if (putObject.RequestItemId != null && putObject.RequestItemId.Length >= 1)
                    {
                        // 기존 배열과 새로운 requestItemId 배열을 합침
                        putObject.RequestItemId = putObject.RequestItemId.Concat(requestItemId.Select(int.Parse)).ToArray();
                    }
                    else
                    {
                        // string 배열을 int 배열로 변환
                        putObject.RequestItemId = requestItemId.Select(int.Parse).ToArray();
                    }
                    SetIPutObject(putObject.putInterface);
                }
            }
            
            #endregion

            #region Interact

            /// <summary>
            /// Interact 오브젝트 할당
            /// </summary>
            public void SetInteractObject(string interactId)
            {
                InteractObjectBaseComponent interactObject = _interactObjectBaseComponents.FirstOrDefault(interactComponent => interactComponent.GetTargetTaskId.Equals(interactId));
                if (interactObject)
                {
                    interactObject.TaskIdleMode(_interactEventHandler);
                    _interactObjects.Add(interactObject);
                }
            }
            
            /// <summary>
            /// Interact 오브젝트 초기화
            /// </summary>
            public void ResetInteract()
            {
                foreach (var interactObject in _interactObjects)
                {
                    interactObject.InitializationInteractObject();
                }
                _interactObjects.Clear();
            }

            #endregion

            #region Farming

            /// <summary>
            /// 파밍 아이템 초기화 (요일? 매 라운드?)
            /// </summary>
            public void ResetFarming()
            {
            
            }

            #endregion

            #region Scenario

            /// <summary>
            /// 시나리오 오브젝트 할당
            /// </summary>
            private void SetScenarioObject(ScenarioTask scenarioTask)
            {
                GameObject scObject = scenarioObjectDB.GetScenarioObject(scenarioTask.RootId);
                if (!scObject) return;
                
                scObject = CreatePrefabObject(scObject, scenarioRoot);
                _scenarioObjects.Add(scObject);
                
                switch (scenarioTask.ScTask.taskType)
                {
                    case TaskHandler.TaskType.Patrol :
                        PatrolFieldComponent patrol = scObject.GetComponentInChildren<PatrolFieldComponent>();
                        if (patrol)
                        {
                            patrol.gameObject.SetActive(true);
                            patrol.InitializationPatrolFieldObject();
                        }
                        break;
                    
                    case TaskHandler.TaskType.Put : 
                        if (scenarioTask.ScTask.taskData is PutTask putTask)
                        {
                            if (putTask.taskId.Equals(TaskHandler.TaskID.Put_Inventory_Lock_Door.ToString()))
                            {
                                DoorComponent door = scObject.GetComponentInChildren<DoorComponent>();
                                if (door)
                                {
                                    door.RequestItemId =  putTask.putRequireItemId.Select(int.Parse).ToArray();
                                    door.SwapPutMode();
                                }
                            }
                            else
                            {
                                PutBaseComponent put = scObject.GetComponentInChildren<PutBaseComponent>();
                                if (put)
                                {
                                    put.RequestItemId =  putTask.putRequireItemId.Select(int.Parse).ToArray();
                                    put.putInterface.SwapPutMode();
                                }
                            }
                        }
                        break;
                    
                    case TaskHandler.TaskType.Interaction :
                        InteractObjectBaseComponent interactObject = scObject.GetComponentInChildren<InteractObjectBaseComponent>();
                        if (interactObject) interactObject.TaskIdleMode(_interactEventHandler);
                        break;
                    
                    default: return;
                }
            }

            /// <summary>
            /// 시나리오 오브젝트 초기화
            /// </summary>
            private void ResetScenarioObject()
            {
                foreach (Transform scObject in scenarioRoot)
                {
                    Destroy(scObject.gameObject);
                }
                _scenarioObjects.Clear();
            }

            #endregion

            #endregion

         private GameObject CreatePrefabObject(GameObject prefab, Transform root)
         {
            return Instantiate(prefab, root);
         }

         #region 파밍

         
         /// <summary>
         /// 파밍 아이템 생성 (1회 호출)
         /// </summary>
         public void CreateFarmingObject(int day)
         {
             foreach (var itemData in _framingParse[day])
             {
                 // 배치 확률 확인
                 if (itemData.SpawnType == Farming.SpawnType.DefaultSpawn &&
                     itemData.SpawnItemData.SpawnPer < UnityEngine.Random.value)
                 {
                     continue; // 배치 확률을 통과하지 못한 경우 다음 아이템으로
                 }

                 // 캐싱된 위치 리스트에서 랜덤하게 위치 선택
                 if (_framingPos.TryGetValue(itemData.SpawnFacility, out var availablePositions) && availablePositions.Count > 0)
                 {
                     Transform spawnPos = GetAvailablePosition(availablePositions);
                     if (spawnPos)
                     {
                         // 아이템 생성 및 초기화
                         var itemCloneData = NoManualHotelManager.Instance.InventoryManager.GetItemCloneData(itemData.SpawnItemData.SpawnItem);
                         var itemComponent = Instantiate(itemCloneData.dropPrefab, spawnPos).GetComponent<ItemComponent>();
                         itemComponent.InitializedItemComponent(itemCloneData.itemId, 1);

                         // 아이템 하이라이팅
                         if (itemData.SpawnType == Farming.SpawnType.TutorialSpawn)
                         {
                             HighLightIcon highLightIcon = itemComponent.GetComponentInChildren<HighLightIcon>();
                             if (highLightIcon) highLightIcon.SetItemHighLight();
                         }
                     }
                     else
                     {
                         // 모든 위치를 시도했으나 사용 가능한 위치가 없던 경우
                         ErrorCode.SendError($"Day: {day}, SpawnType: {itemData.SpawnType}", ErrorCode.ErrorCodeEnum.FullFramingPos);
                     }
                 }
                 else
                 {
                     // 모든 위치가 소진된 경우
                     ErrorCode.SendError($"Day: {day}, SpawnType: {itemData.SpawnType}", ErrorCode.ErrorCodeEnum.FullFramingPos);
                 }
             }
         }
         
         /// <summary>
         /// 파밍 사용 가능한 위치를 랜덤하게 선택
         /// </summary>
         private Transform GetAvailablePosition(List<Transform> availablePositions)
         {
             int attemptCount = 0;

             while (attemptCount < availablePositions.Count)
             {
                 int randomIndex = UnityEngine.Random.Range(0, availablePositions.Count);
                 Transform spawnPos = availablePositions[randomIndex];

                 // 사용 중이지 않음
                 if (spawnPos.childCount == 0)
                 {
                     // 사용한 위치 제거
                     availablePositions.RemoveAt(randomIndex);
                     return spawnPos; // 사용 가능한 위치 반환
                 }
                 attemptCount++;
             }

             // 사용 가능한 위치가 없는 경우
             return null;
         }
         
        /// <summary>
        /// 파밍 아이템 위치 얻기
        /// </summary>
         private List<Transform> GetChildTransforms(Transform parent)
        {
             int childCount = parent.childCount;
             List<Transform> children = new List<Transform>(childCount);
             for (int i = 0; i < childCount; i++)
             {
                 children.Add(parent.GetChild(i));
             }
             return children;
         }
         

         #endregion


         #region 퇴근 문 (마지막 라운드에서 호출)

         public void ActiveExitDoor()
         {
             OutLineUtil.AddOutLine(_exitDoor.gameObject, QuickOutline.Mode.OutlineAll, Color.white, 1.5f);
              _exitDoor.SetYesInteractive();
         }

         #endregion
         

         #region 퀘스트 이벤트 트리거

         /// <summary>
         /// 순찰 지역 트리거 이벤트
         /// </summary>
         public void PatrolFieldEventTrigger(string taskId, string targetTaskId, UI_GuestRoomItem.RoomState roomState)
         {
             _patrolEventHandler?.Invoke(taskId, targetTaskId, roomState);
         }

         [ContextMenu("순찰 퀘스트 강제 클리어")]
         public void PatrolAllClear()
         {
             foreach (var patrol in _patrolFieldObjects)
             {
                 patrol.ClearPatrol();
             }
         }

         #endregion
        
    } 
    
}


