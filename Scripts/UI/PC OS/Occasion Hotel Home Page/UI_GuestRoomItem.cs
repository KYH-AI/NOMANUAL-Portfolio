using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NoManual.UI
{
    public class UI_GuestRoomItem : UI_DropSlot, IPointerEnterHandler, IPointerExitHandler
    {
        public enum RoomState
        {
            None = -1,
            
            Empty = 0,
            Reservation = 1,
            Stay = 2,
            Forbidden = 3,
        }

        [SerializeField] private Image backGroundImage;
        [SerializeField] private TextMeshProUGUI roomNumberText;

        private Color _backGroundOriginalColor;
        private bool _isDragItem = false;
        private UnityAction<string, bool> _reservationSlotRemoveEvent; // 예약자 슬롯 제거 이벤트
        private UnityAction<int, RoomState, string> _roomStateChangeEvent; // 예약자 등록 이벤트
        
        public string guestName { get; private set; } = string.Empty; // 투수객 및 예약자 이름
        public RoomState roomState { get; private set; }
        public int roomNumber { get; private set; }
        public int roomFloor { get; private set; }
        public int roomTire { get; private set; }
        

        /// <summary>
        /// 객실 정보 할당
        /// </summary>
        public void SetGuestRoomData(GuestRoom guestRoom, UnityAction<int, RoomState, string> guestRoomSlotEvent, 
                                    UnityAction<string, bool> reservationSlotRemoveEvent)
        {
            guestName = guestRoom.geustName;
            roomState = (RoomState)guestRoom.roomState;
            roomNumber = guestRoom.roomNumber;
            roomFloor = guestRoom.roomFloor;
            roomTire = guestRoom.roomTire;
            roomNumberText.text = roomNumber.ToString();
            
            _reservationSlotRemoveEvent -= reservationSlotRemoveEvent;
            _reservationSlotRemoveEvent += reservationSlotRemoveEvent;
            _roomStateChangeEvent -= guestRoomSlotEvent;
            _roomStateChangeEvent += guestRoomSlotEvent;
        }

        /// <summary>
        /// 객실 State 변경
        /// </summary>
        public void ChangeRoomState(RoomState newRoomState, Color backGroundColor)
        {
            if (newRoomState is not RoomState.Reservation or RoomState.Stay)
                guestName = string.Empty; // 예약 및 투숙 상태가 아닌경우 이름 지우기
            this.roomState = newRoomState;
            _backGroundOriginalColor = backGroundImage.color = backGroundColor;
        }

        public override void OnDrop(PointerEventData eventData)
        {
            if (roomState is not RoomState.Empty) return;
            GameObject pointerDrag = eventData.pointerDrag;
    
            // IDragItem 인터페이스를 가진 오브젝트만 드래그 Drop 허용
            if (pointerDrag != null && pointerDrag.TryGetComponent(out UI_ReservationItemDragDrop dragItem))
            {
                // 예약으로 변경
                _roomStateChangeEvent?.Invoke(roomNumber, RoomState.Reservation, guestName = dragItem.GetCustomerName());
                // 예약자 명단 제거
                _reservationSlotRemoveEvent?.Invoke(guestName, dragItem);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null &&
                eventData.pointerDrag.TryGetComponent(out UI_ReservationItemDragDrop _))
            {
                _isDragItem = true;
                // 드래그 배경 색상 변경
                switch (roomState)
                {
                    case RoomState.Empty:  
                        backGroundImage.color = Color.green;
                        break;
                    case RoomState.Stay:
                    case RoomState.Forbidden:
                    case RoomState.Reservation:
                        backGroundImage.color = Color.red;
                        break;
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isDragItem) return;
            backGroundImage.color = _backGroundOriginalColor;
            _isDragItem = false;
        }
    }
}
