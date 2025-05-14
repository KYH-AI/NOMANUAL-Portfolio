public class LocalizationTable 
{
    /// <summary>
    /// CSV 텍스트 테이블 이름
    /// </summary>
    public enum TextTable
    {
        None = -1,
        UI_Table,
        Interaction_Table,
        Hint_Table,
        NPC_Table,
        Mail_Item_Table,
        Photo_Gallery_Item_Table,
        Notepad_Item_Table,
        MusicPlayer_Item_Table,
        VideoPlayer_Item_Table,
        Msg_Item_Table,
        Alarm_Item_Table,
        Manual_Book_Item_Table,
        PC_OS_TradersUI_Table,
        
        // 퀘스트 관련 아이템 테이블
        Standard_Task_Item_Table,
        Bonus_Task_Item_Table,
        Inventory_Item_Table,
        Inventory_Item_Description_Table,
        PC_OS_App_Item_Table,
        Facility_Item_Table,
        Object_Item_Table,
        Reserve_Item_Table,
        
        // 독백 관련 테이블
        Monologue_Table,
        
        // LeftTopGuideText
        Check_ListUI_Table,
    }
    
    
    /// <summary>
    /// 힌트 테이블 텍스트 키
    /// </summary>
    public enum HintTableTextKey
    {
        None = -1,
        Hint_SecurityDoorNeedLock = 0,
        TutorialHint_MouseMove = 1,
        TutorialHint_LookAtNures = 2,
        TutorialHint_HoldBreath = 3,
        TutorialHint_BlinkEye = 4,
        TutorialHint_KeyBoardMove = 5,
        TutorialHint_MedicalRegistrationReceipt = 6,
        TutorialHint_ItemPickUp = 7,
        TutorialHint_Inventory = 8,
        TutorialHint_Combine = 9,
        TutorialHint_Completed = 10,
        
                
        HandoverHint_Next,
        HandoverHint_,
        
    }
    
    /// <summary>
    /// 상호작용 테이블 텍스트 키
    /// </summary>
    public enum InteractionTableTextKey
    {
        None = -1,
        
        UI_Floating_TakeText,
        UI_Floating_StickerText,
        UI_Floating_ReadText,
        UI_Floating_DoorText,
        UI_Floating_UseText,
        UI_Floating_PressText,
        UI_Floating_PutText,
    }
    
    
    /// <summary>
    /// NPC 테이블 텍스트 키
    /// </summary>
    public enum NPCTableTextKey
    {
        None = -1,
        
        // 간호사 대사
        Tutorial_Nures_Hello,
        Tutorial_Nures_HoldBreath,
        Tutorial_Nures_BlinkEye,
        Tutorial_Nures_WellDone,
        
        // 의사 대사
        Tutorial_Doctor_Meet_0,
        Tutorial_Doctor_Meet_1,
        Tutorial_Doctor_Meet_2,
        Tutorial_Doctor_Meet_3,
        Tutorial_Doctor_Meet_4,
        Tutorial_Doctor_Meet_5,
        Tutorial_Doctor_Meet_6,
        Tutorial_Doctor_Meet_7,
        Tutorial_Doctor_Meet_8,
        Tutorial_Doctor_Meet_9,
        
        
        // 플레이어 보고 연출 대사 (사용 X)
        Player_Report_Round_0_0,
        Player_Report_Round_0_1,
        Player_Report_Round_1_0,
        Player_Report_Round_1_1,
        Player_Report_Round_2_0,
        Player_Report_Round_2_1,
        Player_Report_Round_3_0,
        Player_Report_Round_3_1,
        Player_Report_Round_4_0,
        Player_Report_Round_4_1,
        
        // 엔딩 None 플레이어 독백 대사
        Monologue_Text_None_0_0,
        Monologue_Text_None_0_1,
        Monologue_Text_None_0_2,
        Monologue_Text_None_0_3,
        Monologue_Text_None_0_4,
        Monologue_Text_None_0_5,
        Monologue_Text_None_1_0,
        Monologue_Text_None_1_1,
        Monologue_Text_None_1_2,
        Monologue_Text_None_1_3,
        Monologue_Text_None_1_4,
        Monologue_Text_None_1_5,
        Monologue_Text_None_1_6,
        Monologue_Text_None_2_0,
        Monologue_Text_None_2_1,
        Monologue_Text_None_2_2,
        Monologue_Text_None_2_3,
        Monologue_Text_None_2_4,
        Monologue_Text_None_3_0,
        Monologue_Text_None_3_1,
        Monologue_Text_None_3_2,
        Monologue_Text_None_3_3,
        Monologue_Text_None_3_4,
        Monologue_Text_None_3_5,
        Monologue_Text_None_3_6,
        Monologue_Text_None_4_0,
        Monologue_Text_None_4_1,
        Monologue_Text_None_4_2,
        Monologue_Text_None_4_3,
        Monologue_Text_None_4_4,
        Monologue_Text_None_5_0,
        Monologue_Text_None_5_1,
        Monologue_Text_None_5_2,
        Monologue_Text_None_5_3,
        Monologue_Text_None_5_4,
        Monologue_Text_None_5_5,
        Monologue_Text_None_5_6,
        Monologue_Text_None_5_7,
        Monologue_Text_None_5_8,
        Monologue_Text_None_5_9,


        Monologue_Text_A_0_0,
        Monologue_Text_B_0_0,
        Monologue_Text_B_0_1,
        Monologue_Text_B_0_2,
        
        // Task Test 씬 심문관 대사 (사용 X)
        Task_Test_0,
        Task_Test_1,
        Task_Test_2,
        Task_Test_3,
        Task_Test_4,
        Task_Test_5,
        Task_Test_6,
        Task_Test_7,
        Task_Test_8,
        Task_Test_9,
        Task_Test_10,
        Task_Test_11,
        Task_Test_12,
        Task_Test_13,
        Task_Test_14,
        Task_Test_15,

        // 엔딩 None (가미긴 대사) 
        Gamigin_Text_None_0,
        Gamigin_Text_None_1,
        Gamigin_Text_None_2,
        Gamigin_Text_None_3,
        Gamigin_Text_None_4,
        Gamigin_Text_None_5,
        
        // Handover 비디오 대사
        HandoverGuide_0,
        HandoverGuide_1,
        HandoverGuide_2,
        HandoverGuide_3,
        HandoverGuide_4,
        HandoverGuide_5,
        HandoverGuide_6,
        HandoverGuide_7,
        HandoverGuide_8,
        HandoverGuide_9,
        HandoverGuide_10,  // 10번 대사는 사용 X, 삭제 X
        HandoverGuide_11,  // 스텝 2 11번 대사 부터 시작
        HandoverGuide_12,
        HandoverGuide_13,
        HandoverGuide_14,
        HandoverGuide_15,
        HandoverGuide_16,
        HandoverGuide_17,
        HandoverGuide_18,
        HandoverGuide_19,
        HandoverGuide_20,
        HandoverGuide_21,
        HandoverGuide_22,
        HandoverGuide_23,
        HandoverGuide_24,
        HandoverGuide_25,
        HandoverGuide_26,
        HandoverGuide_27,
        HandoverGuide_28,
        HandoverGuide_29,
        HandoverGuide_30,
        HandoverGuide_31,
        HandoverGuide_32,
        HandoverGuide_33,
        HandoverGuide_34,
        HandoverGuide_35,
        HandoverGuide_36,
        HandoverGuide_37,
        HandoverGuide_38,
        HandoverGuide_39,
        HandoverGuide_40,
        HandoverGuide_41,
        HandoverGuide_42,
        HandoverGuide_43,
        HandoverGuide_44,
        HandoverGuide_45,
        HandoverGuide_46,
        HandoverGuide_47,
        HandoverGuide_48,
        HandoverGuide_49,
        HandoverGuide_50,
        HandoverGuide_51,
        HandoverGuide_52,
        HandoverGuide_53,
        HandoverGuide_54,
        HandoverGuide_55,
        HandoverGuide_56,
        HandoverGuide_57,
        HandoverGuide_58,
        HandoverGuide_59,
        HandoverGuide_60,
        HandoverGuide_61,
        HandoverGuide_62,
        HandoverGuide_63,
        
        // 엔딩 B (가미긴 + 플레이어 대사)
        Gamigin_Text_B_0,
        Gamigin_Text_B_1,
        Gamigin_Text_B_2,
        Gamigin_Text_B_3,
        Gamigin_Text_B_4,
        Gamigin_Text_B_5,
        Gamigin_Text_B_6,
        Player_Text_B_0,
        Player_Text_B_1,
        Player_Text_B_2,
        Player_Text_B_3,
        Player_Text_B_4,
        Player_Text_B_5,
        
        // 엔딩 A 독백 (플레이어 대사)
        Monologue_Text_A_0_1,
        Monologue_Text_A_0_2,
        Monologue_Text_A_0_3,
        Monologue_Text_A_0_4,
        Monologue_Text_A_0_5,
        Monologue_Text_A_1_0,
        Monologue_Text_A_1_1,
        Monologue_Text_A_1_2,
        Monologue_Text_A_1_3,
        Monologue_Text_A_1_4,
        Monologue_Text_A_1_5,
        Monologue_Text_A_2_0,
        Monologue_Text_A_2_1,
        Monologue_Text_A_2_2,
        Monologue_Text_A_2_3,
        Monologue_Text_A_2_4,
        Monologue_Text_A_3_0,
        Monologue_Text_A_3_1,
        Monologue_Text_A_3_2,
        Monologue_Text_A_3_3,
        Monologue_Text_A_3_4,
        Monologue_Text_A_3_5,
        Monologue_Text_A_3_6,
        Monologue_Text_A_3_7,
        Monologue_Text_A_4_0,
        Monologue_Text_A_4_1,
        Monologue_Text_A_4_2,
        Monologue_Text_A_4_3,
        Monologue_Text_A_4_4,
        Monologue_Text_A_4_5,
        Monologue_Text_A_4_6,
        Monologue_Text_A_4_7,
        Monologue_Text_A_5_0,
        Monologue_Text_A_5_1,
        Monologue_Text_A_5_2,
        Monologue_Text_A_5_3,
        Monologue_Text_A_5_4,
        Monologue_Text_A_6_0,
        Monologue_Text_A_6_1,
        Monologue_Text_A_6_2,
        Monologue_Text_A_6_3,
        Monologue_Text_A_6_4,
        
        // 엔딩 B 독백 (플레이어 대사)
        Monologue_Text_B_0_3,
        Monologue_Text_B_0_4,
        Monologue_Text_B_1_0,
        Monologue_Text_B_1_1,
        Monologue_Text_B_1_2,
        Monologue_Text_B_1_3,
        Monologue_Text_B_1_4,
        Monologue_Text_B_1_5,
        Monologue_Text_B_2_0,
        Monologue_Text_B_2_1,
        Monologue_Text_B_2_2,
        Monologue_Text_B_2_3,
        Monologue_Text_B_2_4,
        Monologue_Text_B_2_5,
        Monologue_Text_B_2_6,
        Monologue_Text_B_3_0,
        Monologue_Text_B_3_1,
        Monologue_Text_B_3_2,
        Monologue_Text_B_3_3,
        Monologue_Text_B_3_4,
        Monologue_Text_B_3_5,
        Monologue_Text_B_3_6,
        Monologue_Text_B_3_7,
        Monologue_Text_B_3_8,
        Monologue_Text_B_4_0,
        Monologue_Text_B_4_1,
        Monologue_Text_B_4_2,
        Monologue_Text_B_4_3,
        Monologue_Text_B_4_4,
        Monologue_Text_B_5_0,
        Monologue_Text_B_5_1,
        Monologue_Text_B_5_2,
        Monologue_Text_B_5_3,
    }
    
    /// <summary>
    /// UI 텍스트 테이블 키
    /// </summary>
    public enum UITableTextKey
    {
        None = -1,
        
        UI_Inventory_AddItemGuideText,
        UI_Inventory_FullItemGuideText,
        
        UI_Inventory_PutGuideText,
        
        UI_Inventory_CombineGuideText,
        UI_Inventory_CombineSuccessfulText,
        UI_Inventory_CombineFailText,
        
        UI_Inventory_ShortCutGuideText,
    }

    public enum Mail_Item_TableTextKey
    {
        None = -1,
        
        // 메일 아이템 텍스트
        Subject,
        FromName,
        MailContent,
        MusicAttachments,
        NoteAttachments,
        PictureAttachments,
        VideoAttachments,
    }

    public enum Photo_Gallery_Item_TableTextKey
    {
        None = -1,
        
        PhotoTitle,
        PhotoDescription
    }

    public enum Notepad_Item_TableTextKey
    {
        None = -1,
        
        DefaultNotepadTitle,
        DefaultNotepadContent,
        NotepadTitle,
        NotepadContent
    }

    public enum MusicPlayer_Item_TableTextKey
    {
        None = -1,
        
        MusicTitle,
        MusicArtist,
    }

    public enum VideoPlayer_Item_TableTextKey
    {
        None = -1,
        
        VideoTitle,
        VideoDescription,
    }

    public enum Msg_Item_TableTextKey
    {
        None = -1,
        
        MsgCallerFirName,
        MsgCallerSecName,
        ChatMsg,
        ChatDynMsg_PositiveContent,
        ChatDynMsg_NegativeContent,
        ChatDynMsg_ReplyPositiveContent,
        ChatDynMsg_ReplyNegativeContent,
        
        /*
            - chat Msg Id
            = ChatMsg{chat_Item_Id}_{chat_Msg_Id}

            - chat Dynamic Msg Id
            = ChatDynMsg_PositiveContent{chat_Item_Id}_{chat_Dynamic_Msg_Id}
            = ChatDynMsg_NegativeContent{chat_Item_Id}_{chat_Dynamic_Msg_Id}
            = ChatDynMsg_ReplyPositiveContent{chat_Item_Id}_{chat_Dynamic_Msg_Id}
            = ChatDynMsg_ReplyNegativeContent{chat_Item_Id}_{chat_Dynamic_Msg_Id}
         */
    }

    public enum Alarm_Item_TableTextKey
    {
        None = -1,
        
        Mail_DownloadFileComplete,
        New_Mail,
        Receive_Mail,
        OpenAlarm,
        CloseAlarm,
        New_Msg,
        HandoverReservePopUp,
        HandoverReserveDescription,
        HandoverReportPopUp,
        HandoverReportDescription,
    }

    public enum Standard_Task_Item_TableTextKey
    {
        None = -1,
        
        Standard_Task_,
    }

    public enum Inventory_Item_TableTextKey
    {
        None = -1,
        
        Inventory_Item_,
        Inventory_Item_Description_,
    }

    public enum Manual_Book_Item_TableTextKey
    {
        None = -1,
        
        Manual_Book_,
    }
    

    public enum PC_OS_App_Item_TableTextKey
    {
        None = -1,
        
        App_,
    }

    public enum Object_Item_TableTextKey
    {
        None = -1,
        
        Object_,
    }

    // Task Test 씬 전용 (사용 X Legacy)
    public enum Facility_Item_TableTextKey
    {
        None = -1,
        
        Facility_,
    }

    public enum Reserve_Item_TableTextKey
    {
        None = -1,
        
        RandomReserv_, // 랜덤 예약자
        TargetReserv_, // 고정 예약자
    }

    public enum Bonus_Item_TableTextKey
    {
        None = -1,
        
        Bonus_Task_
    }

    public enum CheckList_UI_TableTextKey
    {
        None = -1,
        
        Check_List_Clear,
        Check_List_Update,
        Check_List_Exit_Hotel,
        Check_List_FirstDay,
        Check_List_GoPatrol,
    }

    public enum PC_OS_TradersUI_Table
    {
        None = -1,
        
        Traders_Popup_Text,
        Traders_Popup_Button,
        Traders_BuyPopup_Text,
        Traders_BuyPopup_Button
    }
}
