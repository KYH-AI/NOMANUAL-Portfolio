using System;
using UnityEngine;
using NoManual.Managers;

/// <summary>
/// 보고서 연출
/// </summary>
public class ReportCutScene : MonoBehaviour
{
    [Header("경비실 시계")]
    [SerializeField] private Animator reportClockAnimator;
    [SerializeField] private GameObject reportCutSceneCam;

    // 현재 라운드 얻기
    private readonly string _ANI_HASH_KEY = "ClearRound";
    private bool _isReportCam = false;
    public static int ClearRound = 0;
    
    
    private void Awake()
    {
        ClearRound = 0;
        reportCutSceneCam.SetActive(false);
    }

    /// <summary>
    /// 경비실 시계 애니메이션 (타임라인 시그널 트리거)
    /// </summary>
    public void PlayAnimatorTimelineSignal()
    {
        reportClockAnimator.SetInteger(_ANI_HASH_KEY, ClearRound);
    }

    /// <summary>
    /// 보고 캠 활성화 (타임라인 시그널 트리거)
    /// </summary>
    public void ActivateReportCam()
    {
        _isReportCam = true;
        
        // 컷신 캠 활성화
        reportCutSceneCam.SetActive(true);
        
        // 플레이어 캠 토글 (3개)
        HFPS.Systems.ScriptManager.Instance.MentatliyCamera.gameObject.SetActive(false);
        HFPS.Systems.ScriptManager.Instance.ArmsCamera.gameObject.SetActive(false);
        HFPS.Systems.ScriptManager.Instance.MainCamera.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 보고 캠 비활성화 (타임라인 시그널 트리거)
    /// </summary>
    public void DeactivateReportCam()
    {
        if (!_isReportCam) return;
        _isReportCam = false;
    
        // 보고 캠 비활성화
        reportCutSceneCam.SetActive(false);
    
        // 플레이어 캠 활성화 (3개)
        HFPS.Systems.ScriptManager.Instance.MentatliyCamera.gameObject.SetActive(true);
        HFPS.Systems.ScriptManager.Instance.ArmsCamera.gameObject.SetActive(true);
        HFPS.Systems.ScriptManager.Instance.MainCamera.gameObject.SetActive(true);
    }
    
    
    /// <summary>
    /// 보고 컷씬 시작 이벤트
    /// </summary>
    public void PlayReportCutSceneStartEvent()
    {
        HotelManager.Instance.IsPlayReportCutScene = false;
        ClearRound = HotelManager.Instance.DayAndRound.CurrentRound;
        PlayAnimatorTimelineSignal();
        Debug.Log("다음 라운드 진행");
        HotelManager.Instance.SetNextRound();
        NoManualHotelManager.Instance.UiNoManualUIManager.FadePanel(true);
        NoManualHotelManager.Instance.CutSceneManager.PlayTimeLine(CutSceneManager.CutSceneType.Report_Production, 
                                                    new Action[]{ReportCutSceneExitEvent}, 
                                                        true);
    }


    /// <summary>
    /// 보고 컷씬 종료 이벤트
    /// </summary>
    private void ReportCutSceneExitEvent()
    {
        NoManualHotelManager.Instance.PlayerStartFade(false, 5f, true, 0.45f);
        DeactivateReportCam();

        // LeftGuideText 출력
        LocalizationTable.CheckList_UI_TableTextKey leftGuideTextKey;
        // 마지막 라운드
        if (ClearRound == DayAndRound.MAX_ROUND)
        {
            leftGuideTextKey = LocalizationTable.CheckList_UI_TableTextKey.Check_List_Exit_Hotel;
            // 마지막 라운드 경우 퇴근 문 활성화
            HotelManager.Instance.ObjectManager.ActiveExitDoor();
        }
        else
        {
            leftGuideTextKey = LocalizationTable.CheckList_UI_TableTextKey.Check_List_Update;
        }
        NoManualHotelManager.Instance.UiNoManualUIManager.RemoveAllLeftTopGuideText();
        NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(leftGuideTextKey);

        if (HotelManager.Instance.DayAndRound.CurrentDay == DayAndRound.MIN_DAY && ClearRound == 2)
        {
            // 경비실을 나가서 순찰을 진행하세요.
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowHintText(
                GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.Check_ListUI_Table, LocalizationTable.CheckList_UI_TableTextKey.Check_List_GoPatrol),
                true,
                1.25f,
                3f
                );
        }
        
        
        /* 미니 독백
       string rndSubtitleKey = HotelManager.Instance.DayAndRound.CurrentRound+"_"+UnityEngine.Random.Range(0, 2);
        var subtitle = NoManualHotelManager.Instance.NpcManager.GetNpcStorySubtitleKey(NPC.Player, LocalizationTable.NPCTableTextKey.Player_Report_Round_ + rndSubtitleKey);
        NoManualHotelManager.Instance.UiNoManualUIManager.ShowSubtitlesText(subtitle);
      */
    }
    
}
