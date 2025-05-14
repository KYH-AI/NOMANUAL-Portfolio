using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HFPS.Systems;
using NoManual.Inventory;
using NoManual.NPC;
using NoManual.StateMachine;
using NoManual.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace NoManual.Managers
{
    public class UI_NoManualUIManager : MonoBehaviour
    {
        [Header("디버깅 테스트 용")]
        public Debug_TestUI DebugTestUI;
        

        [Serializable]
        public struct UI_Panel
        {
            public GameObject uiMiscPanel;
            public GameObject uiHintNotificationPanel;
            public GameObject uiPaperExaminePanel;
            public GameObject uiFadePanel;
            public GameObject uiPausePanel;
            public GameObject uiOptionPanel;
        }
        
        [Serializable]
        public struct UI_HintNotification
        {
            public Text hintText;
        }
        
        
        [Serializable]
        public struct UI_SFX
        {
            [Header("책 상호작용 효과음")] public AudioClip paperExamine;
        }
        
        [Serializable]
        public struct UI_Subtitle
        {
            [Header("자막 UI Root")] public Transform uiSubtitlePanelRoot;
            [Header("자막 UI BG 이미지")] public Image uiSubtitleBgImage;
            [Header("자막 UI 프리팹")] public GameObject uiSubtitleTextPrefab;
            [Header("Fade 자막 UI")] public GameObject uiFadeSubtitleText;
            [HideInInspector] public Queue<TextMeshProUGUI> uiSubtitleTextPool;
        }
        
        [Serializable]
        public struct UI_Skip
        {
            [Header("스킵 UI 버튼")] 
             public UI_SkipButton uiSkipButton;
        }

        [Serializable]
        public struct UI_LeftTopGuideText
        {
            [Header("가이드 텍스트 프리팹")] public GameObject guideTextPrefab;
            [Header("가이드 텍스트 Root")] public Transform guideTextRoot;
            [Header("가이드 텍스트 SFX")] public AudioClip guideTextSfx;
        }

        [Serializable]
        public struct UI_InventoryGuideText
        {
            [Header("인벤토리 알람 텍스트 프리팹")] public GameObject inventoryGuideTextPrefab;
            [Header("인벤토리 알람 텍스트 Root")] public Transform inventoryGuideTextRoot;
        }
        
        [Serializable]
        public struct UI_PlayerStateHUD
        {
            [Header("플레이어 HUD 캔버스 그룹")] public CanvasGroup playerHUDCanvasGroup;
            [Header("플레이어 정신력 이미지 리소스")] public Sprite[] mentalityResources;
            [Header("플레이어 정신력 이미지 BG")] public Image mentalityBgImage;
            [Header("플레이어 정신력 이미지")] public Image mentalityImage;
            [Header("플레이어 정신력 패널")] public GameObject playerMentalityPanel;
            [Header("플레이어 상태 패널")] public GameObject playerStatePanel;
            [Header("이동 상태")] public Image playerMovementStateSprite;
            [Header("이동 상태 이미지 리소스")] public Sprite[] movementResources;
            [Header("퀵슬롯")] public UI_ShortCutSlot[] shortCutSlots;
        }
        
        [Header("UI Panels")] public UI_Panel uiPanel = new UI_Panel();
        [Header("힌트 UI")] public UI_HintNotification uiHintNotification = new UI_HintNotification();
        [Header("자막 UI")] public UI_Subtitle uiSubtitle = new UI_Subtitle();
        [Header("효과음 UI ")] public UI_SFX uiSfx = new UI_SFX();
        [Header("스킵 UI")] public UI_Skip uiSkip = new UI_Skip();
        [Header("상단 좌측 가이드 UI")] public UI_LeftTopGuideText uiLeftTopGuideText = new UI_LeftTopGuideText();
        [Header("인벤토리 하단 좌측 가이드 UI")] public UI_InventoryGuideText uiInventoryGuideText = new UI_InventoryGuideText();
        [Header("플레이어 상태 UI")] public UI_PlayerStateHUD uiPlayerStateHUD = new UI_PlayerStateHUD();

        [Space(10)]
        [SerializeField] private ManualBookDataBaseScriptable _manualBookDataBase;
        private Dictionary<int, ManualBookCloneData> _manualBookCloneDataList;
        private HFPS.Player.JumpscareEffects _jumpScareEffects;
        private Coroutine _blurEffectCoroutine;
        private bool _isHintTextFadeCoroutineRunning = false;
        // Hint UI Text 코루틴 큐
        private HintFadeTaskHandler _hintFadeTaskHandler = new HintFadeTaskHandler();
        private Stack<UI_PopUpTask> _uiPopUpTaskStack = new Stack<UI_PopUpTask>();
        // 퀵슬롯 하이라이트 UI
        private UI_ShortCutSlot _lastHighLightShortcut;
        // 자막 개수
        private int _subtitleCount = 0;

        private void Awake()
        {
            _jumpScareEffects = ScriptManager.Instance.C<HFPS.Player.JumpscareEffects>();
            // 자막 프리팹 오브젝트 풀링
            uiSubtitle.uiSubtitleTextPool = Utils.ObjectPooling.InitializeQueuePool<TextMeshProUGUI>(3);
            TextMeshProUGUI subtitleText = Utils.ObjectPooling.CreatePrefab(uiSubtitle.uiSubtitleTextPrefab.GetComponent<TextMeshProUGUI>());
            subtitleText.transform.SetParent(uiSubtitle.uiSubtitlePanelRoot);
            Utils.ObjectPooling.AddToQueuePool(uiSubtitle.uiSubtitleTextPool, subtitleText);
            
            // UI 오브젝트 SetActive
            uiSubtitle.uiSubtitlePanelRoot.gameObject.SetActive(true);
            uiSubtitle.uiSubtitleBgImage.enabled = false;
            subtitleText.gameObject.SetActive(false);

            // 매뉴얼 북 데이터 복사
            _manualBookCloneDataList = new Dictionary<int, ManualBookCloneData>(_manualBookDataBase.ManualBookDataBase.Count);
            foreach (var manualBook in _manualBookDataBase.ManualBookDataBase)
            {
                _manualBookCloneDataList.Add(manualBook.manualBookId, new ManualBookCloneData(manualBook.manualBookId, manualBook.requestManualDataType, manualBook.manualBookPrefab));
            }
            
            // Left Top Guide Text 초기화
            foreach (Transform child in uiLeftTopGuideText.guideTextRoot)
            {
                Destroy(child.gameObject);
            }
            
            // 아이템 습득 Guide Text 초기화
            foreach (Transform child in uiInventoryGuideText.inventoryGuideTextRoot )
            {
                Destroy(child.gameObject);
            }

            // 팝업 UI 초기화
            _uiPopUpTaskStack.Clear();
            
            // HUD UI 초기화
            uiPlayerStateHUD.playerHUDCanvasGroup.alpha = 0f;
        }

        private void Start()
        {
            // 옵션 UI 초기화
            uiPanel.uiOptionPanel.GetComponent<UI_Option>().InitializeOptionUI();
        }

        /// <summary>
        /// 런타임 중 String 텍스트 가져오기
        /// </summary>
        public string GetLocalizationText<TTextKey>(LocalizationTable.TextTable textTable, TTextKey textKey) where  TTextKey : System.Enum
        {
            string text = GameManager.Instance.localizationTextManager.GetText(textTable, textKey);
            if (text == string.Empty)
            {
                text = "{" + textTable + " : " + textKey + "}" + "Error";
            }

            return text;
        }


        /// <summary>
        /// Fade 코루틴 체크
        /// </summary>
        private void Update()
        {
            if (_hintFadeTaskHandler.fadeActions.Count > 0)
            {
                 if (!_hintFadeTaskHandler.isFadeActionCoroutineBusy)
                 {
                     _hintFadeTaskHandler.fadeActions.Dequeue()?.Invoke();
                 }
            }
        }

        #region 일시정지 UI

        /// <summary>
        /// 일시정지 관련 UI 활성화
        /// </summary>
        public void ShowPausePanel()
        {
            uiPanel.uiOptionPanel.SetActive(false);
            uiPanel.uiPausePanel.transform.GetChild(0).gameObject.SetActive(true);
            uiPanel.uiPausePanel.SetActive(true);
        }

        /// <summary>
        /// 일시정지 관련 UI 비활성화
        /// </summary>
        public void HidePausePanel()
        {
            uiPanel.uiOptionPanel.SetActive(false);
            uiPanel.uiPausePanel.transform.GetChild(0).gameObject.SetActive(false);
            uiPanel.uiPausePanel.SetActive(false);
        }

        public void EscKeyInputUiHandler(UnityAction enableActions, UnityAction disableActions)
        {
            PopUpHandler(null, true, enableActions, disableActions);
        }
        
        #endregion

        #region 자막
        
        /// <summary>
        /// NPC 자막 텍스트 UI 출력 & 자막 음성 출력
        /// </summary>
        public void ShowSubtitlesText(string subtitleText, LocalizationTable.NPCTableTextKey subtitles, float subtitleTextDuration = 0f)
        {
            // 자막 오디오 가져오기
            AudioClip subtitlesClip = NoManualHotelManager.Instance.AudioManager.GetAudioClip(subtitles);
            ShowSubtitlesText(subtitleText, subtitlesClip, subtitleTextDuration);
        }
        
        /// <summary>
        /// NPC 자막 텍스트 UI 출력 & 자막 음성 출력
        /// </summary>
        public void ShowSubtitlesText(TextScriptArray textScriptArrays)
        {
            StartCoroutine(SubtitleTimer(textScriptArrays.TextScriptList, textScriptArrays.JumpOffset));
        }

        /// <summary>
        /// NPC 자막 리스트 묶음 처리
        /// </summary>
        private IEnumerator SubtitleTimer(TextScript[] textScripts, float jumpOffset)
        {
            // 자막 배열의 길이가 1 이하인 경우, 대기 시간을 적용하지 않음
            if (textScripts.Length > 1)
            {
           
                foreach (var textScript in textScripts)
                {
                    ShowSubtitlesText(GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.NPC_Table, textScript.TextKey), 
                        textScript.VoiceClip, 
                        textScript.Offset, 
                        textScript.Voice3dAudioSource);
                    yield return new WaitForSeconds(jumpOffset + textScript.VoiceClip.length + textScript.Offset);
                }
            }
            else
            {
                // 배열에 자막이 1개 이하일 경우 즉시 실행
                foreach (var textScript in textScripts)
                {
                    ShowSubtitlesText(GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.NPC_Table, textScript.TextKey), 
                        textScript.VoiceClip, 
                        textScript.Offset, 
                        textScript.Voice3dAudioSource);
                }
            }
        }
        
        /// <summary>
        /// NPC 자막 텍스트 UI 출력 & 자막 음성 출력
        /// </summary>
        private void ShowSubtitlesText(string subtitleText, AudioClip voiceClip, float subtitleTextDuration = 0f, AudioSource audioSource3D = null)
        {
            // 자막 지속시간 설정
            subtitleTextDuration += voiceClip.length;
            // 풀 얻기
            TextMeshProUGUI subtitleTextObject = Utils.ObjectPooling.GetFromQueuePool(uiSubtitle.uiSubtitleTextPool);
            if (!subtitleTextObject)
            {
                subtitleTextObject = Utils.ObjectPooling.CreatePrefab(uiSubtitle.uiSubtitleTextPrefab.GetComponent<TextMeshProUGUI>());
                subtitleTextObject.transform.SetParent(uiSubtitle.uiSubtitlePanelRoot);
            }
            if (!uiSubtitle.uiSubtitleBgImage.enabled) uiSubtitle.uiSubtitleBgImage.enabled = true;
            subtitleTextObject.text = subtitleText;
            subtitleTextObject.gameObject.SetActive(true);
            if(audioSource3D) NoManualHotelManager.Instance.AudioManager.PlaySubtitle3dSFX(audioSource3D, voiceClip);
            else  NoManualHotelManager.Instance.AudioManager.PlaySubtitleSFX(voiceClip, true);
               
            _subtitleCount++;
            StartCoroutine(SubtitleTextUICoroutine(subtitleTextObject, subtitleTextDuration));
        }
        
        /// <summary>
        /// NPC 자막 텍스트 UI 출력 & 자막 음성 출력
        /// </summary>
        public void ShowSubtitlesText(NPC_DataScriptable.NpcSubTitlesData npcSubTitlesData)
        {
            string text = GetLocalizationText(LocalizationTable.TextTable.NPC_Table, npcSubTitlesData.NpcSubtitlesTextKey);
            ShowSubtitlesText(text, npcSubTitlesData.NpcSubtitlesTextKey, npcSubTitlesData.subtitlesOffset);
        }

        /// <summary>
        /// NPC 자막 텍스트 닫기
        /// </summary>
        public void HideSubtitlesText(TextMeshProUGUI subtitleText)
        {
           // uiSubtitle.uiSubtitlePanelRoot.gameObject.SetActive(false);
            subtitleText.text = string.Empty;
            subtitleText.gameObject.SetActive(false);
        }

        private IEnumerator SubtitleTextUICoroutine(TextMeshProUGUI subtitleText, float waitTime)
        {
            float timer = 0f;
            while (timer < waitTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            // 자막이 1개 이하 경우 배경색 비활성화
            if (_subtitleCount <= 1)
            {
                uiSubtitle.uiSubtitleBgImage.enabled = false;
            }

            _subtitleCount = Mathf.Max(_subtitleCount - 1, 0);
            // 자막 텍스트 닫기
            HideSubtitlesText(subtitleText);
            // 풀 반납
            Utils.ObjectPooling.ReturnToQueuePool(uiSubtitle.uiSubtitleTextPool, subtitleText);
        }

        #endregion

        #region 상단좌측 가이드 텍스트

        /// <summary>
        /// 상단좌측 가이드 텍스트 알람출력
        /// </summary>
        public void ShowLeftTopGuideText(LocalizationTable.CheckList_UI_TableTextKey textKey)
        {
            ShowLeftTopGuideText(string.Empty, textKey);
        }
        
        /// <summary>
        /// 상단좌측 가이드 텍스트 알람출력 (텍스트 조합)
        /// </summary>
        public void ShowLeftTopGuideText(string guideText, LocalizationTable.CheckList_UI_TableTextKey textKey)
        {
            NoManualHotelManager.Instance.AudioManager.PlaySFX(AudioManager.SFX_Audio_List.Etc, true, uiLeftTopGuideText.guideTextSfx);
            string text = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.Check_ListUI_Table, textKey);
            guideText = guideText + text;
            Instantiate(uiLeftTopGuideText.guideTextPrefab, uiLeftTopGuideText.guideTextRoot).GetComponent<UI_GuideText>().StartFade(guideText);
        }
        
        /// <summary>
        /// 상단좌측 가이드 텍스트 알람출력 2
        /// </summary>
        public void ShowLeftTopGuideText(LocalizationTable.TextTable textTable, string textKey)
        {
            ShowLeftTopGuideText(string.Empty, textTable, textKey);
        }
        
        /// <summary>
        /// 상단좌측 가이드 텍스트 알람출력 (텍스트 조합) 2
        /// </summary>
        public void ShowLeftTopGuideText(string guideText, LocalizationTable.TextTable textTable, string textKey)
        {
            NoManualHotelManager.Instance.AudioManager.PlaySFX(AudioManager.SFX_Audio_List.Etc, true, uiLeftTopGuideText.guideTextSfx);
            string text = GameManager.Instance.localizationTextManager.GetText(textTable, textKey);
            guideText = guideText + text;
            Instantiate(uiLeftTopGuideText.guideTextPrefab, uiLeftTopGuideText.guideTextRoot).GetComponent<UI_GuideText>().StartFade(guideText);
        }

        /// <summary>
        /// 상단좌측 가이드 텍스트 모두 제거
        /// </summary>
        public void RemoveAllLeftTopGuideText()
        {
            foreach (Transform child in uiLeftTopGuideText.guideTextRoot)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// 상단좌측 가이드 딜레이 후 출력
        /// </summary>
        public void DelayShowLeftTopGuideText(LocalizationTable.CheckList_UI_TableTextKey textKey, float delay)
        {
            StartCoroutine(DelayTimerEvent(() => ShowLeftTopGuideText(textKey), delay));
        }
        
        private IEnumerator DelayTimerEvent(Action eventAction, float delayTimer)
        {
            if (eventAction == null) yield break;
            yield return new WaitForSeconds(delayTimer);
            eventAction?.Invoke();
        }

        #endregion

        #region 근무 목록

        /// <summary>
        /// 근무목록 리스트 UI 관련 데이터 얻기 (호텔 or 인수인계 맵)
        /// </summary>
        private UI_CheckList.CheckListUiDataMapper[] GetCheckListData(bool isHotel = true)
        { 
            /* 근무목록 정보는 HotelManager에서 정보를 가져와야함    */
            List<UI_CheckList.CheckListUiDataMapper> checkListUiDataMapperArray = new List<UI_CheckList.CheckListUiDataMapper>();
            
            /* 기본근무 목록 */
            List<Task.TaskHandler.StandardTask> standardTaskItems = isHotel ? 
                                                                    HotelManager.Instance.GetRoundStandardTask() : 
                                                                    Tutorial.HandoverManager.Instance.TaskHandler.GetRoundStandardTask(1);
            if (standardTaskItems != null && standardTaskItems.Count >= 1)
            {
                foreach (var stdTaskItem in standardTaskItems)
                {
                    checkListUiDataMapperArray.Add(new UI_CheckList.CheckListUiDataMapper(stdTaskItem.taskDescription, false, stdTaskItem.isClear));
                }
            }
            
            /* 추가근무 목록 */
            List<Task.TaskHandler.BonusTask> bonusTaskItems = isHotel ?  
                                                              HotelManager.Instance.GetRoundBonusTask() :
                                                              Tutorial.HandoverManager.Instance.TaskHandler.GetBonusTask(1);
                    
            if (bonusTaskItems != null && bonusTaskItems.Count >= 1)
            {
                foreach (var bonusTaskItem in bonusTaskItems)
                {
                    // 클리어된 경우만 할당
                    if(!bonusTaskItem.isClear) continue;
                    checkListUiDataMapperArray.Add(new UI_CheckList.CheckListUiDataMapper(bonusTaskItem.taskDescription, true, bonusTaskItem.isClear));
                }
            }

            var listDataMapper = checkListUiDataMapperArray.ToArray();
            return listDataMapper.Length == 0 ? null : listDataMapper;
        }
        

        #endregion

        #region 책 관련 UI

        /// <summary>
        /// 책 관려 UI 오브젝트 생성호출
        /// </summary>
        public void CreatPaperUI(int paperId)
        {
            ManualBookCloneData manualBookData = _manualBookCloneDataList[paperId];
            ShowPaperExamine(manualBookData);
        }
        
        /// <summary>
        /// 책 알림 UI 출력
        /// </summary>
        private void ShowPaperExamine(ManualBookCloneData manualBook)
        {
             // 프리팹 준비
             GameObject paperUi = manualBook.manualBookUiPrefab;
             uiPanel.uiPaperExaminePanel.SetActive(false);
             
             // 책 펼치는 효과음
             NoManualHotelManager.Instance.AudioManager.PlaySFX(AudioManager.SFX_Audio_List.Etc, true, uiSfx.paperExamine);
             
             // 프리팹 동적 생성 
             paperUi = Instantiate(paperUi, uiPanel.uiPaperExaminePanel.transform);
                   
             // 데이터 매핑
             ManualBookDataMapper(manualBook.requestManualDataType, paperUi);

             // 팝업 Stack 컨트롤
             PopUpHandler(uiPanel.uiPaperExaminePanel, 
                 false,  
                 () =>
                 {
                     HFPS_GameManager.Instance.TabOrEscLockMode(true);
                 }, 
                 () =>
                 {
                     HidePaperExamine();
                     HFPS_GameManager.Instance.TabOrEscLockMode(false);
                 });
        }

        /// <summary>
        /// 책 알림 UI 닫기
        /// </summary>
        private void HidePaperExamine()
        {
             //동적 파괴
            foreach (Transform paperUi in uiPanel.uiPaperExaminePanel.transform)
            {
                Destroy(paperUi.gameObject);
            }
            
            // 종이 효과음 재생
            NoManualHotelManager.Instance.AudioManager.PlaySFX(AudioManager.SFX_Audio_List.Etc, true, uiSfx.paperExamine);
        }

        /// <summary>
        /// 읽기 관련 데이터 UI 매핑
        /// </summary>
        private void ManualBookDataMapper(RequestManualDataType dataType, GameObject paperUi)
        {
            if (dataType == RequestManualDataType.None) return;
            
            switch (dataType)
            {
                case RequestManualDataType.CheckList : 
                    paperUi.GetComponent<UI_CheckList>().CheckListInitialization(GetCheckListData());
                    break;
                
                case RequestManualDataType.HandoverCheckList :
                    paperUi.GetComponent<UI_CheckList>().CheckListInitialization(GetCheckListData(false), false);
                    break;
            }

        }
        

        #endregion

        #region 중앙상단 힌트 UI
        
        /// <summary>
        /// 힌트 알림 UI 출력
        /// </summary>
        public void ShowHintText(string hintText, bool isPingPong, float fadeSpeed, float fadeDuration)
        {
            // 이미 힌트 페이드가 진행 또는 텍스트 내용이 없는경우 생략
            if (_isHintTextFadeCoroutineRunning || hintText == string.Empty) return;

            TextType textType = TextType.Hint;
            uiHintNotification.hintText.text = hintText;// NoManualHotelManager.Instance.TextDataBase.GetTextDataValue(textType, hintText);
            
            List<UnityAction> events = new List<UnityAction>()
            {
               () => uiHintNotification.hintText.gameObject.SetActive(true),
               () => uiPanel.uiHintNotificationPanel.SetActive(true),
            };
            
           Action fadeAction = () => StartCoroutine(AlphaHintFadeCoroutine(uiPanel.uiHintNotificationPanel.GetComponent<CanvasGroup>(), _hintFadeTaskHandler,
                                                isPingPong, 
                                        0f, 1f, fadeSpeed, fadeDuration, 
                                                 events,
                                                  isPingPong ? new List<UnityAction> { HideHintText } : null));

           HintFadeActionTask(fadeAction, "showHintText : " + hintText);
        }

        /// <summary>
        /// 힌트 알림 UI 닫기
        /// </summary>
        public void HideHintText()
        {
            uiHintNotification.hintText.text = string.Empty;
            
            uiHintNotification.hintText.gameObject.SetActive(false);
            uiPanel.uiHintNotificationPanel.SetActive(false);
        }
        
        /// <summary>
        /// 힌트 알림 UI 닫기 (fade 버전)
        /// </summary>
        public void HideHintText(float fadeSpeed)
        {
            Action fadeAction = () => StartCoroutine(AlphaHintFadeCoroutine(uiPanel.uiHintNotificationPanel.GetComponent<CanvasGroup>(), _hintFadeTaskHandler,
                false,
                1f, 0f, fadeSpeed, 0f,
                null,
                 new List<UnityAction> { HideHintText }));
            
            HintFadeActionTask(fadeAction, "hideHintText");
        }

        /// <summary>
        /// Hint Fade 코루틴 Task 실행
        /// </summary>
        private void HintFadeActionTask(Action fadeTask, string debug)
        {
            _hintFadeTaskHandler.FadeActionTaskHandle(fadeTask, debug);
        }

        
        /// <summary>
        /// Hint 텍스트 UI Alpha Fade 코루틴
        /// </summary>
        /// <param name="targetCanvasGroup">Fade 캔버스 그룹</param>
        /// <param name="fadeHandler">Fade 코루틴 큐</param>
        /// <param name="isPingPong">Fade 반대효과</param>
        /// <param name="startAlphaValue">시작 알파 값</param>
        /// <param name="targetAlphaValue">최종 알파 값</param>
        /// <param name="fadeSpeed">Fade 전환시간</param>
        /// <param name="duration">Fade 지속시간</param>
        /// <param name="startEvent">Fade 시작 이벤트</param>
        /// <param name="endEvent">Fade 종료 이벤트</param>
        private IEnumerator AlphaHintFadeCoroutine(CanvasGroup targetCanvasGroup, FadeActionTask fadeHandler,
                                            bool isPingPong,
                                            float startAlphaValue, 
                                            float targetAlphaValue, 
                                            float fadeSpeed,
                                            float duration, 
                                            List<UnityAction> startEvent, 
                                            List<UnityAction> endEvent)
        {
            // 코루틴 시작 알림
            if (fadeHandler != null)
            {
                fadeHandler.isFadeActionCoroutineBusy = true;
            }
            
            // 코루틴 시작 시 호출할 이벤트 실행
            if (startEvent != null)
            {
                foreach (UnityAction action in startEvent)
                {
                    action?.Invoke();
                }
            }
            targetCanvasGroup.alpha = startAlphaValue;
            
            // 페이드 시작
            float currentTime = 0f;
            while (currentTime <= fadeSpeed)
            {
                float alpha = Mathf.Lerp(startAlphaValue, targetAlphaValue, currentTime / fadeSpeed);
                targetCanvasGroup.alpha = alpha;

                currentTime += Time.deltaTime;
                yield return null;
            }

            // 페이드 Text 지속시간
            if (duration > 0f)
            {
                currentTime = 0f;
                while (currentTime <= duration)
                {
                    currentTime += Time.deltaTime;
                    yield return null;
                }
            }
            
            // 페이드 Text 지속시간
            /*
            if (!isPingPong && duration > 0f)
            {
                currentTime = 0f;
                while (currentTime <= duration)
                {
                    currentTime += Time.deltaTime;
                    yield return null;
                }
            }
            */
 
            // 코루틴 종료 시 호출할 이벤트 실행
            if (!isPingPong && endEvent != null)
            {
                foreach (UnityAction action in endEvent)
                {
                    action?.Invoke();
                }
            }

            // 페이드 반대로
            if (isPingPong)
            {
                StartCoroutine(AlphaHintFadeCoroutine(targetCanvasGroup, fadeHandler,
                    false,
                    targetAlphaValue,
                    startAlphaValue,
                    fadeSpeed,
                    0f,
                    null,
                    endEvent));
            }
            else
            {
                // 코루틴 종료 알림
                if (fadeHandler != null)
                {
                    fadeHandler.isFadeActionCoroutineBusy = false;
                }
            }
        }
        
                

        #endregion
        
        #region Blur

        /// <summary>
        /// ESC키  Blur Effect 활성화 및 비활성화
        /// </summary>
        public void BlurEffect(bool isEnable)
        {
            if (_blurEffectCoroutine != null)
            {
                StopCoroutine(_blurEffectCoroutine);
                _blurEffectCoroutine = null;
            }
            
            _blurEffectCoroutine = StartCoroutine(BlurEffectCoroutine(isEnable));
        }
        
        /// <summary>
        /// Blur Effect 코루틴
        /// </summary>
        private IEnumerator BlurEffectCoroutine(bool isEnable)
        {
            while (!_jumpScareEffects.BlurEffect(isEnable))
            {
                yield return null;
            }
        }
        
        #endregion

        #region 페이드

        /// <summary>
        /// 인게임 Fade In, Out
        /// </summary>
        public void FadePanel(bool fadeIn, float fadeDuration, List<UnityAction> fadeEvent = null)
        {
            Image fadeImage = uiPanel.uiFadePanel.GetComponent<Image>();
            uiPanel.uiFadePanel.SetActive(true);

            if (fadeIn)
            {
                DOTweenManager.FadeInImage(fadeImage, fadeDuration, fadeEvent);
            }
            else
            {
                if (fadeEvent != null)
                {
                    fadeEvent.Add(() => uiPanel.uiFadePanel.SetActive(false));
                }
                else
                {
                    fadeEvent = new List<UnityAction>()
                    {
                        () =>  uiPanel.uiFadePanel.SetActive(false),
                    };
                }
                DOTweenManager.FadeOutImage(fadeImage, fadeDuration, fadeEvent);
            }
        }

        /// <summary>
        /// 인게임 Fade In, Out 즉석설정
        /// </summary>
        public void FadePanel(bool fadeIn)
        {
            Image fadeImage = uiPanel.uiFadePanel.GetComponent<Image>();
            uiPanel.uiFadePanel.SetActive(fadeIn);
            // fadeIn이 true면 알파 값을 1로, false면 0으로 즉시 설정
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeIn ? 1f : 0f);
        }

        /// <summary>
        /// 인게임 Fade 자막 UI 활성화 및 비활성화
        /// </summary>
        public void FadePanelHandleSubtitleText(bool isShow)
        {
            uiSubtitle.uiFadeSubtitleText.SetActive(isShow);
        }

        #endregion

        #region UI 팝업 스택

        public void PopUpHandler(GameObject targetUI, bool isInputTapKey, UnityAction enableAction = null, UnityAction disableAction = null)
        {
            if (IsEmptyPopUpStack())
            {
                AddPopUpStack(targetUI, enableAction, disableAction);
                UI_PopUpTask popUpTask = PeekPopUpStack();
                if(popUpTask != null) PopUpTaskHandler(popUpTask, true);
            }
            else
            {
                UI_PopUpTask popUpTask;
                if (isInputTapKey)
                {
                    RemovePopUpStack();
                }
                else
                {
                    popUpTask = PeekPopUpStack();
                    if(popUpTask != null) PopUpTaskHandler(popUpTask, false);
                    AddPopUpStack(targetUI, enableAction, disableAction);
                }
                popUpTask = PeekPopUpStack();
                if(popUpTask != null) PopUpTaskHandler(popUpTask, true);
            }
        }

        /// <summary>
        /// 팝업 UI Stack 확인
        /// </summary>
        public bool IsEmptyPopUpStack()
        {
            return PeekPopUpStack() == null;
        }

        /// <summary>
        /// 팝업 UI Task Stack 추가
        /// </summary>
        /// <param name="target"></param>
        /// <param name="eventAction"></param>
        private void AddPopUpStack(GameObject target, UnityAction enableAction = null, UnityAction disableAciton = null)
        {
            UI_PopUpTask newPopUpStack = new UI_PopUpTask(target, enableAction, disableAciton);
            _uiPopUpTaskStack.Push(newPopUpStack);
        }

        /// <summary>
        /// 팝업 UI Task Peek
        /// </summary>
        private UI_PopUpTask PeekPopUpStack()
        {
            UI_PopUpTask popUpTask = null;
            
            if (_uiPopUpTaskStack.Count != 0)
            {
                return _uiPopUpTaskStack.Peek();
            }
            return popUpTask;
        }

        /// <summary>
        /// 팝업 UI 비활성화 후 Stack 제거
        /// </summary>
        private void RemovePopUpStack()
        {
            if (_uiPopUpTaskStack.Count != 0)
            {
                UI_PopUpTask popUpTask = _uiPopUpTaskStack.Pop();
                if (popUpTask != null)
                {
                    PopUpTaskHandler(popUpTask, false);
                }
            }
        }

        /// <summary>
        /// 팝업 UI 처리
        /// </summary>
        private void PopUpTaskHandler(UI_PopUpTask popUpTask, bool isShow)
        {
            if(popUpTask.targetUI)  popUpTask.targetUI.SetActive(isShow);
            if(isShow) popUpTask.enableAction?.Invoke();
            else popUpTask.disableAction?.Invoke();
        }

        #endregion

        
        #region 인벤토리 UI
        

        /// <summary>
        /// 인벤토리 UI 호출
        /// </summary>
        public void InventoryUiHandler()
        {
            PopUpHandler(HFPS_GameManager.Instance.gamePanels.TabButtonPanel, true, 
            () =>
            { 
                NoManualHotelManager.Instance.InventoryManager.ResetInventory();
                HFPS_GameManager.Instance.TabOrEscLockMode(true);
            },
            () =>
            {
               HFPS_GameManager.Instance.TabOrEscLockMode(false);
            });
        }
        

        /// <summary>
        /// 인벤토리 UI 강제 닫기 (해당 함수는 인벤토리를 열었는 경우가 확실하니 바로 Stack에서 사용함)
        /// </summary>
        public void CloseInventoryUI()
        {
            RemovePopUpStack();
        }

        /// <summary>
        /// 하단 좌측 아이템 습득 텍스트 알람호출
        /// </summary>
        public void ShowInventoryGuideText(LocalizationTable.UITableTextKey textKey)
        {
            ShowInventoryGuideText(GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.UI_Table, textKey));
        }

        /// <summary>
        /// 하단 좌측 아이템 습득 텍스트 알람호출
        /// </summary>
        public void ShowInventoryGuideText(string itemName, int itemAmount)
        {
            string text = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.UI_Table, LocalizationTable.UITableTextKey.UI_Inventory_AddItemGuideText);
            ShowInventoryGuideText(itemName + "(" + itemAmount + ") " + text);
        }

        private void ShowInventoryGuideText(string guideText)
        {
            Instantiate(uiInventoryGuideText.inventoryGuideTextPrefab, uiInventoryGuideText.inventoryGuideTextRoot).GetComponent<UI_GuideText>().StartFade(guideText);
        }

        #endregion

    #region 플레이어 HUD

        /// <summary>
        /// 플레이어 HUD 알파 값 활성화
        /// </summary>
        private void ShowPlayerHUDPanel()
        {
            uiPlayerStateHUD.playerHUDCanvasGroup.alpha = 1f;
        }

        /// <summary>
        /// 플레이어 HUD 알파 값 Fade Out
        /// </summary>
        private void HidePlayerHUDPanel()
        {
            uiPlayerStateHUD.playerHUDCanvasGroup.alpha = 1f;
            uiPlayerStateHUD.playerHUDCanvasGroup.DOKill(false);
            uiPlayerStateHUD.playerHUDCanvasGroup.DOFade(0f, 5f);
        }
    
        #region 플레이어 이동 상태 UI (뭔가 디버깅용?)

        public void ShowPlayerMovementState(StateMachine.PlayerMovementState newMovementState)
        {
            // 플레이어 상태 패널 활성화
            uiPlayerStateHUD.playerStatePanel.SetActive(true);
            Image moveState  = uiPlayerStateHUD.playerMovementStateSprite;
            moveState.sprite = uiPlayerStateHUD.movementResources[(int)newMovementState];

            // 닷트윈 초기화
            moveState.DOKill(false);
                
            // 이미지의 초기 알파 값을 1로 설정
            moveState.DOFade(1f, 1f).OnComplete(()=>
            {
                // 알파 값이 1이 된 후, 1초 뒤에 알파 값을 0으로 설정
                moveState.DOFade(0f, 1f);
            });
        }

        #endregion

        #region 플레이어 정신력 상태 UI

        /// <summary>
        /// 플레이어 정신력 HUD
        /// </summary>
        public void ShowPlayerMentalityState(float value)
        {
            // 플레이어 상태 패널 활성화
            uiPlayerStateHUD.playerMentalityPanel.SetActive(true);
            
            // 정신력 이미지 FillAmount 
            uiPlayerStateHUD.mentalityImage.fillAmount = 1 - value;
            if (value <= 0f)
            {
                // 사망 이미지로 교체
                uiPlayerStateHUD.mentalityBgImage.sprite = uiPlayerStateHUD.mentalityResources[1];
                uiPlayerStateHUD.mentalityImage.enabled = false;
            }
            
            NoManualHotelManager.Instance.InventoryManager.UpdateMentalityUI(uiPlayerStateHUD.mentalityImage.fillAmount,  uiPlayerStateHUD.mentalityBgImage.sprite);
            HidePlayerHUDPanel();
        }


        #endregion

        #region 퀵슬롯 UI

        public void ShortCutSlotUiInit()
        {
            foreach (var shortCut in uiPlayerStateHUD.shortCutSlots) shortCut.Init();
        }
        
        /// <summary>
        /// 퀵슬롯 UI 얻기
        /// </summary>
        private UI_ShortCutSlot GetShortCutSlotUI(string bindKey)
        { 
            return uiPlayerStateHUD.shortCutSlots.SingleOrDefault(slot => slot.GetSlotBindKeyId().Equals(bindKey));
        }
        
        /// <summary>
        /// 퀵슬롯 UI 데이터 할당
        /// </summary>
        public void SetShortCutItemUI(string bindKey, UI_ChildInventoryPanelEmptySlot cipEmptySlot)
        {
            var shortCut = GetShortCutSlotUI(bindKey);
            if (shortCut)
            {
                shortCut.SetShortCutItem(cipEmptySlot);
            }
        }

        /// <summary>
        /// 퀵슬롯 아이템 개수 변경
        /// </summary>
        public void UpdateShortCutItemAmount(string bindKey, int itemAmount)
        {
            var shortCut = GetShortCutSlotUI(bindKey);
            if (shortCut)
            {
                shortCut.UpdateShortCutItemAmount(itemAmount);
            }
        }

        /// <summary>
        /// 퀵슬롯 데이터 지우기
        /// </summary>
        public void RemoveShortCutData(string bindKey)
        {
            if (_lastHighLightShortcut && _lastHighLightShortcut.GetSlotBindKeyId().Equals(bindKey))
            {
                _lastHighLightShortcut.DisableShortCutHighLight();
                _lastHighLightShortcut = null;
            }
            var shortCut = GetShortCutSlotUI(bindKey);
            shortCut.RemoveShortCutItem();
        }
        
        /// <summary>
        /// 퀵슬롯 하이라이트
        /// </summary>
        public void SetHighLightShortCut(string bindKey)
        {
            if (_lastHighLightShortcut && !_lastHighLightShortcut.GetSlotBindKeyId().Equals(bindKey))
            {
                _lastHighLightShortcut.DisableShortCutHighLight();
                _lastHighLightShortcut = null; 
            }
            
            if (!_lastHighLightShortcut)
            {
                _lastHighLightShortcut = GetShortCutSlotUI(bindKey);
                if (_lastHighLightShortcut) _lastHighLightShortcut.EnableShortCutHighLight();
            }

            HidePlayerHUDPanel();
        }

        #endregion

    #endregion
   
    }

    /// <summary>
    /// 매뉴얼 북 정보 복사본
    /// </summary>
    public class ManualBookCloneData
    {
        public int manualBookId { get; private set; }
        public RequestManualDataType requestManualDataType { get; private set; }
        public GameObject manualBookUiPrefab { get; private set; }

        public ManualBookCloneData(int id, RequestManualDataType dataType, GameObject prefab)
        {
            this.manualBookId = manualBookId;
            this.requestManualDataType = dataType;
            this.manualBookUiPrefab = prefab;
        }
        
    }
    
    /// <summary>
    /// UI 팝업 Task (UI Stack에 사용)
    /// </summary>
    public class UI_PopUpTask
    {
        // 관리 대상 UI 오브젝트
        public GameObject targetUI { get; private set; } = null;
        // 활성화 이벤트
        public UnityAction enableAction = null;
        public UnityAction disableAction = null;

        public UI_PopUpTask (GameObject targetUI, UnityAction enableAction,  UnityAction disableAction)
        {
            this.targetUI = targetUI;
            this.enableAction = enableAction;
            this.disableAction = disableAction;
        }
    }

    public abstract class FadeActionTask
    {
        public Queue<Action> fadeActions = new Queue<Action>();
        public bool isFadeActionCoroutineBusy { get; set; } = false;
        
        public void FadeActionTaskHandle(Action fadeTask, string debug)
        {
            if (isFadeActionCoroutineBusy)
            {
               // Debug.Log(debug + " 큐에 넣기");
                fadeActions.Enqueue(fadeTask);
                
            }
            else
            {
                //Debug.Log(debug + " 작업 실행");
                fadeTask?.Invoke();
            }
        }
    }

    public class HintFadeTaskHandler : FadeActionTask
    {
        
    }
    
}


