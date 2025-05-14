using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NoManual.Managers;
using NoManual.Task;
using UnityEngine;
using NoManual.Utils;

namespace NoManual.UI
{
    public class OccasionHotelHomePageHelper : MonoBehaviour
    {
        public UI_OccasionHotelHomePage HomePage { get; set; }
        public OccasionHotelAccount AdminAccount { get; } = new OccasionHotelAccount("admin", "1234");

        #region 예약자 데이터

        public readonly List<UI_ReservationItem> reservationItems = new List<UI_ReservationItem>();
        public readonly Dictionary<string, Customer> reservationCustomers = new Dictionary<string, Customer>(); // 홈페이지 예약자 리스트
        private readonly Dictionary<string, Customer> _sponsorCustomers = new Dictionary<string, Customer>(); // 모든 후원자 데이터 리스트
        [HideInInspector] public List<string> reservedCustomer = new List<string>(); // 예약된 기록이 있는 예약자 이름 리스트 (PlayerSaveData)에서 얻어야함 & 예약자 기록 세이브 데이터 

        #endregion

        #region 게스트 룸 데이터

        public readonly int FLOOR_MAX_STACK = 3; // (2~3)
        public readonly int ROOM_MAX_STACK = 15; // (1~15)
        
        public readonly List<UI_GuestRoomFloor> guestRoomFloors = new List<UI_GuestRoomFloor>();
        public readonly Dictionary<int, UI_GuestRoomItem> guestRoomItems = new Dictionary<int, UI_GuestRoomItem>();
        public Dictionary<int, GuestRoom> guestRoomData = new Dictionary<int, GuestRoom>(); // 모든 객실 데이터 정보

        #endregion

        #region 오브젝트 풀

        public Queue<UI_ReservationItem> reservationItemPool;
        
        #endregion

        #region Task 퀘스트 클리어 이벤트

        public event TaskHandler.TaskEventHandler homePageLoginTaskHandler;
        public event TaskHandler.TaskEventHandler TargetReserveTaskHanlder;
        public event TaskHandler.ManualTaskEventHandler AllClearReserveTaskHanlder;

        #endregion

        private void Awake()
        {
            reservationItemPool = ObjectPooling.InitializeQueuePool<UI_ReservationItem>(5);
         //   LoadGuestRoomData();
         //  LoadReservationData();
        }

        public void SetParentItemObjectPool()
        {
            // 기록해야할 오브젝트 
            // 1. 예약자 명단
            // 2. 객실 Floor
            
            foreach (var item in reservationItems)
            {
                item.gameObject.SetActive(false);
                item.transform.SetParent(transform);
            }

            foreach (var item in guestRoomFloors)
            {
                item.gameObject.SetActive(false);
                item.transform.SetParent(transform);
            }
        }

        /// <summary>
        /// Popup 로그인 성공 Task
        /// </summary>
        public void LoginTaskTrigger()
        {
            homePageLoginTaskHandler?.Invoke(TaskHandler.TaskID.App_Connect.ToString(), ((int)Nomanual_PC_OS_Manager.PopupID.Occasion_Hotel_Home_Page_App).ToString());
        }

        /// <summary>
        /// 예약자 정보 세이브 데이터
        /// </summary>
        public string[] SaveReservedCustomer()
        {
            return reservedCustomer.ToArray();
        }

        /// <summary>
        /// 객실 정보 세이브 데이터
        /// </summary>
        public GuestRoom[] SaveGuestRoomData()
        {
            return guestRoomData.Values.ToArray();
        }

        /// <summary>
        /// 객실 정보 불러오기
        /// </summary>
        public void LoadGuestRoomData(GuestRoom[] saveRoomData)
        {
            bool isFirstDay = false;
            
            // 저장된 객실 정보가 없으면 객실 정보 초기화
            if (saveRoomData == null)
            {
                isFirstDay = true;
                List<GuestRoom> _saveRoomData = new List<GuestRoom>();
                for (int floor = 2; floor <= FLOOR_MAX_STACK; floor++)
                for (int roomNumber = 1; roomNumber <= ROOM_MAX_STACK; roomNumber++)
                {
                    int number = floor * 100 + roomNumber;
                    _saveRoomData.Add(new GuestRoom(string.Empty, UnityEngine.Random.Range(0, 4), number, floor, 0));
                }

                saveRoomData = _saveRoomData.ToArray();
            }
            
            // 객실 정보 불러오기
            if (saveRoomData != null)
            {
                Dictionary<int, GuestRoom> guestRooms = new Dictionary<int, GuestRoom>();
                foreach (GuestRoom roomData in saveRoomData)
                {
                    guestRooms[roomData.roomNumber] = roomData;
                }
                guestRoomData = guestRooms;
                // 0 Day 방 상태 업데이트 진행 X
                if(!isFirstDay) AutoUpdateGuestRoomState();
            }
        }
        
        /// <summary>
        /// 예약자 csv 정보 불러오기
        /// </summary>
        public void LoadReservationData()
        {
            string randomReserveKey = LocalizationTable.Reserve_Item_TableTextKey.RandomReserv_.ToString();
            string targetReserveKey = LocalizationTable.Reserve_Item_TableTextKey.TargetReserv_.ToString();
            int[] randomReserveId = new[] { 8000, 8099 };
            int[] targetReserveId = new[] { 8100, 8110 };

            if (_sponsorCustomers.Count != 0) return;

            // 랜덤 예약자 정보
            for (int i = randomReserveId[0]; i <= randomReserveId[1]; i++)
            {
                string key = randomReserveKey + i;
                string reservationName = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.Reserve_Item_Table, key);
                _sponsorCustomers.Add(key, new Customer(i, reservationName)); 
            }

            // 고정 예약자 정보
            for (int i = targetReserveId[0]; i <= targetReserveId[1]; i++)
            {
                string key = targetReserveKey + i;
                string reservationName = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.Reserve_Item_Table, key);
                _sponsorCustomers.Add(key, new Customer(i, reservationName));
            }
        }

        /// <summary>
        /// 예약자 추가하기
        /// </summary>
        public void AddReservationGuest(string customerID)
        {
            // 후원자에 해당 이름이 있으면서 예약된 기록이 없어야함
            if (_sponsorCustomers.ContainsKey(customerID) && !reservedCustomer.Contains(customerID))
            {
                string customerName = _sponsorCustomers[customerID].customerName;
                reservationCustomers.Add(customerName, _sponsorCustomers[customerID]);
                if(HomePage) // 예약 홈페이지 활성화 시 업데이트 필요한 경우
                    HomePage.AddNewReservationCustomer(customerName);
            }
        }

        /// <summary>
        /// 예악자 랜덤 추출 후 추가하기
        /// </summary>
        public void AddRandomReservationGuest()
        {
            /* 1. 랜덤으로 3~5 값을 찾는다. 
            * 2. 랜덤으로 얻은 값만 큼 총 예약자 리스트에서 랜덤으로 뽑는다. 중복으로 뽑지 못한다. (이를 TotalCount라고 부름)
            * 3. 뽑은 에약자중에서 과거 예약자 명단에 들어있으면 해당 예약자들은 제외한다.
            * 4. 제외된 예약자가 있으면 제외된 인원수 값만큼 예약자를 랜덤으로 뽑아서 채운다. (3번 조건을 만족해야함)
            * 5. 총 예약자 리스트 값과 과거 예약자 명단 리스트 값이 5이하 차이나는 경우 랜덤으로 뽑지 않고 나머지 예약자를 모두 뽑는다.
            */
            
            string randomReservationKey = LocalizationTable.Reserve_Item_TableTextKey.RandomReserv_.ToString();
            int totalCount = UnityEngine.Random.Range(3, 6);
            
            // 총 예약자 ID에서 RandomReserv_ 키가 포함된 ID만 필터링
            var allCustomersId = _sponsorCustomers.Keys
                                            .Where(id => id.Contains(randomReservationKey))
                                            .ToList();
            
          
            // 과거 예약자를 제외한 예약자 리스트를 구함
            List<string> available = allCustomersId.Except(reservedCustomer).ToList();

            // 총 예약자와 과거 예약자 명단의 차이가 5초과하면 랜덤으로 예약자를 선택 (5이하이면 남은 예약자 모두 반환)
            if (available.Count > 5)
            {
                available = available.OrderBy(x => UnityEngine.Random.value).Take(totalCount).ToList();
            }

            // 랜덤으로 선정된 예약자 등록
            foreach (var customerId in available)
            {
                AddReservationGuest(customerId);
            }
        }

        /// <summary>
        /// 예약자 데이터 삭제하기
        /// </summary>
        private void RemoveReservationGuest(string customerName, bool completeReservation, UI_ReservationItem reservationItem = null)
        {
            string customerID = string.Empty;
            // 예약자 이름을 기준으로 예약자 ID 얻기
            foreach (var customersData in _sponsorCustomers.Values)
            {
                if (customersData.customerName == customerName)
                {
                    customerID = customersData.customerId.ToString();
                    break; // 첫 번째로 찾은 예약자 고유 ID 값을 얻으면 종료
                }
            }
            
            // 예약자 기록에 추가
            if(customerName != string.Empty && !reservedCustomer.Contains(customerID))
            {
                reservedCustomer.Add(customerID);
                TargetReserveTaskHanlder?.Invoke(TaskHandler.TaskID.Reserve_Target_Clear.ToString(), customerID);
            }
            
            if (reservationItem) reservationItems.Remove(reservationItem);
            reservationCustomers.Remove(customerName);
            
            // 모든 예약자 처리 퀘스트 클리어 
            if (reservationCustomers.Count <= 0) 
                AllClearReserveTaskHanlder?.Invoke(TaskHandler.TaskType.Reserve, TaskHandler.TaskID.Reserve_All_Clear.ToString(), string.Empty);
        }

        /// <summary>
        /// 예약자 UI 삭제하기
        /// </summary>
        public void RemoveReservationCustomer(string customerName, bool dragUI = false)
        {
            UI_ReservationItem removeTarget = null;
            foreach (var reservationData in reservationItems)
            {
                if (reservationData.GetCustomerName().Equals(customerName))
                {
                    removeTarget = reservationData;
                    break;
                }
            }
            if (removeTarget != null)
            {
                removeTarget.gameObject.SetActive(false);
                RemoveReservationGuest(customerName, dragUI, removeTarget);
                if (HomePage)
                {
                    HomePage.UpdateLobbyText(false);
                    if (dragUI) HomePage.RemoveReservationDragDropUI(customerName);
                }
            }
          
        }
        
        /// <summary>
        /// 객실 정보 얻기
        /// </summary>
        public List<GuestRoom> GetGuestRoomData(int floor)
        {
            int maxNumber = floor * 100 + ROOM_MAX_STACK;
            List<GuestRoom> guestRooms = new List<GuestRoom>(ROOM_MAX_STACK);
            for (int i = floor * 100 + 1; i <= maxNumber; i++)
            {
                guestRooms.Add(guestRoomData[i]);
            }
            return guestRooms;
        }

        /// <summary>
        /// Room State에 맞는 객실 정보 얻기
        /// </summary>
        public List<GuestRoom> GetFilteredGuestRoomData(UI_GuestRoomItem.RoomState targetRoomState)
        {
            // 1~2층 모든 객실 정보 가져오기
            Dictionary<int, List<GuestRoom>> allGuestRoomList = new Dictionary<int, List<GuestRoom>>
            {
                {2, GetGuestRoomData(2)},
                {3, GetGuestRoomData(3)},
            };
            
            // Room State에 맞는 객실 필터링
            List<GuestRoom> filteredRooms = new List<GuestRoom>();
            foreach (var floorRooms in allGuestRoomList)
            {
                filteredRooms.AddRange(floorRooms.Value.FindAll(room => room.roomState == (int)targetRoomState));
            }
            return filteredRooms;
        }

        /// <summary>
        /// 모든 객실 상태 업데이트 (하루가 지났다는 의미)
        /// </summary>
        public void AutoUpdateGuestRoomState()
        {
            foreach (var room in guestRoomData.Values)
            {
                AutoUpdateGuestRoomState(room);
            }
        }

        /// <summary>
        /// 객실 자동 업데이트
        /// </summary>
        private void AutoUpdateGuestRoomState(GuestRoom roomData)
        {
            // 1. Stay -> Empty (하루가 지나면)
            // 2. Reservation -> Stay (하루가 지나면)
            // 3. Forbidden -> Empty (하루가 지나면)
            
            var roomState = (UI_GuestRoomItem.RoomState)roomData.roomState;
            string guestName = string.Empty;
            switch (roomState)
            {
                case UI_GuestRoomItem.RoomState.Reservation:
                    roomState = UI_GuestRoomItem.RoomState.Stay;
                    guestName = roomData.geustName;
                    break;
                
                case UI_GuestRoomItem.RoomState.Stay:
                case UI_GuestRoomItem.RoomState.Forbidden:
                    roomState = UI_GuestRoomItem.RoomState.Empty;
                    break;
                default: return;
            }

            GuestRoomStateHandler(roomData, roomState, guestName);
        }

        /// <summary>
        /// 객실 투수객 정보 및 상태 업데이트
        /// </summary>
        public void GuestRoomStateHandler(int roomNumber, UI_GuestRoomItem.RoomState targetState, string guestName = null)
        {
            GuestRoomStateHandler(guestRoomData[roomNumber], targetState, guestName);
        }

        /// <summary>
        /// 객실 투수객 정보 및 상태 업데이트
        /// </summary>
        private void GuestRoomStateHandler(GuestRoom roomData, UI_GuestRoomItem.RoomState targetState, string guestName = null)
        {
            roomData.geustName = guestName;
            roomData.roomState = (int)targetState;
            if(HomePage) HomePage.ChangeGuestRoomUiState(guestRoomItems[roomData.roomNumber], targetState);
        }

        /// <summary>
        /// 객실 상태를 점검으로 변경
        /// </summary>
        public void ForbiddenUpdateGuestRoomState()
        {
            foreach (var room in guestRoomData.Values)
            {
                if ((UI_GuestRoomItem.RoomState)room.roomState is not UI_GuestRoomItem.RoomState.Empty) continue;
                if (UnityEngine.Random.Range(0, 10) < 4) // 40% 확률
                    GuestRoomStateHandler(room, UI_GuestRoomItem.RoomState.Forbidden);
            }
        }
    }
    
    
    /// <summary>
    /// 홈페이지 계정 정보
    /// </summary>
    public class OccasionHotelAccount
    {
        public string id { get; private set; }
        public string pw { get; private set; }
        public OccasionHotelAccount(string id, string pw)
        {
            this.id = id;
            this.pw = pw;
        }

        public bool LoginCheck(string id, string pw)
        {
            return this.id.Equals(id) && this.pw.Equals(pw);
        }
    }

    /// <summary>
    /// 고객 정보
    /// </summary>
    public class Customer : SaveData
    {
        public int customerId { get; private set; }
        public string customerName { get; private set; } // 고객 이름
        public Customer(int customerId,string name)
        {
            this.customerId = customerId;
            this.customerName = name;
        }

        public override void DefaultSettingValue()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 객실 정보
    /// </summary>
    public class GuestRoom : SaveData
    {
        public string geustName; // 예약자 및 투숙객 이름
        public int roomState;
        public int roomNumber;
        public int roomFloor;
        public int roomTire;

        public GuestRoom(string guestName, int roomState, int roomNumber, int roomFloor, int roomTire)
        {
            this.geustName = guestName;
            this.roomState = roomState;
            this.roomNumber = roomNumber;
            this.roomFloor = roomFloor;
            this.roomTire = roomTire;
        }

        public override void DefaultSettingValue()
        {
            throw new NotImplementedException();
        }
    }
}

