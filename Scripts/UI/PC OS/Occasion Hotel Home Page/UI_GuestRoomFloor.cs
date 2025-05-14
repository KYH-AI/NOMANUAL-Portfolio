using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace NoManual.UI
{
    public class UI_GuestRoomFloor : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI floorText;
        [SerializeField] private GameObject guestRoomItemPrefab;
        [SerializeField] private Transform guestRoomItemRoot;

        /// <summary>
        /// 객실 설정
        /// </summary>
        public List<UI_GuestRoomItem> SetGuestRoomFloor(int roomStack, int floor, List<GuestRoom> roomsDataList, 
                                                        UnityAction<int, UI_GuestRoomItem.RoomState, string> guestRoomSlotEvent, 
                                                        UnityAction<string, bool> reservationSlotRemoveEvent)
        {
            this.floorText.text = $"{floor}F";
            List<UI_GuestRoomItem> uiGuestRooms = new List<UI_GuestRoomItem>(roomStack);
            
            for (int i = 0; i < roomStack; i++)
            {
               var guestRoom = Instantiate(guestRoomItemPrefab, guestRoomItemRoot);
               UI_GuestRoomItem roomInfo = guestRoom.GetComponent<UI_GuestRoomItem>();
               roomInfo.SetGuestRoomData(roomsDataList[i], guestRoomSlotEvent, reservationSlotRemoveEvent);
               uiGuestRooms.Add(roomInfo);
            }
            return uiGuestRooms;
        }
    }
}


