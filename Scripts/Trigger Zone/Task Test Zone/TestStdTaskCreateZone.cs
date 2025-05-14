using System;
using System.Collections;
using System.Collections.Generic;
using NoManual.ANO;
using NoManual.Interaction;
using NoManual.Inventory;
using NoManual.Managers;
using NoManual.Patrol;
using NoManual.Task;
using NoManual.UI;
using UnityEngine;
using Random = System.Random;

[RequireComponent(typeof(BoxCollider))]
public class TestStdTaskCreateZone : MonoBehaviour
{
     private int taskTableId;
    [SerializeField] private bool placeHolder;
    [SerializeField] private LocalizationTable.TextTable placeTextTable;
    [SerializeField] private TaskHandler.TaskType taskType;
    [SerializeField] private TaskHandler.TaskID taskID;
    [SerializeField] private string taskTargetId;

    [SerializeField] private Transform prefabCreateRoot;
    [SerializeField] private Transform prefabCreateRoot2;
    [SerializeField] private ItemDataBaseScriptable itemDB;
    [SerializeField] private GameObject PC;
    [SerializeField] private InteractObjectBaseComponent[] interactObj;
    [SerializeField] private ANO_DataBaseScriptbale anoDB;
    [SerializeField] private List<GameObject> TestPatrolFieldPrefab = new List<GameObject>();


    private bool _inTrigger = false;
    [SerializeField] private List<GameObject> testPutObject;
    
    public void CreateStdTask()
    {
        TaskHandler taskHandler = HotelManager.Instance.TaskHandler;
        Nomanual_PC_OS_Manager nomanualPCOSManager = HotelManager.Instance.PcOSManager;
      

        TaskHandler.StandardTask testStdTask = new TaskHandler.StandardTask()
        {
            taskTableId = taskTableId,
            placeHolder = placeHolder,
            placeHolderTextTableKey = placeTextTable,
            taskType = taskType,
            isClear = false,
        };

        TaskData taskData = null;
        switch (taskType)
        {
            case TaskHandler.TaskType.Get:
                List<int> itemListId = new List<int>();
                foreach (var item in itemDB.itemDataBase)
                {
                    itemListId.Add(item.itemId);
                }

                int itemId = itemListId[UnityEngine.Random.Range(0, itemListId.Count)];
                GetTask getTask = new GetTask()
                {
                    taskId = taskID.ToString(),
                    taskTargetId = itemId.ToString(),
                };
                testStdTask.taskTableId = 10;
                taskData = getTask;
                CreateInventoryItemPrefab(itemDB.GetItemDataToItemId(itemId).dropPrefab, prefabCreateRoot);
                break;
            
            case TaskHandler.TaskType.Interaction:

                for (int i = 0; i < interactObj.Length; i++)
                {
                    interactObj[i].InitializationInteractObject();

                    TaskHandler.StandardTask testStdTask2 = new TaskHandler.StandardTask()
                    {
                        taskTableId = 12,
                        placeHolder = true,
                        placeHolderTextTableKey = LocalizationTable.TextTable.Object_Item_Table,
                        taskType = TaskHandler.TaskType.Interaction,
                    };

                    InteractionTask interactionTask = new InteractionTask()
                    {
                        taskId = TaskHandler.TaskID.Interact_Object.ToString(),
                        taskTargetId = interactObj[i].GetTargetTaskId,
                    };
                    interactObj[i].TaskIdleMode(taskHandler.InteractTaskCheckHandler);
                    testStdTask2.taskData = interactionTask;
                    taskHandler.CreateTestStdTask(testStdTask2);
                }
                return;
   
 
            case TaskHandler.TaskType.Patrol:

                GameObject patrolFieldObject = TestPatrolFieldPrefab[UnityEngine.Random.Range(0, TestPatrolFieldPrefab.Count)];
                PatrolTask patrolTask = new PatrolTask()
                {
                    taskId = patrolFieldObject.GetComponent<PatrolFieldComponent>().TaskID,
                    taskTargetId = patrolFieldObject.GetComponent<PatrolFieldComponent>().TargetTaskID,
                    roomState =  UI_GuestRoomItem.RoomState.None,
                };
                
                if (patrolTask.taskId.Equals(TaskHandler.TaskID.Patrol_Room_Field.ToString()))
                {
                    testStdTask.taskTableId = 4;
                }
                else if (patrolTask.taskId.Equals(TaskHandler.TaskID.Patrol_Floor_Field.ToString()))
                {
                    testStdTask.taskTableId = 3;
                }
                else
                {
                    testStdTask.taskTableId = 5;
                    testStdTask.placeHolderTextTableKey = LocalizationTable.TextTable.Facility_Item_Table;
                }
                
                taskData = patrolTask;
                CreatePatrolFieldPrefab(patrolFieldObject, prefabCreateRoot);
                break;
            
            case TaskHandler.TaskType.Put:

                GameObject putObject = Instantiate(testPutObject[UnityEngine.Random.Range(0, testPutObject.Count)], prefabCreateRoot);
                PutTask putTask = new PutTask();

                int max = UnityEngine.Random.Range(0, itemDB.itemDataBase.Count);
                List<int> allItemListId = new List<int>();
                foreach (var item in itemDB.itemDataBase)
                {
                    allItemListId.Add(item.itemId);
                }

                List<int> requestItemIdList = new List<int>(max);
                for (int i = 0; i < max; i++)
                {
                    int selectItemId = allItemListId[UnityEngine.Random.Range(0, itemDB.itemDataBase.Count)];
                    CreateInventoryItemPrefab(itemDB.GetItemDataToItemId(selectItemId).dropPrefab, prefabCreateRoot2);
                    requestItemIdList.Add(selectItemId);
                }

                DoorComponent door = putObject.GetComponentInChildren<DoorComponent>();
                if (door != null)
                {
                    if (UnityEngine.Random.Range(0, 2) == 0)
                    {
                        testStdTask.placeHolderTextTableKey = LocalizationTable.TextTable.None;
                        door.TaskTargetId = UnityEngine.Random.Range(201, 434).ToString();
                    }
                    else
                    {
                        testStdTask.placeHolderTextTableKey = LocalizationTable.TextTable.Facility_Item_Table;
                        door.TaskTargetId = UnityEngine.Random.Range(0, 3).ToString();
                    }
            
                    putTask.taskId = TaskHandler.TaskID.Put_Inventory_Lock_Door.ToString();
                    putTask.taskTargetId = door.TaskTargetId;
                    testStdTask.taskTableId = 13;
                    door.RequestItemId = requestItemIdList.ToArray();
                    putTask.putRequireItemId = Array.ConvertAll(door.RequestItemId, i => i.ToString());
                    door.CurrentPutMode = IPut.PutMode.Put;
                }
                else
                {
                    PutRelayComponent putRelay = putObject.GetComponent<PutRelayComponent>();
                    if (putRelay != null)
                    {
                        putTask.taskId = TaskHandler.TaskID.Put_Inventory_Item_Relay.ToString();
                        testStdTask.placeHolderTextTableKey = LocalizationTable.TextTable.Object_Item_Table;
                        putTask.taskTargetId = putRelay.TaskTargetId;
                        testStdTask.taskTableId = 14;
                        putRelay.RequestItemId = requestItemIdList.ToArray();
                        putTask.putRequireItemId = Array.ConvertAll(putRelay.RequestItemId, i => i.ToString());
                        putRelay.CurrentPutMode = IPut.PutMode.Put;
                    }
                    else
                    {
                        PutReturnComponent putReturn = putObject.GetComponent<PutReturnComponent>();
                        if (putReturn != null)
                        {
                            putTask.taskId = TaskHandler.TaskID.Put_Inventory_Item_Return.ToString();
                            testStdTask.placeHolderTextTableKey = LocalizationTable.TextTable.Inventory_Item_Table;
                            putTask.taskTargetId = putReturn.TaskTargetId;
                            testStdTask.taskTableId = 11;
                            putReturn.RequestItemId = requestItemIdList.ToArray();
                            putTask.putRequireItemId = Array.ConvertAll(putReturn.RequestItemId, i => i.ToString());
                            putReturn.CurrentPutMode = IPut.PutMode.Put;
                        }
                    }
                }

                taskData = putTask;
                break;
            
            case TaskHandler.TaskType.Reserve:
                PC.transform.position = prefabCreateRoot.position;
                PC.transform.rotation = prefabCreateRoot.rotation;

                ReserveTask reserveTask = new ReserveTask();
                
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    testStdTask.taskTableId = 6;
                    testStdTask.placeHolder = false;
                    // 모든 예약자 처리
                    reserveTask.taskId = TaskHandler.TaskID.Reserve_All_Clear.ToString();
                    nomanualPCOSManager.ohHomePage.occasionHotelHomePageHelper.AddRandomReservationGuest();
                }
                else
                {
                    testStdTask.taskTableId = 7;
                    testStdTask.placeHolderTextTableKey = LocalizationTable.TextTable.Reserve_Item_Table;
                    testStdTask.placeHolder = true;
                    // 특정 예약자 처리
                    reserveTask.taskId = TaskHandler.TaskID.Reserve_Target_Clear.ToString();
                    reserveTask.taskTargetId = "TargetReserv_" + UnityEngine.Random.Range(0, 10).ToString();
                    nomanualPCOSManager.ohHomePage.occasionHotelHomePageHelper.AddReservationGuest( reserveTask.taskTargetId);
                }

                taskData = reserveTask;
                break;
            case TaskHandler.TaskType.PopUp:

                PC.transform.position = prefabCreateRoot.position;
                PC.transform.rotation = prefabCreateRoot.rotation;
                PopUpTask popUpTask = new PopUpTask();
                
                
                // App Connect
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    popUpTask.taskId = TaskHandler.TaskID.App_Connect.ToString();
                    popUpTask.taskTargetId = UnityEngine.Random.Range(0, 5).ToString();
                    testStdTask.placeHolder = true;
                    testStdTask.placeHolderTextTableKey = LocalizationTable.TextTable.PC_OS_App_Item_Table;
                    testStdTask.taskTableId = 1; 
                }
                //PC Connect
                else
                {
                    popUpTask.taskId = TaskHandler.TaskID.PC_Connect.ToString();
                    int pcId = (int)Nomanual_PC_OS_Manager.PopupID.PC_OS;
                    popUpTask.taskTargetId = pcId.ToString();
                    testStdTask.placeHolder = false;
                    testStdTask.taskTableId = 0;
                }
                taskData = popUpTask;
        
                break;
            case TaskHandler.TaskType.TradersBuy:

                PC.transform.position = prefabCreateRoot.position;
                PC.transform.rotation = prefabCreateRoot.rotation;
                nomanualPCOSManager.rsTradersStruct.rsTradersDeliveryTransform = prefabCreateRoot2;
                
                List<int> itemDB_idList = new List<int>();
                foreach (var item in itemDB.itemDataBase)
                {
                    itemDB_idList.Add(item.itemId);
                }

                // 랜덤으로 구매필요한 아이템 갯수 얻기 (아이템이 중복으로 선택되지 않게 하기)
                var itemIdList = NoManual.Utils.NoManualUtilsHelper.SelectRandomItems(itemDB_idList, UnityEngine.Random.Range(1, itemDB_idList.Count));

                // R.S 트레이더스에 모든 아이템 목록 넣기
                for (int i = 0; i < itemDB_idList.Count; i++)
                {
                    nomanualPCOSManager.Add_RS_TradersItem(itemDB_idList[i]);
                }
                
                // itemIdList 정보에 따라 퀘스트 생성
                for (int i = 0; i < itemIdList.Count; i++)
                {
                    TaskHandler.StandardTask testStdTask2 = new TaskHandler.StandardTask()
                    {
                        taskTableId = 8,
                        placeHolder = true,
                        placeHolderTextTableKey = LocalizationTable.TextTable.Inventory_Item_Table,
                        taskType = TaskHandler.TaskType.TradersBuy,
                    };

                    TradersBuyTask tradersBuyTask = new TradersBuyTask()
                    {
                        taskId = TaskHandler.TaskID.Traders_Buy.ToString(),
                        taskTargetId = itemIdList[i].ToString(),
                    };
                    
                    testStdTask2.taskData = tradersBuyTask;
                    taskHandler.CreateTestStdTask(testStdTask2);
                }
                
                // R.S 트레이더스 물품 받기 퀘스트 생성
                GetTask deliveryTask = new GetTask()
                {
                    taskId = TaskHandler.TaskID.Get_Delivery_Item.ToString(),
                };

                testStdTask.taskTableId = 9;
                testStdTask.placeHolder = false;
                testStdTask.placeHolderTextTableKey = LocalizationTable.TextTable.None;
                testStdTask.taskType = TaskHandler.TaskType.Get;
                taskData = deliveryTask;
                
                break;
            
            case TaskHandler.TaskType.ANO:
                
                /* 1. ANO 프리팹 생성 (최대 2개) */
                
                // 랜덤으로 구매필요한 아이템 갯수 얻기 (아이템이 중복으로 선택되지 않게 하기)
                var anoIdList = NoManual.Utils.NoManualUtilsHelper.SelectRandomItems(anoDB.anoDataBase, UnityEngine.Random.Range(1, 3));
                bool first = true;
                foreach (var ano in anoIdList)
                {
                    var anoComponent = Instantiate(ano.ANOPrefab, first ? prefabCreateRoot : prefabCreateRoot2).GetComponent<ANO_Component>();
                    anoComponent.InitializeANO(new ANO_CloneData_Legacy(ano), ANORatingType.DangerANO, -1);
                    first = false;
    
                    /* ANO 추가근무 퀘스트 */
                    TaskHandler.BonusTask bonusTask = new TaskHandler.BonusTask(ano.TaskType, ano.ANOId.ToString());
                    taskHandler.CreateTestBonusTask(bonusTask);
                }
                return;
        }

        testStdTask.taskData = taskData;
        taskHandler.CreateTestStdTask(testStdTask);
    }
    

    private void CreateInventoryItemPrefab(GameObject prefab, Transform root)
    {
        ItemComponent component = Instantiate(prefab, root).GetComponent<ItemComponent>();
        NoManual.Utils.OutLineUtil.AddOutLine(component.gameObject, QuickOutline.Mode.OutlineAll, Color.green, 2f);
        component.itemSettings.itemAmount = 1;
    }

    private void CreatePatrolFieldPrefab(GameObject prefab, Transform root)
    {
        PatrolFieldComponent component = Instantiate(prefab, root).GetComponent<PatrolFieldComponent>();
        component.InitializationPatrolFieldObject();
    }

    private void RemoveTestStdTask()
    {
        foreach (Transform prefab in prefabCreateRoot)
        {
            Destroy(prefab.gameObject);
        }

        if (prefabCreateRoot2 != null)
        {
            foreach (Transform prefab in prefabCreateRoot2)
            {
                Destroy(prefab.gameObject);
            }
        }
    
        
        TaskHandler taskHandler = HotelManager.Instance.TaskHandler;
        taskHandler.RemoveTestStdTask();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player") && !_inTrigger)
        {
            CreateStdTask();
            _inTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player") && _inTrigger)
        {
            if (taskType is TaskHandler.TaskType.Interaction)
            {
                foreach (var t in interactObj) t.InitializationInteractObject();
            }
            RemoveTestStdTask();
            _inTrigger = false;
        }
    }
}
