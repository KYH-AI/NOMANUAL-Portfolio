using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using NoManual.Utils;
using TMPro;

namespace NoManual.UI
{
    public class UI_OccasionHotelHomePage : MonoBehaviour, IHomePageExit
    {

        #region UI

        [Serializable]
        public struct MainPanels
        {
            [Header("Main Content Size Filter")] public ContentSizeFitter mainContentSizeFilter;
            [Header("로그인 창")] public GameObject loginPanel;
            [Header("로비 창")] public GameObject lobbyPanel;
            [Header("예약자 명단 창")] public GameObject reservationListPanel;
            [Header("객실 예약 창")] public GameObject guestRoomListPanel;
        }

        [Serializable]
        public struct ContextButtons
        {
            [Header("로그아웃 패널 버튼")] public Button logOutButton;
            [Header("메인 패널 버튼")] public Button lobbyButton;
            [Header("예약자 명단 패널 버튼")] public Button reservationListButton;
            [Header("객실 패널 버튼")] public Button guestRoomListButton;
        }

        [Serializable]
        public struct Login
        {
            [Header("로그인 실패")] public GameObject failLoginPanel;
            [Header("로그인 버튼")] public Button loginButton;
            [Header("ID 입력 창")] public TMP_InputField inputFieldId;
            [Header("PW 입력 창")] public TMP_InputField inputFieldPw;
        }

        [Serializable]
        public struct Lobby
        {
            [Header("총 투수객 인원 텍스트")] public UnityEngine.Localization.Components.LocalizeStringEvent totalCustomerStackText;
            [Header("예약 대기 인원 텍스트")] public UnityEngine.Localization.Components.LocalizeStringEvent totalReservationStackText;
            [Header("예약 완료 인원 텍스트")] public UnityEngine.Localization.Components.LocalizeStringEvent totalCompleteReservationStackText;
            [Header("예약이 없는 방")] public UnityEngine.Localization.Components.LocalizeStringEvent totalEmptyGuestRoomStackText;
            [Header("점검 중인 방")] public UnityEngine.Localization.Components.LocalizeStringEvent totalForbiddenGuestRoomStackText;
        }
        
        [Serializable]
        public struct Reservation
        {
            [Header("예약자 프리팹")] public GameObject reservationItemPrefab;
            [Header("예약저 정보 Root")] public Transform reservationPanelRoot;
        }

        [Serializable]
        public struct GuestRoom
        {
            [Header("예약자 Drag&Drop 프리팹")] public GameObject reservationItemDragDropPrefab;
            [Header("예약자 Drag&Drop Root")] public Transform reservationItemDragDropRoot;
            [Header("게스트 룸 Floor 프리팹")] public GameObject guestRoomFloorListItemPrefab;
            [Header("게스트 룸 Floor Root")] public Transform guestRoomFloorRoot;
            [Header("빈 객실 색상")] public Color emptyColor;
            [Header("예약 객실 색상")] public Color reservationColor;
            [Header("투숙 객실 색상")] public Color stayColor;
            [Header("점검 객식 색상")] public Color forbiddenColor;
        }

        [Space(10)]
        public MainPanels mainPanels = new MainPanels();
        [Space(10)]
        public ContextButtons contextButtons = new ContextButtons();
        [Space(10)]
        public Login login = new Login();
        [Space(10)]
        public Lobby lobby = new Lobby();
        [Space(10)]
        public Reservation reservation = new Reservation();
        [Space(10)]
        public GuestRoom guestRoom = new GuestRoom();

        #endregion
        
        private OccasionHotelHomePageHelper _helper;
        private bool _isLogin = false;
        private GameObject _lastPanel = null;
        private enum PanelType
        {
            None = -1,
            Lobby = 0,
            Reservation = 1,
            GuestRoom = 2,
        }
        
        private List<UI_ReservationItemDragDrop> reservationItemDragDrops = new List<UI_ReservationItemDragDrop>();
        private Queue<UI_ReservationItemDragDrop> reservationItemDragDropPool;
        

        private void Awake()
        {
            // 레퍼런스 추가

            bool isHotelMap = Managers.HotelManager.HasReference;
            _helper = isHotelMap ? 
                     Managers.HotelManager.Instance.PcOSManager.ohHomePage.occasionHotelHomePageHelper : 
                     Tutorial.HandoverManager.Instance.pcOSManager.ohHomePage.occasionHotelHomePageHelper;

            if (isHotelMap)
            {
                Managers.HotelManager.Instance.PcOSManager.ohHomePage.occasionHotelHomePageHelper.HomePage = this;
            }
            else
            {
                Tutorial.HandoverManager.Instance.pcOSManager.ohHomePage.occasionHotelHomePageHelper.HomePage = this;
            }
            
            
            // 풀링
            reservationItemDragDropPool = ObjectPooling.InitializeQueuePool<UI_ReservationItemDragDrop>(5);
            
            // 패널 버튼 이벤트 등록
            contextButtons.logOutButton.onClick.AddListener(() => ChangeLoginState(false));
            contextButtons.lobbyButton.onClick.AddListener(() => ChangePanel(mainPanels.lobbyPanel, PanelType.Lobby));
            contextButtons.reservationListButton.onClick.AddListener(() => ChangePanel(mainPanels.reservationListPanel, PanelType.Reservation));
            contextButtons.guestRoomListButton.onClick.AddListener(() => ChangePanel(mainPanels.guestRoomListPanel, PanelType.GuestRoom));
            
            // 로그인 버튼 이벤트 등록
            login.loginButton.onClick.AddListener(()=> LoginHomePageEvent(login.inputFieldId.text, login.inputFieldPw.text));
            
            // 패널 초기화
            InitReservationPanel();
            InitGuestRoomPanel();

            LoginInit(_isLogin);

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }
        

        #region 로그인
        
        private void LoginInit(bool loginState)
        {
            ChangeLoginState(loginState);
        }

        /// <summary>
        /// 로그인 상태 교체
        /// </summary>
        private void ChangeLoginState(bool loginState)
        {
            this._isLogin = loginState;
            CheckLogInState(_isLogin);
        }

        /// <summary>
        /// 홈페이지 로그인
        /// </summary>
        private void LoginHomePageEvent(string id, string pw)
        {
            if (_helper.AdminAccount.LoginCheck(id, pw))
            {
                ChangeLoginState(true);
                _helper.LoginTaskTrigger();
            }
            else
                login.failLoginPanel.SetActive(true);
        }

        /// <summary>
        /// 로그인 상태 확인
        /// </summary>
        private void CheckLogInState(bool isLoginState)
        {
            login.failLoginPanel.SetActive(false);
            login.inputFieldId.text = string.Empty;
            login.inputFieldPw.text = string.Empty;
            
            contextButtons.lobbyButton.gameObject.SetActive(isLoginState);
            contextButtons.logOutButton.gameObject.SetActive(isLoginState);
            contextButtons.reservationListButton.gameObject.SetActive(isLoginState);
            contextButtons.guestRoomListButton.gameObject.SetActive(isLoginState);
            GameObject targetPanel = isLoginState ? mainPanels.lobbyPanel : mainPanels.loginPanel;
            ChangePanel(targetPanel, isLoginState ? PanelType.Lobby : PanelType.None);
        }
        
        
        #endregion

        #region 페이지

        /// <summary>
        /// 페이지 교체
        /// </summary>
        private void ChangePanel(GameObject targetPanel, PanelType panelType = PanelType.None)
        {
            if (_lastPanel) _lastPanel.SetActive(false);

            switch (panelType)
            {
                case PanelType.Lobby: UpdateLobbyPanel(); break;
                case PanelType.Reservation : UpdateReservationPanel(); break;
                case PanelType.GuestRoom : UpdateGuestRoomPanel(); break;
            }
            
            targetPanel.SetActive(true);
            _lastPanel = targetPanel;
        }

        #endregion
        
        #region 로비 패널

        private void UpdateLobbyPanel()
        {
            UpdateLobbyText(true);
        }

        public void UpdateLobbyText(bool manualUpdateMode)
        {
            // 수동 업데이트 확인
            if (!manualUpdateMode && !mainPanels.lobbyPanel.activeSelf) return;
            
            // 1. 총 사용중인 객실 수 (Stay 방 찾기)
            // 2. 총 예약 대기 인원 수 (예약 명단 리스트 찾기)
            // 3. 총 예약 완료 된 인원 수 (Reservation 방 찾기)
            // 4. 총 빈 객실 수 (Empty 방 찾기)
            // 5. 총 점검 객실 수 (Busy 방 찾기)

            int totalStayGuestRoomStack = 0;
            int totalReservationGuestStack = 0;
            int totalCompleteReservationGuestStack = 0;
            int totalEmptyGuestRoomStack = 0;
            int totalBusyGuestRoomStack = 0;

            foreach (var roomData in _helper.guestRoomData.Values)
            {
                var roomState = (UI_GuestRoomItem.RoomState)roomData.roomState;
                if (roomState is UI_GuestRoomItem.RoomState.Stay)
                    totalStayGuestRoomStack++;
                if (roomState is UI_GuestRoomItem.RoomState.Reservation)
                    totalCompleteReservationGuestStack++;
                if (roomState is UI_GuestRoomItem.RoomState.Empty)
                    totalEmptyGuestRoomStack++;
                if (roomState is UI_GuestRoomItem.RoomState.Forbidden)
                    totalBusyGuestRoomStack++;
            }
            totalReservationGuestStack = _helper.reservationCustomers.Count;
            
            
            LocalizationVariableHelper.UpdateLocalizationVariableText(lobby.totalCustomerStackText, 
                                                                        LocalizationVariableHelper.PC_OS_HotelHomePage_VariableKey.customer_stack, 
                                                                        totalStayGuestRoomStack);
            LocalizationVariableHelper.UpdateLocalizationVariableText(lobby.totalReservationStackText, 
                                                                        LocalizationVariableHelper.PC_OS_HotelHomePage_VariableKey.reservation_stack, 
                                                                        totalReservationGuestStack);
            LocalizationVariableHelper.UpdateLocalizationVariableText(lobby.totalCompleteReservationStackText, 
                                                                        LocalizationVariableHelper.PC_OS_HotelHomePage_VariableKey.complete_reservation_stack, 
                                                                        totalCompleteReservationGuestStack);
            LocalizationVariableHelper.UpdateLocalizationVariableText(lobby.totalEmptyGuestRoomStackText, 
                                                                        LocalizationVariableHelper.PC_OS_HotelHomePage_VariableKey.empty_guestroom_stack, 
                                                                        totalEmptyGuestRoomStack);
            LocalizationVariableHelper.UpdateLocalizationVariableText(lobby.totalForbiddenGuestRoomStackText, 
                                                                        LocalizationVariableHelper.PC_OS_HotelHomePage_VariableKey.forbidden_guestroom_stack, 
                                                                        totalBusyGuestRoomStack);
        }
        

        #endregion

        #region 예약 명단 패널

        private void InitReservationPanel()
        {
            var reservationItems = _helper.reservationItems;
             if (reservationItems.Count > 0)
             {
                 // 보관중인 UI 모두 활성화
                 foreach (var reservationData in reservationItems)
                 {
                     reservationData.transform.SetParent(reservation.reservationPanelRoot);
                     reservationData.gameObject.SetActive(true);
                 }
             }
             
            var reservationCustomer = _helper.reservationCustomers;
            // 추후에 들어온 예약자 명단 업데이트
            foreach (var customer in reservationCustomer.Values)
            {
                AddNewReservationCustomer(customer.customerName);
            }
            
        }

        private void UpdateReservationPanel()
        {
            var reservationItems  = _helper.reservationItems;
            foreach (var reservationData in reservationItems)
                reservationData.gameObject.SetActive(true);
            // size filter 업데이트
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)mainPanels.mainContentSizeFilter.transform);
        }

        /// <summary>
        /// 예약자 추가
        /// </summary>
        public void AddNewReservationCustomer(string customerName)
        {
            var reservationItems = _helper.reservationItems;
            foreach (var reservationData in reservationItems)
            {
                if (reservationData.GetCustomerName().Equals(customerName))
                {
                    return;
                }
            }
            var uiReservationItem = ObjectPooling.GetFromQueuePool(_helper.reservationItemPool);
            if(uiReservationItem == null)
                uiReservationItem = CreateReservationItem();
            uiReservationItem.SetReservationData(customerName, _helper.RemoveReservationCustomer);
            uiReservationItem.transform.SetAsLastSibling();
            uiReservationItem.gameObject.SetActive(true);
            reservationItems.Add(uiReservationItem);
            
            UpdateReservation(false);
            UpdateLobbyText(false);
        }

        /// <summary>
        /// 예약자 Drag UI 삭제
        /// </summary>
        public void RemoveReservationDragDropUI(string customerName)
        {
            var dragUIDropItem = reservationItemDragDrops.FirstOrDefault(item => item.GetCustomerName().Equals(customerName));
            if (dragUIDropItem != null)
            {
                dragUIDropItem.gameObject.SetActive(false);
                dragUIDropItem.SetParentAndPosition(guestRoom.reservationItemDragDropRoot); // 드래그 UI 부모위치 재설정
                dragUIDropItem.ClearReservationData();
                reservationItemDragDrops.Remove(dragUIDropItem);
                ObjectPooling.ReturnToQueuePool(reservationItemDragDropPool, dragUIDropItem);
            }
        }

        /// <summary>
        /// 예약자 프리팹 생성
        /// </summary>
        private UI_ReservationItem CreateReservationItem()
        {
            GameObject uiReservationItem = Instantiate(reservation.reservationItemPrefab, reservation.reservationPanelRoot);
            uiReservationItem.SetActive(false);
            return uiReservationItem.GetComponent<UI_ReservationItem>();
        }

        #endregion

        #region 객실 패널

        private void InitGuestRoomPanel()
        {
            var guestRoomFloorList = _helper.guestRoomFloors;
            if (guestRoomFloorList.Count > 0)
            {
                foreach (var floor in guestRoomFloorList)
                {
                    floor.transform.SetParent(guestRoom.guestRoomFloorRoot);
                    floor.gameObject.SetActive(true);
                }
            }
            else
            {
                var roomItems = _helper.guestRoomItems;
                // Floor 생성
                for (int i = 2; i <= _helper.FLOOR_MAX_STACK; i++)
                {
                    var roomDataList = _helper.GetGuestRoomData(i);
                    var floorObject = Instantiate(guestRoom.guestRoomFloorListItemPrefab, guestRoom.guestRoomFloorRoot);
                    var floor = floorObject.GetComponent<UI_GuestRoomFloor>();
                    // Room 생성
                    List<UI_GuestRoomItem> guestRoomItems = floor.SetGuestRoomFloor(_helper.ROOM_MAX_STACK, 
                                                                                    i, 
                                                                                    roomDataList,
                                                                                    _helper.GuestRoomStateHandler, 
                                                                                    _helper.RemoveReservationCustomer);

                    // Floor 정보 저장
                    guestRoomFloorList.Add(floor);
                    // Room 정보 저장
                    foreach (var roomInfo in guestRoomItems)
                        roomItems[roomInfo.roomNumber] = roomInfo;
                }
            }
            AutoUpdateGuestRoomState();
        }

        private void UpdateGuestRoomPanel()
        {
            UpdateReservation(true);
        }

        /// <summary>
        /// 객실 예약자 정보 업데이트
        /// </summary>
        private void UpdateReservation(bool manualUpdateMode)
        {
            // 수동 업데이트 확인
            if (!manualUpdateMode && !mainPanels.guestRoomListPanel.activeSelf) return;
            
            var reservationItems = _helper.reservationItems;
            
            // 기존에 예약자 명단(DragDrop) 초기화
            foreach (var dragUI in reservationItemDragDrops)
            {
                dragUI.gameObject.SetActive(false);
                dragUI.ClearReservationData();
                ObjectPooling.ReturnToQueuePool(reservationItemDragDropPool, dragUI);
            }
            reservationItemDragDrops.Clear();
            
            // 예약자 명단 정보 가져오기
            foreach (var item in reservationItems)
            {
                var dragUI = ObjectPooling.GetFromQueuePool(reservationItemDragDropPool);
                if (dragUI == null)
                    dragUI = CreateReservationItemDragDrop();
                dragUI.SetReservationData(item.GetCustomerName(), mainPanels.guestRoomListPanel.transform);
                dragUI.transform.SetAsLastSibling();
                reservationItemDragDrops.Add(dragUI);
                dragUI.gameObject.SetActive(true);
            }
            // size filter 업데이트
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)mainPanels.mainContentSizeFilter.transform);
  
        }
        
        /// <summary>
        /// 모든 객실 상태 업데이트
        /// </summary>
        private void AutoUpdateGuestRoomState()
        {
            var roomDataList = _helper.guestRoomData;
            var roomItemList = _helper.guestRoomItems;
            foreach (var roomData in roomDataList.Values)
            {
                roomItemList[roomData.roomNumber].ChangeRoomState((UI_GuestRoomItem.RoomState)roomData.roomState, GetGuestRoomBackGroundColor((UI_GuestRoomItem.RoomState)roomData.roomState));
            }
        }
        
        /// <summary>
        /// 객실 상태 UI 업데이트
        /// </summary>
        public void ChangeGuestRoomUiState(UI_GuestRoomItem guestRoomItem, UI_GuestRoomItem.RoomState targetState)
        {
            guestRoomItem.ChangeRoomState(targetState, GetGuestRoomBackGroundColor(targetState));
            UpdateLobbyText(false);
        }

        /// <summary>
        /// 객실 상태에 맞는 배경색 가져오기
        /// </summary>
        private Color GetGuestRoomBackGroundColor(UI_GuestRoomItem.RoomState roomState)
        {
            Color targetColor = guestRoom.emptyColor;
            switch (roomState)
            {
                case UI_GuestRoomItem.RoomState.Empty:
                    targetColor = guestRoom.emptyColor;
                    break;
                case UI_GuestRoomItem.RoomState.Reservation:
                    targetColor = guestRoom.reservationColor;
                    break;
                case UI_GuestRoomItem.RoomState.Stay:
                    targetColor = guestRoom.stayColor;
                    break;
                case UI_GuestRoomItem.RoomState.Forbidden:
                    targetColor = guestRoom.forbiddenColor;
                    break;
            }
            return targetColor;
        }
        

        private UI_ReservationItemDragDrop CreateReservationItemDragDrop()
        {
            var uiReservationItem = Instantiate(guestRoom.reservationItemDragDropPrefab, guestRoom.reservationItemDragDropRoot).GetComponent<UI_ReservationItemDragDrop>();
            uiReservationItem.InitReservationItemUI();
            uiReservationItem.gameObject.SetActive(false);
            return uiReservationItem;
        }

        #endregion
        
        /// <summary>
        /// 홈페이지 종료 이벤트
        /// </summary>
        public void HomePageExitEvent()
        {
            // UI 아이템 객체들 부모 위치 변경
            _helper.SetParentItemObjectPool();
        }
    }
}


