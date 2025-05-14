using System;
using System.Collections.Generic;
using System.Linq;
using NoManual.Utils;
using NoManual.UI;
using NoManual.Interaction;
using NoManual.Inventory;
using NoManual.Task;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace NoManual.Managers
{
    public enum InventorySlotType
    {
        None = -1,
        Manual = 0,
        BackPack = 1,
        KeyHolder = 2,
        Pouch = 3,
        Length = 4,
    }

    /// <summary>
    /// 인벤토리 컴포넌트
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] private ItemDataBaseScriptable itemDataBaseScriptable;
        public ItemDataBaseScriptable GetItemDB => itemDataBaseScriptable;
        private readonly Dictionary<int, ItemCloneData> _itemCloneDataBase = new Dictionary<int, ItemCloneData>();
        // 인벤토리 아이템 데이터
        private Dictionary<InventorySlotType, List<InventoryItem>> _inventory = new Dictionary<InventorySlotType, List<InventoryItem>>(); 
        // 인벤토리 UI 데이터
        private Dictionary<InventorySlotType, UI_ChildInventoryPanel> _uiCipPanels = new Dictionary<InventorySlotType, UI_ChildInventoryPanel>();
        // 인벤토리 아이템 상호작용 버튼들
        private List<UI_UsageOpContextButton> _uiUsageOpContextButtons = new List<UI_UsageOpContextButton>();
        // 조합 아이템 매핑
        private CombineItem _firstCombineItem, _secondCombineItem;
        // 인벤토리 아이템 복구용 정보
        private List<RemovedItemInfo> _removedItemInfos = new List<RemovedItemInfo>(2)
        {
            new(InventorySlotType.None, -1, -1, 0, null),
            new(InventorySlotType.None, -1, -1, 0, null)
        };
        // 인벤토리 퀵슬롯 UI 데이터
        private Dictionary<string, UI_ShortCutSlot> _inventoryShortCutList = new Dictionary<string, UI_ShortCutSlot>(3);

        #region 획득 아이템 확인 이벤트
        
        // 아이템 ID와 그에 대응하는 조건 및 동작을 저장할 딕셔너리
        private Dictionary<int, Action> _itemCheckActions = new Dictionary<int, Action>();
        public delegate void ItemAddedEventHandler(int itemId);
        public event ItemAddedEventHandler OnItemAdded;
        
        #endregion

        #region Task 퀘스트 클리어 이벤트
        public event TaskHandler.TaskEventHandler GetInventoryItemTaskHandler;
        public event TaskHandler.PutTaskEventHandler PutInventoryItemTaskHandler;
        public event TaskHandler.ManualTaskEventHandler TradersDeliveryTaskHandler;

        #endregion

        // Context 버튼 비활성화
        public bool DisableContextMenu { get; set; } = false;
        // 조합 모드 On / Off
        public bool IsCombineMode { get; private set; } = false;
        // Put 모드  On / Off
        public bool IsPutMode { get; private set; } = false;
        // Put 관련 데이터 매핑
        private PutCallBackMapper _putCallBackMapper = null;
        // 퀵슬롯 모드 On / Off
        public bool IsShortCutMode { get; private set; } = false;
        // 퀵슬롯 관련 데이터 매핑
        public ShortCutData ShortCutDataMapper { get; private set; } = null;
        // 아이템 클릭 여부
        public bool IsSelecting { get; set; } = false; 
        // 선택된 아이템 배낭 Slot Type
        public InventorySlotType SelectInventorySlotTypeSlotType { get; set; } = InventorySlotType.None;
        // 선택된 아이템 UI Slot Id
        public int SelectSlotId { get; set; } = -1;
        public bool IsContextMenuVisible { get; private set; } = false;
        public bool IsDragging { get; set; } = false;
        public InventoryItem ItemToMove { get; set; } = null;

        #region CIP 세팅

        [Serializable]
        public struct CIPHeaderSettings
        {
            [Tooltip("매뉴얼 CIP Icon")]public Sprite manualIcon;
            [Tooltip("매뉴얼 CIP Name")]public string manualText;
            [Tooltip("가방 CIP Icon")]public Sprite backPackIcon;
            [Tooltip("가방 CIP Name")]public string backPackText;
            [Tooltip("키 CIP Icon")]public Sprite keyHolderIcon;
            [Tooltip("키 CIP Name")]public string keyHolderText;
            [Tooltip("주머니 CIP Icon")]public Sprite pouchIcon;
            [Tooltip("주머니 CIP Name")]public string pouchText;
        }

        public CIPHeaderSettings cipHeaderSettings = new CIPHeaderSettings();
        
        [Serializable]
        public struct InventoryPanels
        {
            [Tooltip("PIP Root")] public Transform pipTransform;
            [Tooltip("아이템 설명 창")] public GameObject itemDescription;
            [Tooltip("인벤토리 아이템 상호작용 버튼")] public GameObject itemContextMenu;
        }

        public InventoryPanels inventoryPanels = new InventoryPanels();
        
        [Serializable]
        public struct ContentUiObjects
        {
            [Tooltip("조합 가이드 UI 패널")] public GameObject combineGuidePanel;
            [Tooltip("조합 가이드 텍스트 UI")] public TextMeshProUGUI combineGuideText;
            [Tooltip("아이템 제목")] public TextMeshProUGUI itemTitleText;
            [Tooltip("아이템 설명")] public TextMeshProUGUI itemDescriptionText;
        }

        public ContentUiObjects contentUiObjects = new ContentUiObjects();
        
        [Serializable]
        public struct InventoryCIPPrefabs
        {
            [Tooltip("CIP")] public GameObject cip;
            [Tooltip("CIP Empty Slot")]  public GameObject cipEmptySlot;
            [Tooltip("CIP Preview Slot")]  public GameObject cipPreviewSlot;
        }
        
        public InventoryCIPPrefabs inventoryCipPrefabs = new InventoryCIPPrefabs();
        
        [Serializable]
        public struct InventoryItemContextMenuButtons
        {
            [Tooltip("아이템 사용하기 버튼")] public UI_UsageOpContextButton UsageOp_BindShortcutButton;
            [Tooltip("아이템 읽기 버튼")] public UI_UsageOpContextButton UsageOp_ReadButton;
            [Tooltip("아이템 장착하기 버튼")] public UI_UsageOpContextButton UsageOp_EquipButton;
            [Tooltip("아이템 먹기 버튼")] public UI_UsageOpContextButton UsageOp_EatButton;
            [Tooltip("아이템 조합하기 버튼")] public UI_UsageOpContextButton UsageOp_CombineButton;
            [Tooltip("아이템 버리기 버튼")] public UI_UsageOpContextButton UsageOp_DropButton;
        }

        public InventoryItemContextMenuButtons inventoryItemContextMenuButtons = new InventoryItemContextMenuButtons();

        [Serializable]
        public struct InventoryItemContextBackGroundColor
        {
            [Tooltip("CIP Preview Slot 비활성화 색상")] public Color cipPreviewSlotDisableBackGroundColor;
            [Tooltip("CIP Empty Slot 비활성화 색상")] public Color cipEmptyDisableBackGroundColor;
            [Tooltip("CIP Empty Slot 기본 배경 색상")] public Color cipEmptyDefaultBackGroundColor;
            [Tooltip("CIP Empty Slot 선택 배경 색상")] public Color cipEmptySelectedBackGroundColor;
            [Tooltip("UsageOpContext 버튼 기본 배경 색상")] public Color usageOpContextDefaultBackGroundColor;
            [Tooltip("UsageOpContext 버튼 선택 배경 색상")] public Color usageOpContextSelectedBackGroundColor;
        }

        public InventoryItemContextBackGroundColor inventoryItemContextBackGroundColor = new InventoryItemContextBackGroundColor();
        
        [Serializable]
        public struct InventoryCipEmptySlotBackGroundImages
        {
            [Tooltip("아이템 기본 값 배경 이미지")] public Sprite cipEmptySlotWithItemSprite;
            [Tooltip("아이템 선택 시 배경 이미지")] public Sprite cipEmptySlotSelectSprite;
        }

        public InventoryCipEmptySlotBackGroundImages inventoryCipEmptySlotBackGroundImages = new InventoryCipEmptySlotBackGroundImages();
        
        [Serializable]
        public struct InventoryCapacitySettings
        {
            [Tooltip("매뉴얼 수납공간 크기 (-1 : 무제한)")] public int manualMaxCapacity;
            [Tooltip("가방 수납공간 크기 (-1 : 무제한)")]  public int backPackMaxCapacity;
            [Tooltip("키 수납공간 크기 (-1 : 무제한)")] public int keyHolderMaxCapacity;
            [Tooltip("주머니 수납공간 크기 (-1 : 무제한)")] public int pouchMaxCapacity;
        }

        public InventoryCapacitySettings inventorCapacitySettings = new InventoryCapacitySettings();

        [Serializable]
        public struct ShortCutImages
        {
            [Tooltip("1번 퀵슬롯 이미지")] public Sprite shortCut1;
            [Tooltip("2번 퀵슬롯 이미지")] public Sprite shortCut2;
            [Tooltip("3번 퀵슬롯 이미지")] public Sprite shortCut3;
            [Tooltip("4번 퀵슬롯 이미지")] public Sprite shortCut4;
        }
        
        [Header("퀵슬롯 이미지 리소스")] 
        public ShortCutImages shortCutImages = new ShortCutImages();
        
        [Serializable]
        public struct InventoryHUDPanel
        {
            [Tooltip("플레이어 정신력 이미지 BG")] public Image mentalityBgImage;
            [Tooltip("플레이어 정신력 이미지")] public Image mentalityImage;
            [Tooltip("퀵슬롯 프리팹")] public GameObject shortCutSlotPrefab;
            [Tooltip("퀵슬롯 프리팹 생성위치")] public Transform shortcutPrefabRoot;
        }

        public InventoryHUDPanel inventoryHUDPanel = new InventoryHUDPanel();
        #endregion
        
        

        [Header("=== 인벤토리 아이템 세이브 테스트 ===")] 
        [Space(10)] 
        [SerializeField] private InventorySaveData[] testInventorySaveData;
        
        /// <summary>
        /// 인벤토리 초기화
        /// </summary>
        public void InitInventory()
        {
            // 아이템 DB 생성
            for (int i = 0; i < itemDataBaseScriptable.itemDataBase.Count; i++)
            {
                var cloneItemData = new ItemCloneData(itemDataBaseScriptable.itemDataBase[i]);
                cloneItemData.itemTitle = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.Inventory_Item_Table,
                                                                                         LocalizationTable.Inventory_Item_TableTextKey.Inventory_Item_.ToString() 
                                                                                         + cloneItemData.itemId.ToString());
                cloneItemData.description = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.Inventory_Item_Description_Table,
                                                                                            LocalizationTable.Inventory_Item_TableTextKey.Inventory_Item_Description_.ToString() 
                                                                                            + cloneItemData.itemId.ToString());
                
              //  Debug.Log(cloneItemData.itemTitle + " / " + cloneItemData.description);
                _itemCloneDataBase.Add(cloneItemData.itemId, cloneItemData);
            }
            
            // 인벤토리 데이터 리스트 생성
            _inventory.Add(InventorySlotType.Manual, new List<InventoryItem>());
            _inventory.Add(InventorySlotType.KeyHolder, new List<InventoryItem>());
            _inventory.Add(InventorySlotType.BackPack,  new List<InventoryItem>());
            _inventory.Add(InventorySlotType.Pouch, new List<InventoryItem>());

            
            // CIP 생성
            for (int i = 0; i < (int)InventorySlotType.Length; i++)
            {
                InventorySlotType type = (InventorySlotType)i;
                string cipName = null;
                Sprite cipIcon = null;

                // CIP 헤더 설정
                switch (type)
                {
                    case InventorySlotType.Manual:
                        cipName = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.UI_Table, cipHeaderSettings.manualText); 
                        cipIcon = cipHeaderSettings.manualIcon;
                        break;
                    case InventorySlotType.BackPack:
                        cipName = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.UI_Table, cipHeaderSettings.backPackText);
                        cipIcon = cipHeaderSettings.backPackIcon;
                        break;
                    case InventorySlotType.Pouch:
                        cipName = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.UI_Table, cipHeaderSettings.pouchText);
                        cipIcon = cipHeaderSettings.pouchIcon;
                        break;
                    case InventorySlotType.KeyHolder:
                        cipName = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.UI_Table, cipHeaderSettings.keyHolderText);
                        cipIcon = cipHeaderSettings.keyHolderIcon;
                        break;
                }
                
                _uiCipPanels[type] = Instantiate(inventoryCipPrefabs.cip, inventoryPanels.pipTransform).GetComponent<UI_ChildInventoryPanel>();
                _uiCipPanels[type].transform.SetSiblingIndex(i); // 가방 순서로 정렬
                _uiCipPanels[type].InitializeCIPHeader(cipName, cipIcon, type);
                
                // Manual, KeyHolder CIP 패널은 숨김
                if(type is InventorySlotType.Manual or InventorySlotType.KeyHolder)
                    _uiCipPanels[type].gameObject.SetActive(false);

                // BackPack, Pouch CIP는 Empty Slot 미리 최대 생성
                else if (type is InventorySlotType.BackPack or  InventorySlotType.Pouch )
                {
                    int targetMaxCapacity = type is InventorySlotType.BackPack ? inventorCapacitySettings.backPackMaxCapacity : inventorCapacitySettings.pouchMaxCapacity;
                    
                    for (int j = 0; j < targetMaxCapacity; j++)
                    {
                        // CIP 패널에 Empty Slot 등록
                        UI_ChildInventoryPanelEmptySlot cipEmptySlot = Instantiate(inventoryCipPrefabs.cipEmptySlot).GetComponent<UI_ChildInventoryPanelEmptySlot>();
                        _uiCipPanels[type].AddCipEmptySlot(cipEmptySlot, j);
                    }
                }
            }
            
            
            // 퀵슬롯 슬롯 생성
            for (int i = 0; i < ItemShortCutManager.SHORTCUT_INPUTSYSTME_VALUES.Length; i++)
            {
                var inventoryShortCut = Instantiate(inventoryHUDPanel.shortCutSlotPrefab, inventoryHUDPanel.shortcutPrefabRoot).GetComponent<UI_ShortCutSlot>();
                inventoryShortCut.Init();
                inventoryShortCut.SetShortCutBindKeyId(ItemShortCutManager.SHORTCUT_INPUTSYSTME_VALUES[i], GetShortCutSprite(ItemShortCutManager.SHORTCUT_INPUTSYSTME_VALUES[i]));
                _inventoryShortCutList.Add(inventoryShortCut.GetSlotBindKeyId(), inventoryShortCut);
            }
            
            
            
            
            // 조합 아이템 매핑 1번 2번 생성
            _firstCombineItem = new CombineItem();
            _secondCombineItem = new CombineItem();
            
      
            // OnItemAdded 이벤트에 메소드를 연결
            OnItemAdded += CheckAndExecuteActions;
        }
        
        /// <summary>
        /// 인벤토리 및 퀵슬롯 초기화
        /// </summary>
        public void InitializationInventory(InventorySaveData[] saveItemData, ShortCutData[] saveShortCutData)
        {
            // 241013 인벤토리 아이템 세이브 테스트 데이터 전용 (주석가능)
            //saveItemData = testInventorySaveData;
            ResetInventory();
            NoManualHotelManager.Instance.UiNoManualUIManager.ShortCutSlotUiInit();
            
            if(saveItemData == null || saveItemData.Length == 0) return;
            InventoryItem[] inventoryItems = new InventoryItem[saveItemData.Length];
           
            for (int i = 0; i < saveItemData.Length; i++)
            {
                inventoryItems[i] = new InventoryItem(GetItemCloneData(saveItemData[i].ItemId), saveItemData[i].ItemAmount, saveItemData[i].slotId, saveItemData[i].slotType);
            }

            InitHiddenCipEmptySlot(saveItemData);
            LoadInventoryItems(inventoryItems);
            LoadInventoryItemShortCut(saveShortCutData);
        }

        private void InitHiddenCipEmptySlot(InventorySaveData[] saveItemData)
        {
            // 규칙서, 키 CIP 인벤토리 세이브 데이터 개수 만큼미리 생성 
            if (saveItemData == null || saveItemData.Length == 0) return;
            int maxManualSlotCount = 0;
            int maxKeyHolderSlotCount = 0;

            foreach (var itemData in saveItemData)
            {
                if (itemData.slotType == InventorySlotType.Manual) maxManualSlotCount = Mathf.Max(maxManualSlotCount, itemData.slotId);
                else if (itemData.slotType == InventorySlotType.KeyHolder) maxKeyHolderSlotCount = Mathf.Max(maxKeyHolderSlotCount, itemData.slotId);
            }
            
            if(maxManualSlotCount > 0)  for(int i=0; i<=maxManualSlotCount; i++) CreateHiddenCipEmptySlot(InventorySlotType.Manual);
            if(maxKeyHolderSlotCount > 0 )  for(int i=0; i<=maxManualSlotCount; i++) CreateHiddenCipEmptySlot(InventorySlotType.KeyHolder);
            
        }

        /// <summary>
        /// 인벤토리 아이템 세이브 파일 불러오기
        /// </summary>
        private void LoadInventoryItems(InventoryItem[] inventoryItems)
        {
            if (inventoryItems == null || inventoryItems.Length == 0) return;

            foreach (var item in inventoryItems)
            {
                AddItem(item.item, item.itemAmount, false, true, item.slotId, item.slotType);
            }
        }

        /// <summary>
        /// 인벤토리 퀵슬롯 세팅 세이브 파일 불러오기
        /// </summary>
        private void LoadInventoryItemShortCut(ShortCutData[] saveShortCutData)
        {
            if (saveShortCutData == null || saveShortCutData.Length == 0) return;

            foreach (var shortCutData in saveShortCutData)
            {
                NoManualHotelManager.Instance.ItemShortCutManager.BindShortCut(shortCutData.ItemId, 
                                                                                shortCutData.ItemType, 
                                                                                shortCutData.SlotId, 
                                                                                shortCutData.ShortCutKey);
            }
        }

        [ContextMenu("인벤 아이템 세이브 변환")]
        public InventorySaveData[] SaveInventoryItems()
        {
            List<InventorySaveData> allItemList = new List<InventorySaveData>();
            foreach (var itemList in _inventory.Values)
            {
                foreach (var item in itemList)
                {
                    allItemList.Add(new InventorySaveData(item.item.itemId, item.itemAmount, item.slotId, item.slotType));
                }
            }
            return allItemList.ToArray();
        }

        #region 수납공간 확인

        /// <summary>
        ///  아이템 넣기 전 수납공간 확인
        /// </summary>
        private bool InventoryItemMaxCapacityCheck(ItemCloneData cloneData, int itemAmount)
        {
            var targetSlot = TargetSlotType(cloneData.InventoryItemUseType);
            var inventoryCapacity = AllInventoryCapacityCheck();
            int itemCount = 0;

            // 아이템 허용 개수 확인
            if (cloneData.itemSettings.maxBagAmount != 0)
            {
                if (targetSlot is InventorySlotType.BackPack or InventorySlotType.Pouch)
                {
                    // 아이템 백팩 허용 개수 확인
                    for (int i = 0; i < inventoryCapacity[InventorySlotType.BackPack]; i++)
                    {
                        if (_inventory[InventorySlotType.BackPack][i].item != null && _inventory[InventorySlotType.BackPack][i].item.itemId == cloneData.itemId)
                            itemCount++;
                    }
                
                    // 아이템 호주머니 허용 개수 확인
                    for (int i = 0; i < inventoryCapacity[InventorySlotType.Pouch]; i++)
                    {
                        if (_inventory[InventorySlotType.Pouch][i].item != null && _inventory[InventorySlotType.Pouch][i].item.itemId == cloneData.itemId)
                            itemCount++;
                    }
                }
                else
                {
                    // 아이템 허용 개수 확인
                    for (int i = 0; i < inventoryCapacity[targetSlot]; i++)
                    {
                        if (_inventory[targetSlot][i].item != null && _inventory[targetSlot][i].item.itemId == cloneData.itemId)
                            itemCount++;
                    }
                }
                
                if (itemCount >= cloneData.itemSettings.maxBagAmount)
                {
                    // TODO : 해당 아이템 인벤토리 is Full UI
                    Debug.Log($"해당 아이템 {cloneData.itemTitle} 인벤토리 is Full UI");
                    return false;
                }
            }

            // 아이템 중복 가능
            // 아이템 중첩 개수 확인 및 아이템 Stack으로 저장
            if (cloneData.itemToggle.isStackable)
            {
                bool isStackFreeSpace = false;
                List<InventoryItem> inventoryItems;
                // 수납공간 대상이 가방 또는 주머니인 경우
                if (targetSlot is InventorySlotType.BackPack or InventorySlotType.Pouch)
                {
                    // 가방 먼저 확인
                    inventoryItems = GetInventoryItemsToSlotTypeAndItemId(InventorySlotType.BackPack, cloneData.itemId);
                    if (inventoryItems.Count >= 1)
                    {
                        foreach (var item in inventoryItems)
                        {
                            // 해당 슬롯에는 아이템 개수 최대치를 의미
                            if (cloneData.itemSettings.maxStackAmount == 0 || cloneData.itemSettings.maxStackAmount >= item.itemAmount + itemAmount)
                            {
                                isStackFreeSpace = true;
                                break;
                            }
                        }
                    }
                    // 가방에 중첩이 불가능한 경우 주머니 확인
                    if (!isStackFreeSpace)
                    {
                        // 주머니 확인
                        inventoryItems = GetInventoryItemsToSlotTypeAndItemId(InventorySlotType.Pouch, cloneData.itemId);
                        if (inventoryItems.Count >= 1)
                        {
                            foreach (var item in inventoryItems)
                            {
                                // 해당 슬롯에는 아이템 개수 최대치를 의미
                                if (cloneData.itemSettings.maxStackAmount == 0 || cloneData.itemSettings.maxStackAmount >= item.itemAmount + itemAmount)
                                {
                                    isStackFreeSpace = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                // 그 외 수납공간 대상인 경우
                else
                {
                    inventoryItems = GetInventoryItems(cloneData.itemId);
                    if (inventoryItems.Count >= 1)
                    {
                        foreach (var item in inventoryItems)
                        {
                            // 해당 슬롯에는 아이템 개수 최대치를 의미
                            if (cloneData.itemSettings.maxStackAmount == 0 || cloneData.itemSettings.maxStackAmount >= item.itemAmount + itemAmount)
                            {
                                isStackFreeSpace = true;
                                break;
                            }
                        }
                    }
                }
                
                // 아이템 중접이 가능한 곳이 있다는 의미
                if(isStackFreeSpace) return true;
            }
        
            // 특정 배낭 크기만 확인
            return InventorySlotCapacityCheck(targetSlot);
        }
        

        /// <summary>
        /// 특정 배낭 수납공간 여부 확인
        /// </summary>
        private bool InventorySlotCapacityCheck(InventorySlotType type)
        {
            int maxCapacity = -1;
            
            switch (type)
            {
                case InventorySlotType.Manual:
                    maxCapacity = inventorCapacitySettings.manualMaxCapacity;
                    break;
                case InventorySlotType.KeyHolder:
                    maxCapacity = inventorCapacitySettings.keyHolderMaxCapacity;
                    break;
                case InventorySlotType.Pouch:
                    maxCapacity = inventorCapacitySettings.pouchMaxCapacity;
                    break;
                case InventorySlotType.BackPack:
                    maxCapacity = inventorCapacitySettings.backPackMaxCapacity;
                    break;
            }

            return maxCapacity == -1 || _inventory[type].Count < maxCapacity;
        }

        /// <summary>
        /// 아이템 종류로 판단해 특정 배낭 최대수납공간 확인
        /// </summary>
        private int GetInventorySlotMaxCapacity(InventoryItemUseType type)
        {
            InventorySlotType slotType = TargetSlotType(type);
            int maxCapacity = -1;
            
            switch (slotType)
            {
                case InventorySlotType.Manual:
                    maxCapacity = inventorCapacitySettings.manualMaxCapacity;
                    break;
                case InventorySlotType.KeyHolder:
                    maxCapacity = inventorCapacitySettings.keyHolderMaxCapacity;
                    break;
                case InventorySlotType.Pouch:
                    maxCapacity = inventorCapacitySettings.pouchMaxCapacity;
                    break;
                case InventorySlotType.BackPack:
                    maxCapacity = inventorCapacitySettings.backPackMaxCapacity;
                    break;
            }
            return maxCapacity;
        }

        private int GetInventorySlotMaxCapacity(InventorySlotType slotType)
        {
            int maxCapacity = -1;

            switch (slotType)
            {
                case InventorySlotType.Manual:
                    maxCapacity = inventorCapacitySettings.manualMaxCapacity;
                    break;
                case InventorySlotType.KeyHolder:
                    maxCapacity = inventorCapacitySettings.keyHolderMaxCapacity;
                    break;
                case InventorySlotType.Pouch:
                    maxCapacity = inventorCapacitySettings.pouchMaxCapacity;
                    break;
                case InventorySlotType.BackPack:
                    maxCapacity = inventorCapacitySettings.backPackMaxCapacity;
                    break;
            }

            return maxCapacity;
        }
        
        /// <summary>
        /// 현재 인벤토리의 모든 수납공간 크기 확인
        /// </summary>
        private Dictionary<InventorySlotType, int> AllInventoryCapacityCheck()
        {
            Dictionary<InventorySlotType, int> inventoryCapacity = new Dictionary<InventorySlotType, int>()
            {
                { InventorySlotType.Manual, _inventory[InventorySlotType.Manual].Count },
                { InventorySlotType.BackPack, _inventory[InventorySlotType.BackPack].Count},
                { InventorySlotType.KeyHolder, _inventory[InventorySlotType.KeyHolder].Count},
                { InventorySlotType.Pouch, _inventory[InventorySlotType.Pouch].Count},
            };

            return inventoryCapacity;
        }

        /// <summary>
        /// 아이템 종류를 이용해 수납공간 선정
        /// </summary>
        private InventorySlotType TargetSlotType(InventoryItemUseType type)
        {
            InventorySlotType slotType = InventorySlotType.None;
            var allInventoryCapacity = AllInventoryCapacityCheck();

            switch (type)
            {
                /*
                // BackPack 수납공간 기준으로 확인 진행 Full 상태인 경우 Pouch로 설정
                case InventoryItemUseType.Bullet or InventoryItemUseType.Equipment or InventoryItemUseType.Food or InventoryItemUseType.ItemPart or InventoryItemUseType.Etc or InventoryItemUseType.Service or InventoryItemUseType.None:
                    slotType = allInventoryCapacity[InventorySlotType.BackPack] < inventorCapacitySettings.backPackMaxCapacity
                                ? InventorySlotType.BackPack : InventorySlotType.Pouch;
                    break;
                */
                
                case InventoryItemUseType.Manual :
                    slotType = InventorySlotType.Manual;
                    break;
                
                case InventoryItemUseType.Key :
                    slotType = InventorySlotType.KeyHolder;
                    break;
                
                default:
                    // BackPack 수납공간 기준으로 확인 진행 Full 상태인 경우 Pouch로 설정
                    slotType = allInventoryCapacity[InventorySlotType.BackPack] < inventorCapacitySettings.backPackMaxCapacity
                        ? InventorySlotType.BackPack : InventorySlotType.Pouch;
                    break;
            }
            return slotType;
        }
        
        #endregion
        
        #region 인벤토리 CRUD
        
        /// <summary>
        /// 상호작용으로 아이템 넣기 
        /// </summary>
        public bool PickUpAddItem(ItemComponent itemComponent)
        {
            ItemCloneData cloneData = GetItemCloneData(itemComponent.InventoryItemId);
            if (AddItem(cloneData, itemComponent.itemSettings.itemAmount))
            {
                // 배송된 아이템을 습득한 경우 퀘스트 트리거 실행
                if(itemComponent.IsDeliveryItem)
                {
                    TradersDeliveryTaskHandler?.Invoke(TaskHandler.TaskType.Get, TaskHandler.TaskID.Get_Delivery_Item.ToString(), string.Empty);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 인벤토리에 아이템 넣기
        /// </summary>
        public bool AddItem(ItemCloneData cloneData, int itemAmount, bool pickUpAlarm = true, bool skipAutoShortCutBind = false, int autoSlotId = -1, InventorySlotType slotType = InventorySlotType.None)
        {
            if (!InventoryItemMaxCapacityCheck(cloneData, itemAmount)) 
            {
                if (pickUpAlarm)
                {
                   NoManualHotelManager.Instance.UiNoManualUIManager.ShowInventoryGuideText(LocalizationTable.UITableTextKey.UI_Inventory_FullItemGuideText);
                }
                Debug.Log($"{cloneData.itemTitle} 아이템을 넣을 수 없음 ! [가방이 꽉 참]");
                return false;
            }
            
            skipAutoShortCutBind = skipAutoShortCutBind || cloneData.itemToggle.autoBindShortcut;

            // 평소 아이템 획득 시
            if (slotType == InventorySlotType.None)
            {
                // 아이템 중첩 개수 확인
                if (cloneData.itemToggle.isStackable)
                {
                    int inventoryItemListSlotId = GetRemainingInventoryItemSlotId(cloneData.itemId, itemAmount, out slotType, out int uiSlotId);
                    if (inventoryItemListSlotId != -1 && uiSlotId != -1)
                    {
                        if (cloneData.InventoryItemUseType == InventoryItemUseType.Equipment && cloneData.itemToggle.autoBindShortcut)
                        {
                            NoManualUtilsHelper.EditorDebugLog(NoManualUtilsHelper.LogTextColor.yellow, $"{cloneData.itemTitle} 장착(착용) 아이템이면서 동시에 isStackable 기능과 autoShorCut 기능을 이용하고 있습니다.");
                            return false;
                        }
                    
                        //  슬롯 id에 따라 아이템 데이터 업데이트
                        Debug.Log($" inventoryItemListSlotId :{inventoryItemListSlotId} / uiSlotId : {uiSlotId}");
                        _inventory[slotType][inventoryItemListSlotId].itemAmount += itemAmount;
                        _uiCipPanels[slotType].cipSlots[uiSlotId].UpdateCipPreviewSlotItemAmount(_inventory[slotType][inventoryItemListSlotId].itemAmount);
                        UpdateShortCutItemUI(cloneData.itemId, slotType, uiSlotId, _inventory[slotType][inventoryItemListSlotId].itemAmount);
                    }
                    else
                    {
                        CreateNewItemSlot(cloneData.itemId, itemAmount, slotType, !skipAutoShortCutBind, autoSlotId);
                    }
                }
                else
                {
                    slotType = TargetSlotType(cloneData.InventoryItemUseType);
                    CreateNewItemSlot(cloneData.itemId, itemAmount, slotType, !skipAutoShortCutBind, autoSlotId);
                }
            }
            // 인벤토리 아이템 초기 로드 시 기록된 위치에 아이템 배치
            else if (slotType != InventorySlotType.Length)
            {
                CreateNewItemSlot(cloneData.itemId, itemAmount, slotType, !skipAutoShortCutBind, autoSlotId);
                
                /*
                _inventory[slotType][autoSlotId].itemAmount += itemAmount;
                _uiCipPanels[slotType].cipSlots[autoSlotId].UpdateCipPreviewSlotItemAmount(_inventory[slotType][autoSlotId].itemAmount);
                UpdateShortCutItemUI(cloneData.itemId, slotType, autoSlotId, _inventory[slotType][autoSlotId].itemAmount);
                */
            }

            // 아이템이 추가될 때 이벤트 발생
            OnItemAdded?.Invoke(cloneData.itemId);
            
            // Get 퀘스트 이벤트
            GetInventoryItemTaskHandler?.Invoke(TaskHandler.TaskID.Get_Inventory_Item.ToString(), cloneData.itemId.ToString());
            
            // 아이템획득 UI 호출
            if (pickUpAlarm)
            {
                NoManualHotelManager.Instance.UiNoManualUIManager.ShowInventoryGuideText(cloneData.itemTitle, itemAmount);
            }
            
            return true;
        }
        
        /// <summary>
        /// 인벤토리에서 아이템 지우기
        /// </summary>
        public void RemoveItem(InventorySlotType slotType, int uiSlotId,  int removeItemAmount, InventoryItem removeTargetItem, bool rememberRestoreItem = false)
        {
            InventoryItem removeItem = GetSlotIdToInventoryItem(uiSlotId, slotType, out int inventoryListSlotId);

            if (removeItem != null && inventoryListSlotId != -1)
            {
                // 삭제 전 복구용도로 기억 (조합 하는 경우 대비)
                if (rememberRestoreItem)
                {
                    foreach (var removedItemInfo in _removedItemInfos)
                    {
                        if (removedItemInfo.SlotType == InventorySlotType.None)
                        {
                            removedItemInfo.SetRemovedItemInfo(slotType, inventoryListSlotId, uiSlotId, removeItemAmount, removeTargetItem);
                            break;
                        }
                    }
                }
                
                int totalItemAmount = _inventory[slotType][inventoryListSlotId].itemAmount - removeItemAmount;
                
                // 사용한 아이템 수량을 모두 소모했으면 아이템 리스트를 제거 
                if (totalItemAmount <= 0)
                {
                    _inventory[slotType].RemoveAt(inventoryListSlotId);
                    RemoveItemSlot(slotType, uiSlotId);
                    RemoveShortCutItemData(removeItem.item.itemId, slotType, uiSlotId);
                }
                // 아직 아이템 수량이 남아있다면 정보를 업데이트
                else
                {
                    _inventory[slotType][inventoryListSlotId].itemAmount = totalItemAmount;
                    _uiCipPanels[slotType].cipSlots[uiSlotId].UpdateCipPreviewSlotItemAmount(totalItemAmount);
                    UpdateShortCutItemUI(removeItem.item.itemId, slotType, uiSlotId, totalItemAmount);
                }
            }
        }

        /// <summary>
        /// 아이템 오브젝트를 바닥에 Drop
        /// </summary>
        public void DropItemGround(InventorySlotType slotType, int slotId,  int removeItemAmount, InventoryItem removeTargetItem, bool dropItemNoInteraction = false, Transform dropPos = null)
        {
            if (SelectSlotId == -1 && !IsSelecting) return;

            ItemCloneData cloneItemData = GetItemCloneData(removeTargetItem.item.itemId);
            Transform dropPosition = dropPos == null ? 
                                            HFPS.Player.PlayerController.Instance.GetComponentInChildren<HFPS.Player.PlayerFunctions>().inventoryDropPos
                                            : dropPos;

            GameObject dropItem;
            if (dropPos)
            { 
                dropItem = Instantiate(cloneItemData.dropPrefab, dropPosition);
            }
            else
            {
                dropItem = Instantiate(cloneItemData.dropPrefab, dropPosition.position, Quaternion.Euler(dropPosition.eulerAngles));
            }

            if (dropItem == null) // Drop 아이템 null 예외처리
            {
                ErrorCode.SendError(ErrorCode.ErrorCodeEnum.DropItem);
                return;
            }

            // Drop 아이템 대상이 장착 아이템인 경우 
            if (cloneItemData.InventoryItemUseType == InventoryItemUseType.Equipment)
            {
                // 현재 착용한 아이템 ID와 비교후 맞으면 장착 아이템 비활성화
                if (NoManualHotelManager.Instance.ItemShortCutManager.GetEquipItemId() == cloneItemData.itemSettings.equipItemId)
                {
                    NoManualHotelManager.Instance.ItemShortCutManager.DisableEquipItem();
                }
            }

            // dropItem 컴포넌트에 아이템 정보 할당
            if (dropItem.TryGetComponent(out ItemComponent dropItemComponent))
            {
                if (dropItemComponent.inventoryItem == null)
                {
                    ItemScriptable itemScriptable =  itemDataBaseScriptable.GetItemDataToItemId(cloneItemData.itemId);
                    if (itemScriptable == null)
                    {
                        ErrorCode.SendError(ErrorCode.ErrorCodeEnum.GetItem);
                        return;
                    }

                    dropItemComponent.inventoryItem = itemScriptable;
                }
                    
                dropItemComponent.InitializedItemComponent(cloneItemData.itemId, removeItemAmount);
                // Drop 아이템은 더 이상 상호작용이 불가능하게 설정
                if (dropItemNoInteraction)
                {
                    dropItemComponent.SetNoInteractive();
                    dropItemComponent.RemovePhysicsComponent();
                }
            }
            else
            {
                ErrorCode.SendError(ErrorCode.ErrorCodeEnum.NullComponent);
                return;
            }

            // 인벤토리 아이템 데이터 삭제 (Put_Relay 구별)
            if(dropPos == null) RemoveItem(slotType, slotId, removeItemAmount, removeTargetItem);
        }
        
        /// <summary>
        /// 새로운 아이템 데이터 생성 및 CIP PreviewSlot UI 생성
        /// </summary>
        private void CreateNewItemSlot(int itemId, int itemAmount, InventorySlotType type, bool autoShortCut = false, int slotId = -1)
        {
            bool createSlotSuccess = false;
            InventoryItem newItem = new InventoryItem(GetItemCloneData(itemId), itemAmount);
            
            int loopNum = 0; // 반복문 탈출용
            do
            {
                // CIP 슬롯 강제 할당 (불러오기 할 시 배낭 아이템 위치 때문에)
                if (slotId != -1)
                {
                       // 아이템 정보가 비워있으면 CIP PreviewSlot 생성
                    if (!_uiCipPanels[type].cipSlots[slotId].CheckCipPreviewSlot())
                    {
                        UI_ChildInventoryPanelPreviewSlot cipPreviewSlot = Instantiate(inventoryCipPrefabs.cipPreviewSlot).GetComponent<UI_ChildInventoryPanelPreviewSlot>();
                        _uiCipPanels[type].cipSlots[slotId].InitializeCipPreviewSlot(cipPreviewSlot);
                        // 인벤토리 아이템 데이터에 Slot Id 할당
                        newItem.slotId = slotId;
                        newItem.slotType = type;
                        _inventory[type].Add(newItem);
                        SynchronizationInventoryUI(newItem, itemAmount, type, slotId);
                        
                        // 자동으로 장착 아이템 퀵슬롯 등록
                        if (autoShortCut && newItem.item.itemToggle.UsageOp_BindShortcut)
                        {
                            NoManualHotelManager.Instance.ItemShortCutManager.AutoBindShortCut(itemId, type, slotId);
                            
                            // 정상적으로 퀵슬롯 등록됨
                            if (cipPreviewSlot.ShortCut != string.Empty)
                            {
                                // CIP PreView Slot에 퀵슬롯 이미지 등록
                                Sprite shortCutIcon = GetShortCutSprite(cipPreviewSlot.ShortCut);
                                cipPreviewSlot.SetCipPreviewShortCutIcon(shortCutIcon); 
                                UpdateShortCutItemUI(itemId, type, slotId, itemAmount);
                            }
                        }
                        break;
                    }
                }
                else
                {
                    // CIP 슬롯 순회 검색
                    for (slotId = 0; slotId < _uiCipPanels[type].cipSlots.Count; slotId++)
                    {
                        // 아이템 정보가 비워있으면 CIP PreviewSlot 생성
                        if (!_uiCipPanels[type].cipSlots[slotId].CheckCipPreviewSlot())
                        {
                            UI_ChildInventoryPanelPreviewSlot cipPreviewSlot = Instantiate(inventoryCipPrefabs.cipPreviewSlot).GetComponent<UI_ChildInventoryPanelPreviewSlot>();
                            _uiCipPanels[type].cipSlots[slotId].InitializeCipPreviewSlot(cipPreviewSlot);
                            // 인벤토리 아이템 데이터에 Slot Id 할당
                            newItem.slotId = slotId;
                            newItem.slotType = type;
                            _inventory[type].Add(newItem);
                            SynchronizationInventoryUI(newItem, itemAmount, type, slotId);
                            
                            // 자동으로 장착 아이템 퀵슬롯 등록
                            if (autoShortCut && newItem.item.itemToggle.UsageOp_BindShortcut)
                            {
                                NoManualHotelManager.Instance.ItemShortCutManager.AutoBindShortCut(itemId, type, slotId);
                                
                                // 정상적으로 퀵슬롯 등록됨
                                if (cipPreviewSlot.ShortCut != string.Empty)
                                {
                                    // CIP PreView Slot에 퀵슬롯 이미지 등록
                                    Sprite shortCutIcon = GetShortCutSprite(cipPreviewSlot.ShortCut);
                                    cipPreviewSlot.SetCipPreviewShortCutIcon(shortCutIcon); 
                                    UpdateShortCutItemUI(itemId, type, slotId, itemAmount);
                                    // TODO : 퀵슬롯에 등록되었다는 UI 호출
                                }
                            }
                            
                            createSlotSuccess = true;
                            break;
                        }
                    }
                
                    // Manual 및 KeyHolder 배낭 CIP Empty Slot 생성 
                     // 1. 매뉴얼 과 키홀더 CIP는 기본적으로 CIP Empty Slot을 동적생성하지 않음 (CIP Empty Slot 개수 파악으로 아이템 스포일러 추측 방지용)
                     // 2. 그래서 CIP를 아이템과 얻는 동시에 따로 CIP Empty Slot를 동적생성 함
                    if (type is InventorySlotType.Manual or InventorySlotType.KeyHolder && !createSlotSuccess)
                    {
                        // CIP 패널에 Empty Slot 등록
                        CreateHiddenCipEmptySlot(type);
                    } 
                }
                if (loopNum++ > 10)
                {
                    Debug.Log("Loop 탈출");
                    break;
                }
                
            } while (!createSlotSuccess);
        }

        /// <summary>
        /// Manual 및 KeyHolder 배낭 CIP Empty Slot 생성 
        /// </summary>
        private void CreateHiddenCipEmptySlot(InventorySlotType type)
        {
            UI_ChildInventoryPanelEmptySlot cipEmptySlot = Instantiate(inventoryCipPrefabs.cipEmptySlot).GetComponent<UI_ChildInventoryPanelEmptySlot>();
            _uiCipPanels[type].AddCipEmptySlot(cipEmptySlot, _uiCipPanels[type].cipSlots.Count);
            if(!_uiCipPanels[type].gameObject.activeSelf) _uiCipPanels[type].gameObject.SetActive(true);
        }

        /// <summary>
        /// CIP PreViewSlot 삭제
        /// </summary>
        private void RemoveItemSlot(InventorySlotType slotType, int slotId)
        {
            if (_uiCipPanels[slotType].cipSlots[slotId] != null)
            {
                _uiCipPanels[slotType].cipSlots[slotId].RemoveCipPreviewSlot();
            }
        }
        
        /// <summary>
        /// 아이템 id기준으로 아이템 복사본 데이터 가져오기
        /// </summary>
        public ItemCloneData GetItemCloneData(int itemId)
        {
            if (_itemCloneDataBase.ContainsKey(itemId))
            {
                return _itemCloneDataBase[itemId];
            }

            ErrorCode.SendError($"[{itemId} ItemID] ", ErrorCode.ErrorCodeEnum.GetItem);
            return default;
        }

        /// <summary>
        /// 아이템 id기준으로 인벤토리 아이템 얻기
        /// </summary>
        public InventoryItem GetInventoryItem(int itemId)
        {
            ItemCloneData itemCloneData = GetItemCloneData(itemId);
            
            var type = TargetSlotType(itemCloneData.InventoryItemUseType);

            for (int i = 0; i < _inventory[type].Count; i++)
            {
                if (_inventory[type][i].item.itemId == itemId)
                {
                    // TargetSlotType() 함수대신 변수 type으로 사용하면 되지 않을까?
                   return _inventory[type][itemId];
                }
            }
            return null;
        }
        
        /// <summary>
        /// 슬롯 id기준으로 인벤토리 아이템 얻기
        /// </summary>
        private InventoryItem GetSlotIdToInventoryItem(int uiSlotId, InventorySlotType inventorySlotType, out int inventoryListSlotId)
        {
            for (int i = 0; i < _inventory[inventorySlotType].Count; i++)
            {
                if (_inventory[inventorySlotType][i].slotId == uiSlotId && _inventory[inventorySlotType][i].item != null)
                {
                    inventoryListSlotId = i;
                    return _inventory[inventorySlotType][i];
                }
            }

            inventoryListSlotId = -1;
            ErrorCode.SendError(ErrorCode.ErrorCodeEnum.GetItem);
            return null;
        }
        
        /// <summary>
        /// 슬롯 id기준으로 인벤토리 아이템 얻기
        /// </summary>
        public InventoryItem GetSlotIdToInventoryItem(int uiSlotId, InventorySlotType inventorySlotType)
        {
            for (int i = 0; i < _inventory[inventorySlotType].Count; i++)
            {
                if (_inventory[inventorySlotType][i].slotId == uiSlotId && _inventory[inventorySlotType][i].item != null)
                {
                    return _inventory[inventorySlotType][i];
                }
            }
            
            ErrorCode.SendError(ErrorCode.ErrorCodeEnum.GetItem);
            return null;
        }
        
        /// <summary>
        /// 아이템 id기준으로 인벤토리에서 동일한 아이템들 얻기
        /// </summary>
        private List<InventoryItem> GetInventoryItems(int itemId)
        {
            ItemCloneData itemCloneData = GetItemCloneData(itemId);
            List<InventoryItem> getInventoryItems = new List<InventoryItem>();
            var type = TargetSlotType(itemCloneData.InventoryItemUseType);

            for (int i = 0; i < _inventory[type].Count; i++)
            {
                if (_inventory[type][i].item.itemId == itemId)
                {
                    getInventoryItems.Add(_inventory[type][i]);
                }
            }
            
            return getInventoryItems;
        }

        /// <summary>
        /// 특정 아이템 id를 가지고 있는지 확인 
        /// </summary>
        public int HasInventoryItem(int itemId)
        {
            int hasItemCount = 0;
            foreach (var itemList in _inventory.Values)
            {
                foreach (var item in itemList)
                {
                    if (item.item.itemId.Equals(itemId)) hasItemCount++;
                }
            }
            return hasItemCount;
        }
        
        /// <summary>
        /// 아이템 id기준과 SlotType으로 인벤토리에서 동일한 아이템들 얻기
        /// </summary>
        private List<InventoryItem> GetInventoryItemsToSlotTypeAndItemId(InventorySlotType type, int itemId)
        {
            List<InventoryItem> getInventoryItems = new List<InventoryItem>();
     
            for (int i = 0; i < _inventory[type].Count; i++)
            {
                if (_inventory[type][i].item.itemId == itemId)
                {
                    getInventoryItems.Add(_inventory[type][i]);
                }
            }
            return getInventoryItems;
        }
        
        /// <summary>
        /// 아이템 id기준으로 여유공간이 남아있는 슬롯 id 얻기
        /// </summary>
        private int GetRemainingInventoryItemSlotId(int targetItemId, int targetItemAmount, out InventorySlotType targetType, out int uiSlotId)
        {
            ItemCloneData compareItemData = GetItemCloneData(targetItemId);

            targetType = TargetSlotType(compareItemData.InventoryItemUseType);
            
            List<InventoryItem> inventoryItems;// = GetInventoryItems(compareItemData.itemId);

            if (targetType is InventorySlotType.BackPack or InventorySlotType.Pouch)
            {
                inventoryItems = GetInventoryItemsToSlotTypeAndItemId(InventorySlotType.BackPack, targetItemId);
                if (inventoryItems.Count >= 1)
                {
                    for (var i = 0; i < inventoryItems.Count; i++)
                    {
                        // 해당 슬롯에는 아이템 개수 최대치를 의미
                        if (compareItemData.itemSettings.maxStackAmount != 0 && compareItemData.itemSettings.maxStackAmount < inventoryItems[i].itemAmount + targetItemAmount)
                        {
                            continue;
                        }

                        targetType = InventorySlotType.BackPack;
                        return uiSlotId = inventoryItems[i].slotId;
                        //return i;
                    }
                }
                
                inventoryItems = GetInventoryItemsToSlotTypeAndItemId(InventorySlotType.Pouch, targetItemId);
                if (inventoryItems.Count >= 1)
                {
                    for (var i = 0; i < inventoryItems.Count; i++)
                    {
                        // 해당 슬롯에는 아이템 개수 최대치를 의미
                        if (compareItemData.itemSettings.maxStackAmount != 0 && compareItemData.itemSettings.maxStackAmount < inventoryItems[i].itemAmount + targetItemAmount)
                        {
                            continue;
                        }
                        
                        targetType = InventorySlotType.Pouch;
                        return uiSlotId = inventoryItems[i].slotId;
                        // return i;
                    }
                }
            }
            else
            {
                inventoryItems = GetInventoryItems(targetItemId);
                if (inventoryItems.Count >= 1)
                {
                    for (var i = 0; i < inventoryItems.Count; i++)
                    {
                        // 해당 슬롯에는 아이템 개수 최대치를 의미
                        if (compareItemData.itemSettings.maxStackAmount != 0 && compareItemData.itemSettings.maxStackAmount < inventoryItems[i].itemAmount + targetItemAmount)
                        {
                            continue;
                        }
                        uiSlotId = inventoryItems[i].slotId;
                        return i;
                    }
                }
            }
            return uiSlotId = -1;
        }

        /// <summary>
        /// CIP Empty Slot 얻기
        /// </summary>
        public UI_ChildInventoryPanelEmptySlot GetCipEmptySlot(InventorySlotType type, int slotId)
        {
            var cipEmptySlot = _uiCipPanels[type].cipSlots[slotId];

            if (cipEmptySlot == null)
            {
                ErrorCode.SendError(ErrorCode.ErrorCodeEnum.GetCIP);
                return null;
            }

            return cipEmptySlot;
        }
        
        /// <summary>
        /// 인벤토리 아이템 정보 -> 인벤토리 CIP UI 동기화
        /// </summary>
        private void SynchronizationInventoryUI(InventoryItem inventoryItem, int itemAmount, InventorySlotType type, int slotId)
        {
            _uiCipPanels[type].cipSlots[slotId].UpdateCipPreviewSlot(inventoryItem, itemAmount);
        }

        /// <summary>
        /// 퀵슬롯 UI 아이템 개수 변경
        /// </summary>
        private void UpdateShortCutItemUI(int itemId, InventorySlotType type, int slotId, int itemAmount)
        {
            var shortCutBindKey = NoManualHotelManager.Instance.ItemShortCutManager.GetBindKeyBaseInventoryItemData(itemId, type, slotId);
            if (!NoManualUtilsHelper.FindStringEmptyOrNull(shortCutBindKey))
            {
                var inventoryShortCut = _inventoryShortCutList[shortCutBindKey];
                if (inventoryShortCut)
                {
                    inventoryShortCut.UpdateShortCutItemAmount(itemAmount);
                    NoManualHotelManager.Instance.UiNoManualUIManager.UpdateShortCutItemAmount(shortCutBindKey, itemAmount);
                }
            }
        }

        /// <summary>
        /// 퀵슬롯 UI 데이터 할당
        /// </summary>
        public void SetShortCutItemUI(string bindKey, UI_ChildInventoryPanelEmptySlot cipEmptySlot)
        {
            var inventoryShortCut = _inventoryShortCutList[bindKey];
            if (inventoryShortCut)
            {
                inventoryShortCut.SetShortCutItem(cipEmptySlot);
                NoManualHotelManager.Instance.UiNoManualUIManager.SetShortCutItemUI(bindKey, cipEmptySlot);
            }
        }

        /// <summary>
        /// 퀵슬롯 UI 데이터 지우기
        /// </summary>
        public void RemoveShortCutItemData(int itemId, InventorySlotType type, int slotId)
        {
            var shortCutBindKey = NoManualHotelManager.Instance.ItemShortCutManager.GetBindKeyBaseInventoryItemData(itemId, type, slotId);
            if (!NoManualUtilsHelper.FindStringEmptyOrNull(shortCutBindKey))
            {
                var inventoryShortCut = _inventoryShortCutList[shortCutBindKey];
                if (inventoryShortCut && NoManualHotelManager.Instance.ItemShortCutManager.RemoveBindShortCut(shortCutBindKey))
                {
                    inventoryShortCut.RemoveShortCutItem();
                    NoManualHotelManager.Instance.UiNoManualUIManager.RemoveShortCutData(shortCutBindKey);
                }
            }
        }

        /// <summary>
        /// 인벤토리 정신력 UI 업데이트
        /// </summary>
        public void UpdateMentalityUI(float mentalityFillAmount, Sprite bgSprite = null)
        {
            inventoryHUDPanel.mentalityImage.fillAmount = mentalityFillAmount;

           if (mentalityFillAmount >= 1)
           {
               inventoryHUDPanel.mentalityBgImage.sprite = bgSprite;
               inventoryHUDPanel.mentalityImage.enabled = false;
           }
        }
        
        #endregion

        #region 인벤토리 관련 UI 호출

        /// <summary>
        /// 아이템 상세 설명 창 호출
        /// </summary>
        public void ShowItemDetail(bool isShow, string itemTitle = null, string itemDescription = null)
        {
            contentUiObjects.itemTitleText.text = itemTitle;
            contentUiObjects.itemDescriptionText.text = itemDescription;
            inventoryPanels.itemDescription.SetActive(isShow);
        }

        /// <summary>
        /// 아이템 선택창 및 상세 설명 창 닫기
        /// </summary>
        public void ResetInventory()
        {
            foreach (var cipPanel in _uiCipPanels)
            {
                cipPanel.Value.AllResetCipSlots();
            }

            ShowItemContextMenu(false);
            ShowItemDetail(false);
            IsSelecting = false;
            // 조합 모드 또는 Put 모드를 취소하고 combineGuidePanel을 비활성화
            if (IsCombineMode || IsPutMode || IsShortCutMode || contentUiObjects.combineGuidePanel.activeSelf)
            {
                if (contentUiObjects.combineGuidePanel.activeSelf) contentUiObjects.combineGuidePanel.SetActive(false);
               IsShortCutMode =  IsPutMode = IsCombineMode = false;
               _putCallBackMapper = null;
               ShortCutDataMapper = null;
            }
            SelectSlotId = -1;
            SelectInventorySlotTypeSlotType = InventorySlotType.None;
            //EventSystem.current.SetSelectedGameObject(null);
        }
        
        /// <summary>
        /// 마지막에 클릭 한 슬롯 Reset
        /// </summary>
        public void LastSelectedSlotToReset()
        {
            if (SelectSlotId > -1)
            {
                _uiCipPanels[SelectInventorySlotTypeSlotType].cipSlots[SelectSlotId].ResetCipSlotProperty();
                SelectSlotId = -1;
            }
        }

        /// <summary>
        /// 아이템 Context Menu 호출
        /// </summary>
        public void ShowItemContextMenu(bool isShow, InventoryItem selectItem = null, InventorySlotType slotType = InventorySlotType.None, int slotId = -1)
        {
            if (DisableContextMenu) return;
            
            if (isShow && selectItem != null && slotId > -1 && slotType != InventorySlotType.None)
            {
                ItemCloneData itemCloneData = GetItemCloneData(selectItem.item.itemId);

                Vector3[] corners = new Vector3[4];
                // 가장 끝에 있는 Slot 확인
                int cornerSlot = _uiCipPanels[slotType].cipSlots.Count - 1;
                
                // 사각형의 4개 모서리의 각 pixel 위치 가져옴 (배열 순서 : 왼쪽 아래부터 시계방향)
                // 0 : 왼쪽아래, 1 : 왼쪽 위, 2 : 오른쪽 위, 3 : 오른쪽 아래
                _uiCipPanels[slotType].cipSlots[slotId].GetComponent<RectTransform>().GetWorldCorners(corners);
                
                // 가장 끝에 있는 Slot인 경우 Context Menu는 좌측으로 뛰움
                if (cornerSlot != slotId)
                {
                    //이 코드는 컨텍스트 메뉴의 회전 축을 설정합니다. (0, 1)은 메뉴의 왼쪽 상단을 의미합니다.
                    inventoryPanels.itemContextMenu.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
                    //이 코드는 컨텍스트 메뉴의 위치를 선택된 슬롯의 오른쪽 상단 모서리[2]로 설정합니다.
                    inventoryPanels.itemContextMenu.transform.position = corners[2];
                }
                else
                {   
                    //이 코드는 컨텍스트 메뉴의 회전 축을 설정합니다. (1, 1)은 메뉴의 오른쪽 상단을 의미합니다.
                    inventoryPanels.itemContextMenu.GetComponent<RectTransform>().pivot = new Vector2(1, 1);
                    //이 코드는 컨텍스트 메뉴의 위치를 선택된 슬롯의 왼쪽 상단 모서리[1]로 설정합니다.
                    inventoryPanels.itemContextMenu.transform.position = corners[1];
                }
                

                // Context 버튼들 컨트롤
                inventoryItemContextMenuButtons.UsageOp_BindShortcutButton.gameObject.SetActive(itemCloneData.itemToggle.UsageOp_BindShortcut);
                inventoryItemContextMenuButtons.UsageOp_EquipButton.gameObject.SetActive(itemCloneData.itemToggle.UsageOp_Equip);
                inventoryItemContextMenuButtons.UsageOp_EatButton.gameObject.SetActive(itemCloneData.itemToggle.UsageOp_Eat);
                inventoryItemContextMenuButtons.UsageOp_ReadButton.gameObject.SetActive(itemCloneData.itemToggle.UsageOp_Read);
                inventoryItemContextMenuButtons.UsageOp_CombineButton.gameObject.SetActive(itemCloneData.itemToggle.UsageOp_Combine);
                inventoryItemContextMenuButtons.UsageOp_DropButton.gameObject.SetActive(itemCloneData.itemToggle.UsageOp_Drop);

                for (int i = 0; i < inventoryPanels.itemContextMenu.transform.childCount; i++)
                {
                    Transform child = inventoryPanels.itemContextMenu.transform.GetChild(i);
                    if (child.gameObject.activeSelf)
                    {
                        _uiUsageOpContextButtons.Add(child.GetComponent<UI_UsageOpContextButton>());
                    }
                }

                IsContextMenuVisible = true;
            }
            else
            {
                inventoryItemContextMenuButtons.UsageOp_BindShortcutButton.gameObject.SetActive(false);
                inventoryItemContextMenuButtons.UsageOp_EquipButton.gameObject.SetActive(false);
                inventoryItemContextMenuButtons.UsageOp_EatButton.gameObject.SetActive(false);
                inventoryItemContextMenuButtons.UsageOp_ReadButton.gameObject.SetActive(false);
                inventoryItemContextMenuButtons.UsageOp_CombineButton.gameObject.SetActive(false);
                inventoryItemContextMenuButtons.UsageOp_DropButton.gameObject.SetActive(false);
                
                foreach (var contextButton in _uiUsageOpContextButtons)
                {
                    contextButton.Deselect();
                }
                
                _uiUsageOpContextButtons.Clear();
                IsContextMenuVisible = false;
            }
            
            inventoryPanels.itemContextMenu.SetActive(isShow);
        }
        
        /// <summary>
        /// CIP PreViewSlot 퀵슬롯 이미지 얻기
        /// </summary>
        public Sprite GetShortCutSprite(string shortCutControl)
        {
            Sprite shortCugImage = null;
            switch (shortCutControl)
            {
                case "UseItem1":
                    shortCugImage = shortCutImages.shortCut1;
                    break;

                case "UseItem2":
                    shortCugImage = shortCutImages.shortCut2;
                    break;

                case "UseItem3":
                    shortCugImage = shortCutImages.shortCut3;
                    break;

                case "UseItem4":
                    shortCugImage = shortCutImages.shortCut4;
                    break;
            }

            return shortCugImage;
        }

        /// <summary>
        /// 아이템 조합 가이드 설명 창 호출
        /// </summary>
        private void ShowInventoryGuideText(LocalizationTable.UITableTextKey textKey)
        {
            contentUiObjects.combineGuideText.text = NoManualHotelManager.Instance.UiNoManualUIManager.GetLocalizationText(LocalizationTable.TextTable.UI_Table, textKey);
            contentUiObjects.combineGuidePanel.SetActive(true);
        }

        /// <summary>
        /// 아이템 조합 가이드 설명 창 닫기
        /// </summary>
        private void HideCombineGuideText()
        {
            contentUiObjects.combineGuidePanel.SetActive(false);
        }
        
        
        #endregion

        #region 아이템 조합 (Combine)

        /// <summary>
        /// 삭제한 아이템 복구
        /// </summary>
        private void RestoreItem()
        {
            /* 삭제한 아이템은 복구는 예외처리를 따로 하지 않고 해당 SlotType과 SlotId에 InventoryItem정보와 ItemAmount를 일방적으로 넣는다. */
            /* 1. 조합 전 조합 재료에 사용되는 아이템 데이터를 삭제 후 기억
             * 2. 배낭공간 확인 후 조합 결과물이 들어갈 공간이 있으면 사용 기억한 데이터를 제거 X
             * 3. 배낭공간 확인 후 조합 결과물이 들어갈 공간이 없으면 기억한 데이터를 가반으로 RestoreItem() 함수 이용 후 아이템 원상복구
             */
            
            foreach (var removedItemInfo in _removedItemInfos)
            {
                Debug.Log(removedItemInfo.SlotType.ToString());
                if(removedItemInfo.SlotType == InventorySlotType.None) continue;

                // 아이템 중첩 개수 확인
                if (removedItemInfo.RemovedItem.item.itemToggle.isStackable)
                {
                    if (removedItemInfo.InventoryItemListId != -1 && removedItemInfo.UiSlotId != -1)
                    {
                        //  슬롯 id에 따라 아이템 데이터 업데이트
                       // Debug.Log($" inventoryItemListSlotId :{removedItemInfo.InventoryItemListId} / uiSlotId : {removedItemInfo.UiSlotId}");
                        _inventory[removedItemInfo.SlotType][removedItemInfo.InventoryItemListId].itemAmount += removedItemInfo.RemoveItemAmount;
                        _uiCipPanels[removedItemInfo.SlotType].cipSlots[removedItemInfo.UiSlotId].UpdateCipPreviewSlotItemAmount( _inventory[removedItemInfo.SlotType][removedItemInfo.InventoryItemListId].itemAmount);
                    }
                    else
                    {
                        CreateItemSlot(removedItemInfo);
                    }
                }
                else
                {
                    CreateItemSlot(removedItemInfo);
                }
            }

            void CreateItemSlot(RemovedItemInfo removedItemInfo)
            {
                // 아이템 정보가 비워있으면 CIP PreviewSlot 생성
                if (!_uiCipPanels[removedItemInfo.SlotType].cipSlots[removedItemInfo.UiSlotId].CheckCipPreviewSlot())
                {
                    UI_ChildInventoryPanelPreviewSlot cipPreviewSlot = Instantiate(inventoryCipPrefabs.cipPreviewSlot).GetComponent<UI_ChildInventoryPanelPreviewSlot>();
                    _uiCipPanels[removedItemInfo.SlotType].cipSlots[removedItemInfo.UiSlotId].InitializeCipPreviewSlot(cipPreviewSlot);
                    // 인벤토리 아이템 데이터에 Slot Id 할당
                    _inventory[removedItemInfo.SlotType].Add(removedItemInfo.RemovedItem);

                    SynchronizationInventoryUI(removedItemInfo.RemovedItem, removedItemInfo.RemoveItemAmount, removedItemInfo.SlotType, removedItemInfo.UiSlotId);
                    
                    // 자동으로 장착 아이템 퀵슬롯 등록
                    if (removedItemInfo.RemovedItem.item.itemToggle.autoBindShortcut && removedItemInfo.RemovedItem.item.itemToggle.UsageOp_BindShortcut)
                    {
                        NoManualHotelManager.Instance.ItemShortCutManager.AutoBindShortCut(removedItemInfo.RemovedItem.item.itemId, removedItemInfo.SlotType, removedItemInfo.UiSlotId);
                        
                        // 정상적으로 퀵슬롯 등록됨
                        if (cipPreviewSlot.ShortCut != string.Empty)
                        {
                            // CIP PreView Slot에 퀵슬롯 이미지 등록
                            Sprite shortCutIcon = GetShortCutSprite(cipPreviewSlot.ShortCut);
                            cipPreviewSlot.SetCipPreviewShortCutIcon(shortCutIcon);
                        }
                    }
                    
                    /*
                    foreach (var tItem in _inventory[removedItemInfo.SlotType])
                    {
                        Debug.Log("Slot Id : " + tItem.slotId + ", Item Name : " + tItem.item.itemTitle);
                    }
                    */
                }
            }
        }
        
        /// <summary>
        /// 아이템 조합 데이터 매핑
        /// </summary>
        private bool SetCombineItemData(CombineItem combineItemMapper)
        {
            if (SelectSlotId == -1 && !IsSelecting && SelectInventorySlotTypeSlotType == InventorySlotType.None)
            {
                ErrorCode.SendError(ErrorCode.ErrorCodeEnum.CombineData);
                return false;
            }
            
            InventoryItem inventoryItem = GetSlotIdToInventoryItem(SelectSlotId, SelectInventorySlotTypeSlotType);
            combineItemMapper.SetCombineItemData(inventoryItem, SelectInventorySlotTypeSlotType, SelectSlotId);
            
            return true;
        }
        
        /// <summary>
        /// 아이템 조합
        /// </summary>
        public void CombineItem()
        {
            bool isCombineFail = false;
            
            if (_firstCombineItem == null || !SetCombineItemData(_secondCombineItem))
            {
                ErrorCode.SendError(ErrorCode.ErrorCodeEnum.CombineData);
                return;
            }

            ItemScriptable.CombineSettings firstCombineSetting = null;
            ItemScriptable.CombineSettings secondCombineSetting = null;
            
            foreach (var firstSetting in _firstCombineItem.InventoryItem.item.combineSettings)
            {
                if (firstSetting.combineWithID != _secondCombineItem.InventoryItem.item.itemId)
                    continue;

                var secondSetting = _secondCombineItem.InventoryItem.item.combineSettings.FirstOrDefault(setting => setting.combineWithID == _firstCombineItem.InventoryItem.item.itemId);

                firstCombineSetting = firstSetting;
                secondCombineSetting = secondSetting;
                break;
            }

            if (firstCombineSetting == null || secondCombineSetting == null)
            {
                isCombineFail = true;
            }
            else
            {
                if (firstCombineSetting.combineAddItem)
                {
                    // 조합 아이템 제거 작업
                    HandleCombineSettings(firstCombineSetting, secondCombineSetting);
                    ItemCloneData combineResult = GetItemCloneData(firstCombineSetting.resultCombineID);
                    if (AddItem(combineResult, firstCombineSetting.resultCombineItemAmount))
                    {
                        // 조합 결과 아이템 데이터 동적 생성  
                        // 1. 결과 아이템 위치는 _secondCombineItem 위치로
                        //  1-1. 조합을 했는데도 _secondCombineItem의 개수가 남아있는 경우는 0위치 에서 N위치에 비워있는 공간에 아무곳이나 넣기
                        isCombineFail = false;
                    }
                    else
                    {
                        // 조합 결과물이 들어갈 공간이 없어서 제작에 사용된 아이템 원상복구
                        RestoreItem();
                        isCombineFail = true;
                    }
                    // 복구용 조합 아이템 제거
                    ResetRestoreItemData();
                }
            }
                

            IsCombineMode = false;

            // 최종적으로 조합 결과 UI 출력 확인
            if (isCombineFail)
            {
                ShowInventoryGuideText(LocalizationTable.UITableTextKey.UI_Inventory_CombineFailText);
            }
            else
            {
                ShowInventoryGuideText(LocalizationTable.UITableTextKey.UI_Inventory_CombineSuccessfulText);
            }
        }
        
        /// <summary>
        /// 조합 아이템 삭제 전 복구용 데이터 생성
        /// </summary>
        private void HandleCombineSettings(ItemScriptable.CombineSettings firstSetting, ItemScriptable.CombineSettings secondSetting)
        {
            if (!firstSetting.combineKeepItem)
            {
                // 첫 번째 아이템 복구용 데이터 생성
                RemoveItem(_firstCombineItem.InventorySlotType, _firstCombineItem.SlotId, 1, _firstCombineItem.InventoryItem, true);
            }

            if (!secondSetting.combineKeepItem)
            {
                // 두 번째 아이템 복구용 데이터 생성
                RemoveItem(_secondCombineItem.InventorySlotType, _secondCombineItem.SlotId, 1, _secondCombineItem.InventoryItem, true);
            }
        }

        /// <summary>
        /// 복구용 아이템 데이터 초기화
        /// </summary>
        private void ResetRestoreItemData()
        {
            // 복구용 아이템 데이터 초기화
            foreach (var removedItemInfo in _removedItemInfos)
            {
                removedItemInfo.ResetRemovedItemInfo();
            }
        }

        #endregion

        #region 아이템 전달 (Put)

        /// <summary>
        /// Put Task를 위한 인벤토리 창 호출
        /// </summary>
        public void ShowPutInventory(PutCallBackMapper putCallBack)
        {
     //       if (putCallBack.RequestItemIdList.Length <= 0) return; (09.29) 
            NoManualHotelManager.Instance.UiNoManualUIManager.InventoryUiHandler();
            
            _putCallBackMapper = putCallBack;
            IsPutMode = true;
            ShowInventoryGuideText(LocalizationTable.UITableTextKey.UI_Inventory_PutGuideText);
            PutInventoryItemCheck(_putCallBackMapper.RequestItemIdList);
        }
        
        private void PutInventoryItemCheck(int[] requestPutInventoryItemIdList)
        {
            // 모든 인벤토리 모든 슬롯 선택 막기
            foreach (var inventorySlotPanel in _uiCipPanels.Values)
            {
               inventorySlotPanel.AllCipSlotLock();
            }

            if (requestPutInventoryItemIdList == null || requestPutInventoryItemIdList.Length == 0) return; // (09.29)
            
            // Put이 가능한 아이템 슬롯 선택 풀기
            foreach (var itemId in requestPutInventoryItemIdList)
            {
                foreach (var inventorySlotPanel in _uiCipPanels.Values)
                {
                    inventorySlotPanel.TargetClipSlotReset(itemId);
                }
            }
        }
        
        /// <summary>
        /// Put 할당
        /// </summary>
        public void PutInventoryItem()
        {
            if (!IsPutMode || _putCallBackMapper == null) return;
            
            InventoryItem inventoryItem = GetSlotIdToInventoryItem(SelectSlotId, SelectInventorySlotTypeSlotType);
            if(inventoryItem == null) return;
            
            // Put 퀘스트 클리어
            PutInventoryItemTaskHandler?.Invoke(_putCallBackMapper.TaskId, _putCallBackMapper.TaskTargetId, inventoryItem.item.itemId.ToString());
            // Put 행동 트리거
            _putCallBackMapper.PutCallBackTrigger();
              // 인벤토리 변경 여부 
            if (_putCallBackMapper.PutRemoveItem)
            {
                RemoveItem(SelectInventorySlotTypeSlotType, SelectSlotId, 1, inventoryItem);
            }
            NoManualHotelManager.Instance.UiNoManualUIManager.CloseInventoryUI();
            ResetInventory();
        }

        #endregion

        #region 아이템 사용 효과

        /// <summary>
        /// 아이템 착용
        /// </summary>
        private void EquipItem()
        {
            var equipItem = GetSlotIdToInventoryItem(SelectSlotId, SelectInventorySlotTypeSlotType, out _);
            EquipItem(equipItem);
        }
        
        private void EquipItem(InventoryItem equipItem)
        {
            if (equipItem != null && equipItem.item.InventoryItemUseType is InventoryItemUseType.Equipment)
            {
                NoManualHotelManager.Instance.ItemShortCutManager.EquipItemSwitch(equipItem.item.itemSettings.equipItemId);
            }
        }

        /// <summary>
        /// 아이텝 섭취
        /// </summary>
        private void EatItem()
        {
            var eatItem = GetSlotIdToInventoryItem(SelectSlotId, SelectInventorySlotTypeSlotType, out _);
            EatItem(eatItem, SelectInventorySlotTypeSlotType, SelectSlotId);
        }

        private void EatItem(InventoryItem eatItem, InventorySlotType type, int uiSlotId, int removeAmount = 1)
        {
            if (eatItem != null && eatItem.item.InventoryItemUseType is InventoryItemUseType.Food)
            {
                if(eatItem.item.itemSettings.mentalityAmount > 0) HFPS.Player.PlayerController.Instance.IncreaseMentality(eatItem.item.itemSettings.mentalityAmount);
                if(eatItem.item.itemSettings.staminaAmount > 0)  HFPS.Player.PlayerController.Instance.IncreaseStamina(eatItem.item.itemSettings.staminaAmount);
                
                NoManualHotelManager.Instance.AudioManager.PlaySFX(AudioManager.SFX_Audio_List.Player, true, eatItem.item.itemSounds.useSound);
                RemoveItem(type, uiSlotId, removeAmount, eatItem);
            }
        }

        /// <summary>
        /// 책 읽기
        /// </summary>
        private void ReadItem()
        {
            if (SelectSlotId == -1 && !IsSelecting && SelectInventorySlotTypeSlotType == InventorySlotType.None) return;
            InventoryItem readItemData = _uiCipPanels[SelectInventorySlotTypeSlotType].cipSlots[SelectSlotId].GetCipPanelPreviewSlotItem();
            ReadItem(readItemData);
        }

        private void ReadItem(InventoryItem readItem)
        {
            // 아이템 정보가 읽기가 불가능한 경우 생략
            if (readItem != null || readItem.item.InventoryItemUseType is InventoryItemUseType.Manual)
            {
                // 책 읽기 ID 이용
                NoManualHotelManager.Instance.UiNoManualUIManager.CreatPaperUI(readItem.item.itemSettings.readItemId);
            }
        }

        private void BindShortCutItem()
        {
            if (SelectSlotId == -1 && !IsSelecting && SelectInventorySlotTypeSlotType == InventorySlotType.None) return;
            InventoryItem shortcutItem = _uiCipPanels[SelectInventorySlotTypeSlotType].cipSlots[SelectSlotId].GetCipPanelPreviewSlotItem();
            ShortCutDataMapper = new ShortCutData(shortcutItem.item.itemId, SelectInventorySlotTypeSlotType, SelectSlotId, string.Empty);
            
            // 인벤토리 가이드 텍스트 출력
            ShowInventoryGuideText(LocalizationTable.UITableTextKey.UI_Inventory_ShortCutGuideText);
            
            ShortCutInventoryItemCheck(ShortCutDataMapper);
            IsShortCutMode = true;
            ShowItemContextMenu(false);
        }
        
        private void ShortCutInventoryItemCheck(ShortCutData shortCutData)
        {
            // 모든 인벤토리 모든 슬롯 선택 막기
            foreach (var inventorySlotPanel in _uiCipPanels.Values)
            {
                inventorySlotPanel.AllCipSlotLock();
            }
            
            // ShortCut 아이템 슬롯 선택 풀기 (1015 : 퀵슬롯 등록 시 클릭 방지처리)
            /*
            foreach (var inventorySlotPanel in _uiCipPanels)
            {
                if (inventorySlotPanel.Key.Equals(shortCutData.ItemType))
                {
                    foreach (var cipEmptySlot in inventorySlotPanel.Value.cipSlots)
                    {
                        if (cipEmptySlot.SlotId.Equals(shortCutData.SlotId))
                        {
                            // 해당 슬롯 제한 해체
                            cipEmptySlot.ResetCipSlotProperty();
                            break;
                        }
                    }
                }
            }
            */
        }


        #endregion

        #region 퀵슬롯 아이템 호출

        /// <summary>
        /// 퀵슬롯 아이템 호출
        /// </summary>
        public void UseQuickSlotItem(InventorySlotType slotType, int slotId, string shortCutKey)
        {
            InventoryItem usableItem = null;

            usableItem = GetSlotIdToInventoryItem(slotId, slotType, out _);

            if (usableItem == null) return;
            
            if (usableItem.item.InventoryItemUseType is InventoryItemUseType.Equipment)
            {
                EquipItem(usableItem);
            }
            else if (usableItem.item.InventoryItemUseType is InventoryItemUseType.Food)
            {
                EatItem(usableItem, slotType, slotId);
            }
            else if (usableItem.item.InventoryItemUseType is InventoryItemUseType.Manual)
            {
                ReadItem(usableItem);
            }
            NoManualHotelManager.Instance.UiNoManualUIManager.SetHighLightShortCut(shortCutKey);
        }

        #endregion

        #region Context Button 이벤트
        

        /// <summary>
        /// Context Drop 버튼 이벤트
        /// </summary>
        public void DropItemButtonEvent()
        {
            if (SelectSlotId == -1 && !IsSelecting && SelectInventorySlotTypeSlotType == InventorySlotType.None) return;
            
            _uiCipPanels[SelectInventorySlotTypeSlotType].cipSlots[SelectSlotId].DropSlotItem();
            ResetInventory();
        }

        /// <summary>
        ///  Context Combine 버튼 이벤트
        /// </summary>
        public void CombineItemButtonEvent()
        {
            // 첫 번쨰 조합 아이템 정보 얻기
            if (!SetCombineItemData(_firstCombineItem)) return;
            
            // 조합 가이드 텍스트 출력
            ShowInventoryGuideText(LocalizationTable.UITableTextKey.UI_Inventory_CombineGuideText);

            // 현재 선택한 슬롯은 선택 불가능으로 설정
           _uiCipPanels[SelectInventorySlotTypeSlotType].cipSlots[SelectSlotId].CipEmptySlotLock();
           
           // 조합 모드로 변경
           IsCombineMode = true;

           // Context Button 모두 비활성화
           ShowItemContextMenu(false);
        }

        /// <summary>
        /// Context Read 버튼 이벤트
        /// </summary>
        public void ReadItemButtonEvent()
        {
            ReadItem();
            ShowItemContextMenu(false);
        }

        /// <summary>
        /// Context Equip 버튼 이벤트
        /// </summary>
        public void EquipItemButtonEvent()
        {
            EquipItem();
            ShowItemContextMenu(false);
        }

        /// <summary>
        /// Context Eat 버튼 이벤트
        /// </summary>
        public void EatItemButtonEvent()
        {
            EatItem();
            ShowItemContextMenu(false);
        }

        /// <summary>
        /// Context ShorCut 버튼 이벤트
        /// </summary>
        public void ShorCutButtonEvent()
        {
            // 퀵슬롯 등록 모드로 전환
            BindShortCutItem();
            ShowItemContextMenu(false);
        }

        #endregion

        #region 아이템 획득 이벤트 핸들러
        

        // 조건에 따라 동작을 실행하는 메소드
        private void CheckAndExecuteActions(int itemId)
        {
            if (_itemCheckActions.TryGetValue(itemId, out Action action))
            {
                action.Invoke();
                // 이벤트 트리거를 실행하고 바로 해당 이벤트 구독 해체
                UnregisterItemCheckAction(itemId);
            }
        }

        // 조건과 동작을 등록하는 메소드
        public void RegisterItemCheckAction(int findItemId, Action action)
        {
            _itemCheckActions.Add(findItemId, action);
        }

        // 조건과 동작을 해제하는 메소드
        public void UnregisterItemCheckAction(int findItemId)
        {
            _itemCheckActions.Remove(findItemId);
        }

        #endregion
    }

    /// <summary>
    /// 인벤토리 복구 아이템
    /// </summary>
    public class RemovedItemInfo
    {
        public InventorySlotType SlotType { get; private set; }
        public int InventoryItemListId { get; private set; }
        public int UiSlotId { get; private set; }
        public int RemoveItemAmount { get; private set; }
        public InventoryItem RemovedItem { get; private set; }

        public RemovedItemInfo(InventorySlotType slotType, int inventoryItemListId, int uiSlotId, int removeItemAmount, InventoryItem removedItem)
        {
            SetRemovedItemInfo(slotType, inventoryItemListId, uiSlotId, removeItemAmount, removedItem);
        }

        /// <summary>
        /// 복구 아이템 데이터 설정
        /// </summary>
        public void SetRemovedItemInfo(InventorySlotType slotType, int inventoryItemListId, int uiSlotId, int removeItemAmount, InventoryItem removedItem)
        {
            this.SlotType = slotType;
            this.InventoryItemListId = inventoryItemListId;
            this.UiSlotId = uiSlotId;
            this.RemoveItemAmount = removeItemAmount;
            this.RemovedItem = removedItem;
        }

        /// <summary>
        /// 복구 아이템 초기화
        /// </summary>
        public void ResetRemovedItemInfo()
        {
            this.SlotType = InventorySlotType.None;
            this.InventoryItemListId = -1;
            this.UiSlotId = -1;
            this.RemoveItemAmount = 0;
            this.RemovedItem = null;
        }
    }
    
    /// <summary>
    /// 조합 아이템 데이터 매핑 
    /// </summary>
    public class CombineItem
    {
        // 조합 아이템 데이터
        public InventoryItem InventoryItem { get; private set; } = null;
        // 조합 아이템 인벤토리 위치
        public InventorySlotType InventorySlotType { get; private set; } = InventorySlotType.None;
        // 조합 아이템 인벤토리 슬롯 ID
        public int SlotId { get; private set; } = -1;

        /// <summary>
        /// 조합 아이템 데이터 기록
        /// </summary>
        public void SetCombineItemData(InventoryItem item, InventorySlotType slotType, int slotId)
        {
            this.InventoryItem = item;
            this.InventorySlotType = slotType;
            this.SlotId = slotId;
        }

        /// <summary>
        /// 조합 아이템 데이터 초기화
        /// </summary>
        public void ResetCombineItemData()
        {
            this.InventoryItem = null;
            this.InventorySlotType = InventorySlotType.None;
            this.SlotId = -1;
        }
    }

    
    /// <summary>
    /// Put 콜백 데이터 매핑
    /// </summary>
    public class PutCallBackMapper
    {
        public string TaskId { get; private set; }
        public string TaskTargetId { get; private set; }
        public readonly int[] RequestItemIdList;
        public bool PutRemoveItem { get; private set; }
        private event Action _putCallBackAction;
        private IPut _putInterface;
        
        public PutCallBackMapper(string taskId, string taskTargetId, int[] requestItemIdList, Action putCallBackAction, IPut putInterface)
        {
            this.TaskId = taskId;
            this.TaskTargetId = taskTargetId;
            this.RequestItemIdList = requestItemIdList;
            this._putCallBackAction -= putCallBackAction;
            this._putCallBackAction += putCallBackAction;
            this._putInterface = putInterface;
            PutRemoveItem = this._putInterface.RemoveInventoryItem();
        }

        /// <summary>
        /// Put 콜백 트리거
        /// </summary>
        public void PutCallBackTrigger()
        {
            // Put 모드인경우에만 트리거 진행
            if (_putInterface.GetMode is IPut.PutMode.Put)
            {
                _putCallBackAction?.Invoke();
                _putInterface.SwapPutMode();
            }
        }
    }

    [System.Serializable]
    public class InventorySaveData
    {
        public int ItemId;
        public int ItemAmount;
        public int slotId = -1;
        public InventorySlotType slotType = InventorySlotType.None;

        public InventorySaveData(int itemId, int itemAmount, int slotId, InventorySlotType slotType)
        {
            this.ItemId = itemId;
            this.ItemAmount = itemAmount;
            this.slotId = slotId;
            this.slotType = slotType;
        }
    }
    
    /// <summary>
    /// 인벤토리 아이템 데이터
    /// </summary>
    [System.Serializable]
    public class InventoryItem
    {
        public ItemCloneData item;
        public int itemAmount;
        public int slotId = -1;
        public InventorySlotType slotType = InventorySlotType.None;

        public InventoryItem(ItemCloneData item, int itemAmount, int slotId = -1, InventorySlotType slotType = InventorySlotType.None)
        {
            this.item = item;
            this.itemAmount = itemAmount;
            this.slotId = slotId;
            this.slotType = slotType;
        }
    }

    /// <summary>
    /// 인벤토라 아이템 데이터 복사본
    /// </summary>
    public sealed class ItemCloneData
    {
        public int itemId = -1;
        public string itemTitle = "New Item";
        public Sprite itemIcon = null;
        public string description = "New Item Description";
        public InventoryItemUseType InventoryItemUseType = InventoryItemUseType.None;
        public GameObject dropPrefab = null;
        
        public ItemScriptable.Toggle itemToggle;
        public ItemScriptable.Settings itemSettings;
        public ItemScriptable.Sounds itemSounds;
        public ItemScriptable.CombineSettings[] combineSettings;
        
        public ItemCloneData(ItemScriptable itemScriptable)
        {
            itemId = itemScriptable.itemId;
            itemTitle = itemScriptable.itemTitle;
            itemIcon = itemScriptable.itemIcon;
            description = itemScriptable.description;
            InventoryItemUseType = itemScriptable.inventoryItemUseType;
            dropPrefab = itemScriptable.dropPrefab;

            itemToggle = itemScriptable.itemToggle;
            itemSettings = itemScriptable.itemSettings;
            itemSounds = itemScriptable.itemSounds;
            combineSettings = itemScriptable.combineSettings;
        }
    }
}


