using System;
using System.Collections.Generic;
using System.Linq;
using Michsky.DreamOS;
using NoManual.Interaction;
using NoManual.Task;
using NoManual.UI;
using UnityEngine;

namespace NoManual.Managers
{
    public class Nomanual_PC_OS_Manager : MonoBehaviour
    {
        
        public enum PopupID
        {
            None = -1,
            PC_OS = 2000,
            Report_App = 2001,
            Mail_App = 2002,
            Message_App = 2003,
            RS_Traders_App = 2004,
            Occasion_Hotel_Home_Page_App = 2005
        }
        

        [Serializable]
        public struct Report
        {
            [Header("보고서")] public UI_Report report;
        }
        
        [Serializable]
        public struct RS_Traders
        {
            [Header("R.S 트레이더스")] public UI_RS_Traders rsTraders;
            [Header("R.S 트레이더스 실시간 상품 업데이트")] public bool runTimeUpdateTraders;
            [Header("R.S 트레이더스 배송 위치")] public Transform rsTradersDeliveryTransform;
            [Header("R.S 트레이더스 배송 위치 하이라이팅")] public HighLightIcon highLightIcon;
        }
        
        [Serializable]
        public struct OccasionHotelHomePage
        {
            [Header("Occasion 호텔 홈페이지")] public OccasionHotelHomePageHelper occasionHotelHomePageHelper;
        }
        
        [Serializable]
        public struct Mail
        {
            [Header("메일 APP")] public MailManager mailApp;
        }
        
        [Serializable]
        public struct Msg
        {
            [Header("메세지")] public MessagingManager msgHelper;
        }
        
        [Serializable]
        public struct Picture
        {
            [Header("사진 갤러리")] public PhotoGalleryManager photoApp;
        }
        
        [Serializable]
        public struct NotePad
        {
            [Header("메모장 갤러리")] public NotepadManager NotepadApp;
        }
        
        [Serializable]
        public struct Video
        {
            [Header("영상 갤러리")] public VideoPlayerManager videoPlayerApp;
        }
        
        [Serializable]
        public struct Music
        {
            [Header("음원 갤러리")] public MusicPlayerManager MusicPlayerApp;
        }


        [Space(20)]
        public Report reportStruct = new Report();
        [Space(20)]
        public RS_Traders rsTradersStruct = new RS_Traders();
        [Space(20)]
        public OccasionHotelHomePage ohHomePage = new OccasionHotelHomePage();
        [Space(20)]
        public Mail mail = new Mail();
        [Space(20)]
        public Msg msg = new Msg();
        [Space(20)] 
        public Picture picture = new Picture();
        [Space(20)] 
        public NotePad notePad = new NotePad();
        [Space(20)] 
        public Video video = new Video();
        [Space(20)] 
        public Music music = new Music();
        
        
        private int testRecordId = -1;
        private int testCustomerId = -1;
        
        #region Task 퀘스트 클리어 이벤트
        
        public event TaskHandler.TaskEventHandler PopUpTaskHandler;

        #endregion

        #region 메일 및 메세지 오브젝트 생성 이벤트

        public static bool IsLogin = false;

        private readonly string _MESSENGER_CSV_PATH = "CSVData/Messenger";
        private readonly string _RS_TRADERS_JSON_PATH = "JsonData/RsTraders/RsTraders_List";
        private Dictionary<int, MessengerCsvParse[]> _messengerCsvParses = new Dictionary<int, MessengerCsvParse[]>();
        //private MessengerCsvParse[] _messengerCsvParses;
        private MailData[] savedMailDataList;
        private MessagingManager.ChatItemData[] savedChatDataList;
        private List<int> newMailIdList = new List<int>();
        private List<int> newChatIdList = new List<int>();
        
        #endregion
        
        private void ReadMessengerCsvFile()
        {
            MessengerCsvParse ParseMessengerCsvFile(string[] values)
            {
                MessengerCsvParse parseData = new MessengerCsvParse(values);
                return parseData;
            }

            var csvHelper = new CSVFileHelper<MessengerCsvParse>(ParseMessengerCsvFile);
            var messengerParsesList = csvHelper.ParseCSV(_MESSENGER_CSV_PATH);
            Dictionary<int, List<MessengerCsvParse>> messengerCsvParses = new Dictionary<int, List<MessengerCsvParse>>();

            foreach (var messengerData in messengerParsesList)
            {
                if (messengerCsvParses.ContainsKey(messengerData.Day))
                {
                    messengerCsvParses[messengerData.Day].Add(messengerData);
                }
                else
                {
                    messengerCsvParses[messengerData.Day] = new List<MessengerCsvParse> { messengerData };
                }
            }
            _messengerCsvParses = messengerCsvParses.ToDictionary(pair => pair.Key, pair => pair.Value.ToArray());
        }
        
        /// <summary>
        /// Task 클리어 트리거 이벤트 등록
        /// </summary>
        public void InitializationTaskEvent(TaskHandler.TaskEventHandler popUpTask, 
                                             TaskHandler.TaskEventHandler tradersBuyTask,
                                             TaskHandler.TaskEventHandler reserveTask,
                                             TaskHandler.ManualTaskEventHandler manualTask)
        {
            // Popup Task 처리
            PopUpTaskHandler -= popUpTask;
            PopUpTaskHandler += popUpTask;
            
            // R.S 트레이더스 물품 구매 
            rsTradersStruct.rsTraders.TradersBuyTaskHandler -= tradersBuyTask;
            rsTradersStruct.rsTraders.TradersBuyTaskHandler += tradersBuyTask;

            // 홈페이지 로그인 처리
            ohHomePage.occasionHotelHomePageHelper.homePageLoginTaskHandler -= PopUpTaskTrigger;
            ohHomePage.occasionHotelHomePageHelper.homePageLoginTaskHandler += PopUpTaskTrigger;
            
            // 특정 예약자 처리
            ohHomePage.occasionHotelHomePageHelper.TargetReserveTaskHanlder -= reserveTask;
            ohHomePage.occasionHotelHomePageHelper.TargetReserveTaskHanlder += reserveTask;
            
            // 모든 예약 처리
            ohHomePage.occasionHotelHomePageHelper.AllClearReserveTaskHanlder -= manualTask;
            ohHomePage.occasionHotelHomePageHelper.AllClearReserveTaskHanlder += manualTask;
            
            // 메일 및 메세지 CSV 파일 읽기
            ReadMessengerCsvFile();
        }

        /// <summary>
        /// App 이벤트 관련 (Task와 무관)
        /// </summary>
        public void InitializationAppEvent(UI_Report.CanReportHandler canReportHandler)
        {
            // Report 보고 이벤트
            reportStruct.report.InitializationReportEvent(canReportHandler);
        }
        
        /// <summary>
        /// 팝업 Task 퀘스트 트리거
        /// </summary>
        public void PopUpTaskTrigger(string taskId, string taskTargetId)
        {
            PopUpTaskHandler?.Invoke(taskId, taskTargetId);
        }

        /// <summary>
        /// PC 로그인 성공 트리거
        /// </summary>
        public void PC_LoginTaskTrigger()
        {
            int pcId = (int)PopupID.PC_OS;
            PopUpTaskTrigger(TaskHandler.TaskID.PC_Connect.ToString(), pcId.ToString());
            
            if (!IsLogin)
            {
                IsLogin = true;
                CreateMailAndMsg();
            }
        }

        #region 보고서
        
        /// <summary>
        /// 보고서 레코드 동적 생성
        /// </summary>
        public void AddRecordItem(string title)
        {
            title += (testRecordId += 1);
            reportStruct.report.CreateRecordItem(title);
        }

        /// <summary>
        /// 보고서 레코드 업데이트 (호텔 맵 전용)
        /// </summary>
        public void UpdateRecordItem_Hotel()
        {
            reportStruct.report.CreateRecordItem_Hotel();
        }

        /// <summary>
        /// 보고서 레코드 업데이트 (인수인계 맵 전용)
        /// </summary>
        public void UpdateRecordItem_Handover()
        {
            reportStruct.report.CreateRecordItem_Handover();
        }

        #endregion

        #region R.S 트레이더스
        
        /// <summary>
        /// R.S 트레이더스 아이템 동적 생성 (테스트용)
        /// </summary>
        public void Add_RS_TradersItem(int itemId)
        {
            rsTradersStruct.rsTraders.CreateNewTradersItem(itemId, rsTradersStruct.runTimeUpdateTraders);
        }

        /// <summary>
        /// R.S 트레이더스 아이템 판매목록 초기화
        /// </summary>
        public void Init_RS_TradersItem(int targetDay)
        {
           var rsTradersItemList = JsonFileHelper.ReadJsonAssetFile<RS_TradersItemList[]>(_RS_TRADERS_JSON_PATH);
           foreach (var rsTradersItem in rsTradersItemList)
           {
               if (rsTradersItem.Days == targetDay)
               {
                   rsTradersStruct.rsTraders.InitTradersItem(rsTradersItem.itemIdList);
                   break;
               }
           }
        }

        /// <summary>
        /// R.S 트레이더스 머니 추가
        /// </summary>
        public void Add_RS_Cash(int cashStack)
        {
            rsTradersStruct.rsTraders.User_RS_Cash += cashStack;
        }

        /// <summary>
        /// R.S 트레이더스 세이브 데이터
        /// </summary>
        public int SaveRsChas()
        {
            return rsTradersStruct.rsTraders.User_RS_Cash;
        }

        /// <summary>
        /// R.S 트레이더스 배송물품 세이브 데이터
        /// </summary>
        public RS_TradersDeliveryMapper[] SaveDeliveryItem()
        {
            return rsTradersStruct.rsTraders.SaveDeliveryItem();
        }

        /// <summary>
        /// R.S 트레이더스 아이템 배송 (일차 호출)
        /// </summary>
        public void Send_RS_TradersDelivery(RS_TradersDeliveryMapper[] deliveryMappers)
        {
            if (deliveryMappers == null || deliveryMappers.Length == 0) return;
            
            // 아이템 동적 생성
            int deliveryCount = deliveryMappers.Length;
            foreach (var itemInfo in deliveryMappers)
            {
                var itemData = NoManualHotelManager.Instance.InventoryManager.GetItemCloneData(itemInfo.itemId);
                for (int i = 0; i < itemInfo.itemStack; i++)
                {
                   var itemObject = Instantiate(itemData.dropPrefab, rsTradersStruct.rsTradersDeliveryTransform);
                   if (itemObject.TryGetComponent(out ItemComponent itemComponent))
                   {
                      itemComponent.InitializedItemComponent(itemData.itemId, 1);
                      itemComponent.IsDeliveryItem = true; // 배송된 아이템 마킹
                   }
                   deliveryCount--;
                }
            }
            
            rsTradersStruct.highLightIcon.SetItemHighLight();
            
            // 이템 배송 실패 시
            if (deliveryCount > 0) Utils.NoManualUtilsHelper.EditorDebugLog(Utils.NoManualUtilsHelper.LogTextColor.red, "R.S 트레이더스 아이템 배송 실패!");
        }
        
        
        #endregion

        #region 호텔 홈페이지
        
        /// <summary>
        /// 호텔 홈페이지 예약자 추가
        /// </summary>
        public void AddReservationItem(string customerName)
        {
            customerName += (testCustomerId += 1);
            ohHomePage.occasionHotelHomePageHelper.AddReservationGuest(customerName);
        }

        /// <summary>
        /// 예약자 랜덤 추가
        /// </summary>
        public void AddRandomReservationItem()
        {
            ohHomePage.occasionHotelHomePageHelper.AddRandomReservationGuest();
        }

        /// <summary>
        /// 특정 예약자 추가
        /// </summary>
        public void AddTargetReservationItem(string customerId)
        {
            ohHomePage.occasionHotelHomePageHelper.AddReservationGuest(LocalizationTable.Reserve_Item_TableTextKey.TargetReserv_.ToString() + customerId);
        }

        /// <summary>
        /// 랜덤으로 객실 상태를 점검으로 변경
        /// </summary>
        public void SetRandomGuestRoomBusyState()
        {
            ohHomePage.occasionHotelHomePageHelper.ForbiddenUpdateGuestRoomState();
        }

        /// <summary>
        /// 모든 객실 상태 업데이트
        /// </summary>
        public void UpdateGuestRoomState()
        {
            ohHomePage.occasionHotelHomePageHelper.AutoUpdateGuestRoomState();
        }
        
        /// <summary>
        /// 객실 정보 추가
        /// </summary>
        public void SetGuestRoomData(GuestRoom[] saveRoomData)
        {
            ohHomePage.occasionHotelHomePageHelper.LoadGuestRoomData(saveRoomData);
        }
        
        /// <summary>
        /// 객실 정보 세이브 데이터
        /// </summary>
        public GuestRoom[] SaveGuestRoomData()
        {
            return ohHomePage.occasionHotelHomePageHelper.SaveGuestRoomData();
        }
        
        /// <summary>
        /// 예약자 정보 추가
        /// </summary>
        public void SetReservedCustomer(string[] reservedId)
        {
            ohHomePage.occasionHotelHomePageHelper.LoadReservationData();
            if (reservedId == null) return;
            ohHomePage.occasionHotelHomePageHelper.reservedCustomer = reservedId.ToList();
        }
        
        /// <summary>
        /// 예약자 정보 세이브 데이터
        /// </summary>
        public string[] SaveReservedCustomer()
        {
            return ohHomePage.occasionHotelHomePageHelper.SaveReservedCustomer();
        }

 
        


        
        #endregion

        #region 메신저 콘텐츠 로컬라이징

        public void MessengerLocalization()
        {
            mail.mailApp.LocalizationMailMapper();
            msg.msgHelper.LocalizationMessageMapper();
            notePad.NotepadApp.LocalizationNotepadMapper();
            video.videoPlayerApp.LocalizationVideoMapper();
            // BETA 출시하고 나머지 로컬라이징 진행
        }

        #endregion
        
        #region 메일 & 메세지 생성

        public void ReadCsvMapper(int day, int round, EndingType currentEndingType)
        {
            // csv 데이터
           // newMailIdList.Clear();
          //  newChatIdList.Clear();
          
            msg.msgHelper.UpdateDynamicMessageReply();
            
            var dataList = _messengerCsvParses[day];
            foreach (var data in dataList)
            {
                if (data.Day.Equals(day) && data.Round.Equals(round)) // && data.EndingType.Equals(currentEndingType) // 베타 끝나고 풀기 (241019)
                {
                    newMailIdList.Add(data.MailId);
                    newChatIdList.Add(data.MessageId);
                }
            }
        }
        
        public void CreateMailAndMsg()
        {
            CreateMailItem();
            CreateMsgItem();
        }

        #endregion
        
        #region 메일

        public void CreateMailItem()
        {
            if(!IsLogin) return;
            
            // 1. 세이브 파일 먼저 불러오기
            if (savedMailDataList != null)
            {
                foreach (var savedMailData in savedMailDataList)
                {
                    mail.mailApp.PrepareMail(savedMailData.mailId, savedMailData.mailCheck);
                }
                savedMailDataList = null;
            }

            
            // 2. CSV 파싱 읽은 후 불러오기
            if (newMailIdList.Count > 0)
            {
                foreach (var newMailId in newMailIdList)
                {
                    if(newMailId == -1) continue;
                    mail.mailApp.PrepareMail(newMailId, false);
                }
                newMailIdList.Clear();
            }
        }
        
        /// <summary>
        /// 메일 추가
        /// </summary>
        public void AddMailItem(int mailId)
        {
            mail.mailApp.PrepareMail(mailId, false);
        }

        /// <summary>
        /// 메일 세이브 데이터
        /// </summary>
        public MailData[] SaveMail()
        {
            return mail.mailApp.SaveMailData();
        }

        /// <summary>
        /// 메일 불러오기
        /// </summary>
        public void LoadMail(MailData[] savedMailData)
        {
            if (savedMailData == null) return;
            savedMailDataList = savedMailData;
        }

        #endregion

        #region 메세지

        public void CreateMsgItem()
        {
            if(!IsLogin) return;

            // 1. 세이브 파일 먼저 불러오기
            if (savedChatDataList != null)
            {
                foreach (var savedMsgData in savedChatDataList)
                {
                    msg.msgHelper.PrepareMessageChatItem(savedMsgData.chatItemId);
                }
                savedChatDataList = null;
            }
        
            
            // 2. CSV 파싱 읽은 후 불러오기
            if (newChatIdList.Count > 0)
            {
                foreach (var newMsgId in newChatIdList)
                {
                    if(newMsgId == -1) continue;

                    if (msg.msgHelper.IsCreateMessageChatItem(newMsgId))
                    {
                        msg.msgHelper.ContinueMessageChatItem(newMsgId, true);
                    }
                    else
                    {
                        msg.msgHelper.PrepareMessageChatItem(newMsgId);
                    }
                }
                newChatIdList.Clear();
            }
            
        }
        
        /// <summary>
        /// 메세지 세이브 데이터
        /// </summary>
        public MessagingManager.ChatItemData[] LocalSaveMessage()
        {
            return msg.msgHelper.LocalSaveMessage();
        }
        
        /// <summary>
        /// 메세지 불러오기 
        /// </summary>
        public void LoadMessage(MessagingManager.ChatItemData[] savedMsgData)
        {
            if (savedMsgData == null) return;

            msg.msgHelper.LoadMessage(savedMsgData);
            savedChatDataList = savedMsgData;
        }
        
        /// <summary>
        /// 메세지 생성
        /// </summary>
        public void AddMsgItem(int msgId)
        {
            msg.msgHelper.PrepareMessageChatItem(3);
        }
        
        /// <summary>
        /// 일반 메세지 이어서 진행
        /// </summary>
        public void ContinueMsg(int msgId)
        {
            msg.msgHelper.ContinueMessageChatItem(3, true);
        }

        #endregion

        #region 사진 갤러리

        /// <summary>
        /// 사진 세이브 데이터
        /// </summary>
        public int[] SavePicture()
        {
            return picture.photoApp.SavePhotoData();
        }

        /// <summary>
        /// 사진 세이브 데이터 불러오기
        /// </summary>
        public void LoadPicture(int[] savedPictureId)
        {
            if (savedPictureId == null) return;
            
            picture.photoApp.LoadPhotoData(savedPictureId);
        }

        #endregion

        #region 메모장 갤러리

        /// <summary>
        /// 기본 메모장 세이브 데이터
        /// </summary>
        public int[] SaveNotePad()
        {
            return notePad.NotepadApp.SaveNotepadData();
        }

        /// <summary>
        /// 기본 메모장 불러오기
        /// </summary>
        public void LoadNotePad(int[] noteId)
        {
            if (noteId == null) return;
            
            notePad.NotepadApp.LoadNotepadData(noteId);
        }
        
        /// <summary>
        /// 커스텀 메모장 세이브 데이터
        /// </summary>
        public NotepadLibrary.NoteItem[] SaveCustomNotePad()
        {
            return notePad.NotepadApp.SaveCustomNotepadData();
        }

        /// <summary>
        /// 커스텀 메모장 불러오기
        /// </summary>
        public void LoadCustomNotePad(NotepadLibrary.NoteItem[] savedCustomNoteData)
        {
            if (savedCustomNoteData == null) return;
        
            notePad.NotepadApp.LoadCustomNotepadData(savedCustomNoteData);
        }

        #endregion

        #region 영상 갤러리

        /// <summary>
        /// 영상 세이브 데이터
        /// </summary>
        public int[] SaveVideo()
        {
            return video.videoPlayerApp.SaveVideo();
        }

        public void LoadVideo(int[] savedVideo)
        {
            if (savedVideo == null) return;
            video.videoPlayerApp.LoadVideo(savedVideo);
        }

        #endregion

        #region 음원 갤러리

        /// <summary>
        /// 음원 세이브 데이터 저장하기
        /// </summary>
        public int[] SaveMusic()
        {
            return music.MusicPlayerApp.SaveMusic();
        }

        /// <summary>
        /// 음원 세이브 데이터 불러오기
        /// </summary>
        public void LoadMusic(int[] savedMusic)
        {
            if (savedMusic == null) return;
            
            music.MusicPlayerApp.LoadMusic(savedMusic);
        }


        #endregion
      
    }

    /// <summary>
    /// 장바구니 아이템 배송 매핑
    /// </summary>
    [System.Serializable]
    public class RS_TradersDeliveryMapper
    {
        public int itemId;
        public int itemStack;
        
        public  RS_TradersDeliveryMapper(int itemId, int itemStack)
        {
            this.itemId = itemId;
            this.itemStack = itemStack;
        }
    }
    
    public class MessengerCsvParse
    {
        public int Day;
        public int Round;
        public int MessageId ;
        public int MailId;
        public EndingType EndingType;

        public MessengerCsvParse(string[] values)
        {
            // Day 변환 (공백은 기본값 0으로 처리)
            if (string.IsNullOrWhiteSpace(values[0]) || !int.TryParse(values[0], out this.Day))
            {
                if (string.IsNullOrWhiteSpace(values[0]))
                {
                    Debug.LogError($"Day 변환 실패: {values[0]}");
                }
            }

            // Round 변환 (공백은 기본값 0으로 처리)
            if (string.IsNullOrWhiteSpace(values[1]) || !int.TryParse(values[1], out this.Round))
            {
                if (!string.IsNullOrWhiteSpace(values[1]))
                {
                    Debug.LogError($"Round 변환 실패: {values[1]}");
                }
            }

            // MessageId 변환 (공백은 기본값 0으로 처리)
            if (string.IsNullOrWhiteSpace(values[2]) || !int.TryParse(values[2], out this.MessageId))
            {
                if (!string.IsNullOrWhiteSpace(values[2]))
                {
                    Debug.LogError($"MessageId 변환 실패: {values[2]}");
                }
                this.MessageId = -1; // 기본값 설정
            }

            // MailId 변환 (공백은 기본값 0으로 처리)
            if (string.IsNullOrWhiteSpace(values[3]) || !int.TryParse(values[3], out this.MailId))
            {
                if (!string.IsNullOrWhiteSpace(values[3]))
                {
                    Debug.LogError($"MailId 변환 실패: {values[3]}");
                }
                this.MailId = -1; // 기본값 설정
            }
            
            if (string.IsNullOrWhiteSpace(values[4]) || !Enum.TryParse(values[4], out EndingType endingType))
            {
                if (!string.IsNullOrWhiteSpace(values[4]))
                {
                    Debug.LogError($"EndingType 변환 실패: {values[4]}");
                }
                this.EndingType = EndingType.None; // 기본값 설정
            }
            else
            {
                this.EndingType = endingType;
            }
        }
    }
}


