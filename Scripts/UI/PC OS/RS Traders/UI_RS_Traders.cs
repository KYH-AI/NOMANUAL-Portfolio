using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NoManual.Inventory;
using NoManual.Managers;
using NoManual.Task;
using UnityEngine;
using UnityEngine.UI;
using NoManual.Utils;
using TMPro;

namespace NoManual.UI
{
    /*
     * 1. R.S Traders 와 R.S BucketList 

         1. R.S Traders 동적 아이템 생성가능 (실시간 상품 업데이트 가능)
         2. R.S Traders 장비, 소모품, 서비스, 기타 패널 이동 가능
         3. R.S Traders 패널 스크롤 크기에 따라 스크롤뷰 대응 가능
         4. 중복된 상품 등록시 예외 처리 완료
     */
    
    public class UI_RS_Traders : MonoBehaviour
    {
        [Serializable]
        public struct RS_Traders
        {
          [Header("장비 트레이더스 버튼")] public Button equipTradersStoreButton;
          [Header("소모품 트레이더스 버튼")] public Button consumableTradersStoreButton;
          [Header("서비스 트레이더스 버튼")] public Button serviceTradersStoreButton;
          [Header("기타 트레이더스 버튼")] public Button etcTradersStoreButton;
          [Space(20)] 
          [Header("트레이더스 아이템 프리팹")] public GameObject tradersItemPrefab;
          [Header("스크롤뷰 트레이더스 패널")] public Transform tradersItemPanel;
          [Header("장비 오브젝트 풀")] public Transform equipItemPool;
          [Header("소모품 오브젝트 풀")] public Transform consumableItemPool;
          [Header("서비스 오브젝트 풀")] public Transform serviceItemPool;
          [Header("기타 오브젝트 풀")] public Transform etcItemPool;
        }
        
        [Serializable]
        public struct RS_BucketList
        {
            [Header("장바구니 구매 버튼")] public Button bucketListAllBuyButton;
            [Space(20)]
            [Header("장바구니 아이템 프리팹")] public GameObject bucketListItemPrefab;
            [Header("장바구니 패널")] public Transform bucketListPanel;
            [Header("총 상품 가격 1")] public TextMeshProUGUI bucketListTotalPriceText1;
            [Header("총 상품 가격 2")] public TextMeshProUGUI bucketListTotalPriceText2;
            [Header("총 보유 RS 머니 1")] public TextMeshProUGUI userTotal_RS_CashText1;
            [Header("총 보유 RS 머니 2")] public TextMeshProUGUI userTotal_RS_CashText2;
            [Header("총 상품 개수")] public TextMeshProUGUI bucketListTotalItemStackText;
            [Header("구매 후 RS 머니")] public TextMeshProUGUI calculation_RS_CashText;
        }

        [Header("RS 트레이더스")]
        public RS_Traders rsTraders = new RS_Traders();
        [Space(20)]
        [Header("RS 장바구니")]
        public RS_BucketList rsBucketList = new RS_BucketList();
        [Space(10)] 
        [Header("RS 팝업창")]
        [SerializeField] private UI_RS_Popup uiRsPopup;

        /* R.S 트레이더스 변수 */
        private Queue<UI_RS_TradersItem> _tradersItemPool; // 비활성화된 아이템 패널 풀링
        private List<UI_RS_TradersItem> _enableTradersItemList; // 활성화된 아이템 패널
        private Dictionary<TradersPanelType, List<RS_TradersItemCloneData>> _tradersAllItemPool; // 각 파트에 맞는 아이템 데이터들
        private TradersPanelType _lastOpenTraderPanel; // 마지막으로 활성화된 트레이더스 패널타입
        
        /* R.S 장바구니 변수 */
        private Queue<UI_RS_BucketList> _bucketListPool; // 비활성화된 장바구니 패널 풀링
        private List<UI_RS_BucketList> _enableBucketList; // 활성화된 장바구니 패널
        private List<RS_TradersItemCloneData> _bucketListItemData; // 장바구니 아이템 데이터
        private int _currentTotalPrice = 0; // 장바구니 최종 합산 금액
        private int _currentTotalItemStack = 0; // 장바구니 최종 제품 합산개수
        
        /* 배송물품 변수 */
        private RS_TradersDeliveryMapper[] _deliveryList;
        
        /* R.S 팝업창 텍스트 로컬라이징 */
        private string _addBucketListTitleText;
        private string _addBucketListButtonText;
        private string _buyBucketItemListTitleText;
        private string _buyBucketItemListButtonText;

        #region Task 퀘스트 클리어 이벤트

        public event TaskHandler.TaskEventHandler TradersBuyTaskHandler;

        #endregion
        
        /* 유저 RS 머니 테스트 */
        public int User_RS_Cash
        {
            get => _user_RS_Cash;
            set
            {
                _user_RS_Cash = value;
                
                rsBucketList.bucketListTotalPriceText1.text = rsBucketList.bucketListTotalPriceText2.text = $"{_currentTotalPrice}$";
                rsBucketList.userTotal_RS_CashText1.text = rsBucketList.userTotal_RS_CashText2.text = $"{_user_RS_Cash}$";
            
                rsBucketList.bucketListTotalItemStackText.text = $"({_currentTotalItemStack})";
                rsBucketList.calculation_RS_CashText.text = $"{_user_RS_Cash}$ - {_currentTotalPrice}$ = {_user_RS_Cash - _currentTotalPrice}$";
                
                CanBuyBucketListStateHandler();
            }
        }

        private int _user_RS_Cash = 0;
        
        private enum TradersPanelType
        {
            None = -1,
            Equip = 0,
            Consumable = 1,
            Service = 2,
            Etc = 3,
        }
        
        public void InitTradersItem(int[] itemIdList)
        {
            InitRsTraders();
            uiRsPopup.gameObject.SetActive(false);
            
            if (itemIdList == null) return;
            foreach (var itemId in itemIdList)
            {
                CreateNewTradersItem(itemId);
            }
        }
        
        private void InitRsTraders()
        {
            // 트레이더스 아이템 UI 오브젝트 풀 초기화
            _tradersItemPool = ObjectPooling.InitializeQueuePool<UI_RS_TradersItem>(10);
            _enableTradersItemList = new List<UI_RS_TradersItem>();
            _tradersAllItemPool = new Dictionary<TradersPanelType, List<RS_TradersItemCloneData>>
            {
                { TradersPanelType.Equip, new List<RS_TradersItemCloneData>()},
                { TradersPanelType.Consumable, new List<RS_TradersItemCloneData>()},
                { TradersPanelType.Service, new List<RS_TradersItemCloneData>()},
                { TradersPanelType.Etc, new List<RS_TradersItemCloneData>()},
            };
            
            // 장바구니 아이템 UI 오브젝트 풀 초기화
            _bucketListPool = ObjectPooling.InitializeQueuePool<UI_RS_BucketList>(5);
            _enableBucketList = new List<UI_RS_BucketList>();
            _bucketListItemData = new List<RS_TradersItemCloneData>();
            
            // 트레이더스 상점 버튼 이벤트 등록
            rsTraders.equipTradersStoreButton.onClick.AddListener(() => ChangeTradersItemPanelHandler(TradersPanelType.Equip));
            rsTraders.consumableTradersStoreButton.onClick.AddListener(() => ChangeTradersItemPanelHandler(TradersPanelType.Consumable));
            rsTraders.serviceTradersStoreButton.onClick.AddListener(() => ChangeTradersItemPanelHandler(TradersPanelType.Service));
            rsTraders.etcTradersStoreButton.onClick.AddListener(() => ChangeTradersItemPanelHandler(TradersPanelType.Etc));
            
            // 장바구니 구매 버튼 이벤트 등록
            rsBucketList.bucketListAllBuyButton.onClick.AddListener(BuyBucketItemList);

            // 풀 Root 비활성화
            rsTraders.equipItemPool.gameObject.SetActive(false);
            rsTraders.consumableItemPool.gameObject.SetActive(false);
            rsTraders.serviceItemPool.gameObject.SetActive(false);
            rsTraders.etcItemPool.gameObject.SetActive(false);
            
            // R.S Tarders PopUp 언어 로컬라이징 캐싱 
            _addBucketListTitleText = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.PC_OS_TradersUI_Table,
                                                                                      LocalizationTable.PC_OS_TradersUI_Table.Traders_Popup_Text);
            _addBucketListButtonText = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.PC_OS_TradersUI_Table,
                                                                                    LocalizationTable.PC_OS_TradersUI_Table.Traders_Popup_Button);
            _buyBucketItemListTitleText = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.PC_OS_TradersUI_Table,
                                                                                        LocalizationTable.PC_OS_TradersUI_Table.Traders_BuyPopup_Text);
            _buyBucketItemListButtonText = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.PC_OS_TradersUI_Table,
                                                                                    LocalizationTable.PC_OS_TradersUI_Table.Traders_BuyPopup_Button);
            
            // 트레이더스 초기화면 설정
            ChangeTradersItemPanelHandler(TradersPanelType.Equip);
        }

        /// <summary>
        /// 배송물품 세이브 데이터
        /// </summary>
        public RS_TradersDeliveryMapper[] SaveDeliveryItem()
        {
            return _deliveryList;
        }

        #region 이벤트 핸들러

        /// <summary>
        /// 트레이더스 상점 버튼 클릭 이벤트
        /// </summary>
        private void ChangeTradersItemPanelHandler(TradersPanelType tradersPanelType)
        {
            SwapTradersItemPanel(tradersPanelType);
            _lastOpenTraderPanel = tradersPanelType;
        }

        /// <summary>
        /// 트레이더스 상품을 장바구니 추가 이벤트
        /// </summary>
        private void AddBucketList(int itemId)
        {
            var itemData = NoManualHotelManager.Instance.InventoryManager.GetItemCloneData(itemId);
            RS_TradersItemCloneData addBucketRsItemData = Get_RS_TradersItemCloneData(itemData);
            if (addBucketRsItemData == null) return;
            
            uiRsPopup.SetText(true, _addBucketListTitleText, _addBucketListButtonText);
            
            // 장바구니 중복 추가 예외처리
            foreach (var inBucketItemData in _bucketListItemData)
            {
                // 장바구니 아이템 데이터 얻기
                if (inBucketItemData.itemId == addBucketRsItemData.itemId)
                {
                    foreach (var bucketList in _enableBucketList)
                    {
                        // 장바구니 UI 데이터 얻기
                        if (bucketList.itemId == inBucketItemData.itemId)
                        {
                            BucketListStackAddEvent(bucketList, inBucketItemData);
                            break;
                        }
                    }
                    return;
                }
            }
            
            // 장바구니 목록 새로 추가
            var bucketPanel = ObjectPooling.GetFromQueuePool(_bucketListPool);
            if (bucketPanel == null)
                bucketPanel = CreateBucketListItemPrefab();

            SetBucketListItemInfo(bucketPanel, addBucketRsItemData);
            bucketPanel.transform.SetAsLastSibling(); // 장바구니 목록 순서대로 패널 정렬
            BucketListStackUpdateEvent(bucketPanel, addBucketRsItemData, 1);
            _enableBucketList.Add(bucketPanel);
            _bucketListItemData.Add(addBucketRsItemData);
        }

        /// <summary>
        /// 장바구니 제거 이벤트
        /// </summary>
        private void RemoveBucketList(UI_RS_BucketList bucketListPanel, RS_TradersItemCloneData inBucketListItem)
        {
            // 1. _enableBucketList 찾아 제거
            // 2. UI_RS_BucketList에 표현된 데이터 모두 제거
            // 3. 큐에 다시 넣어줌
            // 4. 합산 금액 다시 계산

            if (_enableBucketList.Contains(bucketListPanel) && _bucketListItemData.Contains(inBucketListItem))
            {
                _enableBucketList.Remove(bucketListPanel);
                _bucketListItemData.Remove(inBucketListItem);
                BucketListStackUpdateEvent(bucketListPanel, inBucketListItem, inBucketListItem.itemStack * -1);
            }
            ClearBucketListItemInfo(bucketListPanel);
        }
        
        
        /// <summary>
        /// 장바구니 제품 수량 업데이트 이벤트
        /// </summary>
        private void BucketListStackUpdateEvent(UI_RS_BucketList bucketListPanel, RS_TradersItemCloneData bucketListItemData, int change)
        {
            // 수량 변경
            bucketListItemData.itemStack += change;
            int totalPrice = CalculationBucketItemPrice(bucketListItemData);
            bucketListPanel.UpdateItemStackAndPriceText(bucketListItemData.itemStack.ToString(), totalPrice.ToString());
            
            // 최총 금액 계산
            UpdateBucketListTotalPrice(change, bucketListItemData.itemPrice * change);
        }

        /// <summary>
        /// 장바구니 제품 수량 증가 이벤트
        /// </summary>
        private void BucketListStackAddEvent(UI_RS_BucketList bucketListPanel, RS_TradersItemCloneData bucketListItemData)
        {
            // 수량증가 제한이 없음
            BucketListStackUpdateEvent(bucketListPanel, bucketListItemData, 1);
        }

        /// <summary>
        /// 장바구니 제품 수량 감소 이벤트
        /// </summary>
        private void BucketListStackSumEvent(UI_RS_BucketList bucketListPanel, RS_TradersItemCloneData bucketListItemData)
        {
            // 수량 감소 제한이 1미만 떨어질 수 없음
            if (bucketListItemData.itemStack <= 1) return;
            BucketListStackUpdateEvent(bucketListPanel, bucketListItemData, -1);
        }
        
        /// <summary>
        /// 장바구니 구매 상태 이벤트
        /// </summary>
        private void CanBuyBucketListStateHandler()
        {
            bool canBuyState = User_RS_Cash >= _currentTotalPrice && _currentTotalItemStack > 0;
            // 구매버튼 제한
            rsBucketList.bucketListAllBuyButton.interactable = canBuyState;
        }

        /// <summary>
        /// 장바구니 구매 버튼 이벤트
        /// </summary>
        private void BuyBucketItemList()
        {
            RS_TradersDeliveryMapper[] tradersDeliveryMappers = new RS_TradersDeliveryMapper[_bucketListItemData.Count];
            for (int i = 0; i < tradersDeliveryMappers.Length; i++)
            {
                tradersDeliveryMappers[i] = new RS_TradersDeliveryMapper(_bucketListItemData[i].itemId, _bucketListItemData[i].itemStack);
            }

            // 배송풀품 데이터 기록
            _deliveryList = tradersDeliveryMappers;
            
            string taskID = TaskHandler.TaskID.Traders_Buy.ToString();
            // 돈 차감 및 BucketPanel 업데이트   
            User_RS_Cash = Mathf.Max(User_RS_Cash - _currentTotalPrice, 0);
            for (int i = tradersDeliveryMappers.Length - 1; i >= 0; i--)
            {
                // TradersBuy Task 이벤트 트리거
                string taskTargetID = _bucketListItemData[i].itemId.ToString();
                TradersBuyTaskHandler?.Invoke(taskID, taskTargetID);
                RemoveBucketList(_enableBucketList[i], _bucketListItemData[i]);
            }
            
            uiRsPopup.SetText(false, _buyBucketItemListTitleText, _buyBucketItemListButtonText);
        }

        #endregion

        #region 트레이더스 패널 교체

        /// <summary>
        /// 트레이더스 패널 아이템 리스트 교체
        /// </summary>
        private void SwapTradersItemPanel(TradersPanelType targetPanelType)
        {
            // 1. 먼저 기존에 있는 아이템 패널을 회수
            // 2. 회수 후 아이템 패널 데이터 초기화
            // 3. Queue에 모두 넣기
            // 4. targetPanelType 아이템 패널 개수만큼 Queue에서 꺼내기
            // 5. 아이템 패널 데이터 넣기

            foreach (var itemPanel in _enableTradersItemList)
            {
                ClearTradersItemInfo(itemPanel);
            }
            _enableTradersItemList.Clear();

            foreach (var itemData in _tradersAllItemPool[targetPanelType])
            {
                UI_RS_TradersItem itemPanel = ObjectPooling.GetFromQueuePool(_tradersItemPool);
                if (itemPanel == null)
                    itemPanel = CreateTradersItemPrefab();

                SetTradersItemInfo(itemPanel, itemData);
                itemPanel.transform.SetAsLastSibling(); // _tradersAllItemPool의 리스트 순서대로 아이템 패널 정렬
                _enableTradersItemList.Add(itemPanel);
            }
        }

        #endregion

        #region 트레이더스 아이템 패널 CRUD

        /// <summary>
        /// 신규 트레이더스 아이템 등록
        /// </summary>
        public void CreateNewTradersItem(int itemId, bool isRunTimeUpdate = false)
        {
            SetRsTradersItemData(itemId, isRunTimeUpdate);
        }

        
        /// <summary>
        /// 프리팹 생성
        /// </summary>
        private UI_RS_TradersItem CreateTradersItemPrefab()
        {
            GameObject itemPanel = Instantiate(rsTraders.tradersItemPrefab, rsTraders.tradersItemPanel);
            itemPanel.SetActive(false);
            return itemPanel.GetComponent<UI_RS_TradersItem>();
        }
        

        /// <summary>
        /// 트레이더스 아이템 정보 등록
        /// </summary>
        private void SetTradersItemInfo(UI_RS_TradersItem itemPanel, RS_TradersItemCloneData tradersItemData)
        {
            itemPanel.SetRsTradersItemInfo(tradersItemData.itemId,
                tradersItemData.itemIcon,
                tradersItemData.itemTitel,
                tradersItemData.itemPrice.ToString(),
                AddBucketList);
            itemPanel.gameObject.SetActive(true);
        }

        /// <summary>
        /// 트레이더스 아이템 정보 초기화
        /// </summary>
        private void ClearTradersItemInfo(UI_RS_TradersItem itemPanel)
        {
            itemPanel.gameObject.SetActive(false);
            itemPanel.ClearRsTradersItemInfo();
            ObjectPooling.ReturnToQueuePool(_tradersItemPool, itemPanel);
        }

        /// <summary>
        /// 트레이더스 판매 아이템 정보 등록
        /// </summary>
        private void SetRsTradersItemData(int itemId, bool isRunTimeUpdate = false)
        {
            RS_TradersItemCloneData rsTradersItemCloneData = Get_RS_TradersItemCloneData(itemId);
            if (rsTradersItemCloneData == null) return;
            
            TradersPanelType tradersPanelType = TradersPanelType.None;
            List<RS_TradersItemCloneData> targetList = null;

            switch (rsTradersItemCloneData.InventoryItemUseType)
            {
                case InventoryItemUseType.Equipment:
                    targetList = _tradersAllItemPool[TradersPanelType.Equip];
                    tradersPanelType = TradersPanelType.Equip;
                    break;
        
                case InventoryItemUseType.Food:
                case InventoryItemUseType.Bullet:
                case InventoryItemUseType.ItemPart:
                    targetList = _tradersAllItemPool[TradersPanelType.Consumable];
                    tradersPanelType = TradersPanelType.Consumable;
                    break;
        
                case InventoryItemUseType.Service:
                case InventoryItemUseType.Key:
                case InventoryItemUseType.Manual:
                    targetList = _tradersAllItemPool[TradersPanelType.Service];
                    tradersPanelType = TradersPanelType.Service;
                    break;
        
                case InventoryItemUseType.Etc:
                case InventoryItemUseType.None:
                    targetList = _tradersAllItemPool[TradersPanelType.Etc];
                    tradersPanelType = TradersPanelType.Etc;
                    break;
            }

            if (targetList != null && !targetList.Contains(rsTradersItemCloneData))
            {
                // 동일한 아이템 id 값 중복방지
                foreach (var tradersItemCloneData in targetList)
                {
                    if (tradersItemCloneData.itemId == rsTradersItemCloneData.itemId) 
                        return;
                }
                targetList.Add(rsTradersItemCloneData);
                if (isRunTimeUpdate && tradersPanelType == _lastOpenTraderPanel) // 실시간으로 R.S 트레이더스 재고 업데이트
                {
                    UI_RS_TradersItem itemPanel = ObjectPooling.GetFromQueuePool(_tradersItemPool);
                    if (itemPanel == null)
                        itemPanel = CreateTradersItemPrefab();

                    SetTradersItemInfo(itemPanel, rsTradersItemCloneData);
                    itemPanel.transform.SetAsLastSibling(); // _tradersAllItemPool의 리스트 순서대로 아이템 패널 정렬
                    _enableTradersItemList.Add(itemPanel);
                }
            }
        }
        
        #endregion

        #region 장바구니 아이템 패널 CRUD

        /// <summary>
        /// 프리팹 생성
        /// </summary>
        private UI_RS_BucketList CreateBucketListItemPrefab()
        {
            GameObject itemPanel = Instantiate(rsBucketList.bucketListItemPrefab, rsBucketList.bucketListPanel);
            itemPanel.SetActive(false);
            return itemPanel.GetComponent<UI_RS_BucketList>();
        }
        
        /// <summary>
        /// 장비구니 아이템 정보 등록
        /// </summary>
        private void SetBucketListItemInfo(UI_RS_BucketList bucketPanel, RS_TradersItemCloneData tradersItemData)
        {
            bucketPanel.SetRsBucketListItemInfo(tradersItemData,
                tradersItemData.itemId,
                tradersItemData.itemTitel,
                tradersItemData.itemPrice,
                tradersItemData.itemIcon,
                tradersItemData.itemStack,
                RemoveBucketList, BucketListStackAddEvent, BucketListStackSumEvent);
            bucketPanel.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// 장비구니 아이템 정보 초기화
        /// </summary>
        private void ClearBucketListItemInfo(UI_RS_BucketList bucketPanel)
        {
            bucketPanel.gameObject.SetActive(false);
            bucketPanel.ClearRsBucketListItemInfo();
            ObjectPooling.ReturnToQueuePool(_bucketListPool, bucketPanel);
        }

        /// <summary>
        /// 장바구니 슬롯 아이템 가격 합산
        /// </summary>
        private int CalculationBucketItemPrice(RS_TradersItemCloneData inBucketItemData)
        {
            return inBucketItemData.itemPrice * inBucketItemData.itemStack;
        }

        /// <summary>
        /// 장바구니 최종 금액 계산
        /// </summary>
        private void UpdateBucketListTotalPrice(int itemStack, int itemPrice)
        {
            _currentTotalItemStack += itemStack;
            _currentTotalPrice += itemPrice;

            rsBucketList.bucketListTotalPriceText1.text = rsBucketList.bucketListTotalPriceText2.text = $"{_currentTotalPrice}$";
            rsBucketList.userTotal_RS_CashText1.text = rsBucketList.userTotal_RS_CashText2.text = $"{User_RS_Cash}$";
            
            rsBucketList.bucketListTotalItemStackText.text = $"({_currentTotalItemStack})";
            rsBucketList.calculation_RS_CashText.text = $"{User_RS_Cash}$ - {_currentTotalPrice}$ = {User_RS_Cash - _currentTotalPrice}$";
            
            CanBuyBucketListStateHandler();
        }

        #endregion

        /// <summary>
        /// 아이템 고유 ID를 이용해 RS 트레이더스 복사본 데이터 얻기
        /// </summary>
        private RS_TradersItemCloneData Get_RS_TradersItemCloneData(int itemId)
        {
            var itemData = NoManualHotelManager.Instance.InventoryManager.GetItemCloneData(itemId);
            if (itemData == null) return null;

            return new RS_TradersItemCloneData(itemData);
        }

        /// <summary>
        /// ItemCloneData을 이용해 RS 트레이더스에 판매 중인 제품 데이터 얻기
        /// </summary>
        private RS_TradersItemCloneData Get_RS_TradersItemCloneData(ItemCloneData itemCloneData)
        {
            List<RS_TradersItemCloneData> targetList = null;
            RS_TradersItemCloneData rsTradersItemData = null;

            switch (itemCloneData.InventoryItemUseType)
            {
                case InventoryItemUseType.Equipment:
                    targetList = _tradersAllItemPool[TradersPanelType.Equip];
                    break;
        
                case InventoryItemUseType.Food:
                case InventoryItemUseType.Bullet:
                case InventoryItemUseType.ItemPart:
                    targetList = _tradersAllItemPool[TradersPanelType.Consumable];
                    break;
        
                case InventoryItemUseType.Service:
                case InventoryItemUseType.Key:
                case InventoryItemUseType.Manual:
                    targetList = _tradersAllItemPool[TradersPanelType.Service];
                    break;
        
                case InventoryItemUseType.Etc:
                case InventoryItemUseType.None:
                    targetList = _tradersAllItemPool[TradersPanelType.Etc];
                    break;
            }

            if (targetList != null)
            {
                foreach (var rsTradersItemCloneData in targetList)
                {
                    if (rsTradersItemCloneData.itemId == itemCloneData.itemId)
                    {
                        rsTradersItemData = rsTradersItemCloneData;
                        break;
                    }
                }
            }

            return rsTradersItemData;
        }
    }

    /// <summary>
    /// R.S 트레이더스에서 사용할 아이템 데이터 일부분 복사
    /// </summary>
    public class RS_TradersItemCloneData
    {
        public int itemId;
        public string itemTitel;
        public Sprite itemIcon = null;
        public string description = "New Item Description";
        public InventoryItemUseType InventoryItemUseType = InventoryItemUseType.None;
        public int itemPrice;
        public int itemStack = 0;

        public RS_TradersItemCloneData(ItemCloneData itemCloneData)
        {
            this.itemId = itemCloneData.itemId;
            this.itemTitel = itemCloneData.itemTitle;
            this.itemIcon = itemCloneData.itemIcon;
            this.description = itemCloneData.description;
            this.InventoryItemUseType = itemCloneData.InventoryItemUseType;
            this.itemPrice = itemCloneData.itemSettings.itemPrice;
        }
    }

    public class RS_TradersItemList
    {
        public int Days;
        public int[] itemIdList;
    }
}
