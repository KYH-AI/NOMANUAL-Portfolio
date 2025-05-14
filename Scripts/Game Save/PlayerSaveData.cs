using System;
using Michsky.DreamOS;
using NoManual.Managers;
using NoManual.UI;

/// <summary>
/// 플레이어 세이브 데이터
/// </summary>
[Serializable]
public class PlayerSaveData : SaveData
{
    /*
     세이브 시간
     세이브 파일 이름
     현재 일차
    재화
    메세지
    메일
    음원 갤러리
    사진 갤러리
    메모장 갤러리
      일반 메모장 
      커스텀 메모장
    영상 갤러리
    R.S 트레이더스 배송물품 목록
    객실 정보
        객실 호수, 객실 예약자 이름, 객실 층, 객실 상태
    예약자 정보
         과거 예약 기록
    아이템 인벤토리 
    엔딩 타입
    엔딩 프로세스
    ANO_LINK ID
    이상현상 해결 개수
    정신력
    튜토리얼 클리어
    아이템 퀵슬롯 위치
     */
    
    public string SaveDateTime;
    public int Day;
    public int RS_Cash = 0;
    public MessagingManager.ChatItemData[] Chat_ID= null;
    public MailData[] Mail_ID = null;
    public int[] Picture_ID = null;
    public int[] NotePad_ID = null;
    public NotepadLibrary.NoteItem[] CustomNotePad = null;
    public int[] Video_ID = null;
    public int[] Music_ID = null;
    public RS_TradersDeliveryMapper[] DeliveryItem = null;
    public GuestRoom[] GuestRooms = null;
    public string[] Reserved_ID = null;
    public InventorySaveData[] InventoryItems = null;
    public EndingType EndingType = EndingType.None;
    public ProcessType EndingProcess = ProcessType.None;
    public int[] AnoLink = null;
    public int ClearAnoCount;
    public float Mentatilty;
    public bool IsTutorialClear;
    public ShortCutData[] ShortCutData = null;

    public override void DefaultSettingValue()
    {
        SaveDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        RS_Cash = 0;
        Mail_ID = null;
        Picture_ID = null;
        NotePad_ID = null;
        CustomNotePad = null;
        Video_ID = null;
        GuestRooms = null;
        Reserved_ID = null;
        InventoryItems = null;
        EndingType = EndingType.None;
        EndingProcess = ProcessType.None;
        AnoLink = null;
        ClearAnoCount = 0;
        Mentatilty = NoManual.StateMachine.Mentality.MAX_MENTALITY;
        IsTutorialClear = false;
        ShortCutData = null;
    }
}
