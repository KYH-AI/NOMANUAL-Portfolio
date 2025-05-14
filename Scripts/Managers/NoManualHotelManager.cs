using System;
using System.Collections;
using System.Collections.Generic;
using HFPS.Systems;
using NoManual.StateMachine;
using NoManual.Utils;
using UnityEngine;
using UnityEngine.Audio;

namespace NoManual.Managers
{
    public class NoManualHotelManager : Singleton<NoManualHotelManager>
    {
        #region 매니저 관련 변수

        private UI_NoManualUIManager _uiNoManualUIManager;
        private CoroutineManager _coroutineManager;
        private ItemShortCutManager _itemShortCutManager;
        private FloatingIconManager _floatingIconManager;
        private JumpScareManager _jumpScareManager;
        private InventoryManager _inventoryManager;
        private InteractManager _interactManager;
        private TextDataBase _textDataBase;
        private AudioManager _audioManager;
        private CutSceneManager _cutSceneManager;

        public UI_NoManualUIManager UiNoManualUIManager => _uiNoManualUIManager;
        public CoroutineManager CoroutineManager => _coroutineManager;
        public ItemShortCutManager ItemShortCutManager => _itemShortCutManager;
        public FloatingIconManager FloatingIconManager => _floatingIconManager;
        public JumpScareManager JumpScareManager => _jumpScareManager;
        public InventoryManager InventoryManager => _inventoryManager;
        public InteractManager InteractManager => _interactManager;
        public TextDataBase TextDataBase => _textDataBase;
        public AudioManager AudioManager => _audioManager;
        public CutSceneManager CutSceneManager => _cutSceneManager;

        public bool OnInitialized { get; private set; } = false;// 초기화 완료 확인

        #endregion

        #region 세이브 데이터 이벤트
        
        public event Action<bool> HotelSaveDataOvreWriteEvent;

        #endregion
        
        private void Awake()
        {
            _uiNoManualUIManager = GetComponentInChildren<UI_NoManualUIManager>();
            _coroutineManager = GetComponentInChildren<CoroutineManager>();
            _itemShortCutManager = GetComponentInChildren<ItemShortCutManager>();
            _floatingIconManager = GetComponentInChildren<FloatingIconManager>();
            _jumpScareManager = GetComponentInChildren<JumpScareManager>();
            _inventoryManager = GetComponentInChildren<InventoryManager>();
            _interactManager = GetComponentInChildren<InteractManager>();
            _textDataBase = GetComponentInChildren<TextDataBase>();
            _audioManager = GetComponentInChildren<AudioManager>();
            _cutSceneManager = GetComponentInChildren<CutSceneManager>();

            // 오디오 믹서 할당
            AudioMixer audioMixer;
            if (!GameManager.HasReference)
            {
                audioMixer = Resources.Load<AudioMixer>("Audio/Master Audio Mixer");
            }
            else
            {
                if (GameManager.Instance.optionHandler == null)
                    audioMixer = Resources.Load<AudioMixer>("Audio/Master Audio Mixer");
                else 
                    audioMixer = GameManager.Instance.optionHandler.audioMixer;
            }
            _audioManager.audioMixer = audioMixer;

            // 현재 세이브 파일 읽기
            PlayerSaveData currentSaveFile = null;

#if !UNITY_EDITOR
             GameManager.Instance.DebugMode = false;
#endif
            if (!GameManager.Instance.DebugMode) currentSaveFile = GetCurrentSaveFile();

            // 인벤토리 초기화
            _inventoryManager.InitInventory();
            
            // 플레이어 정신력 세이브 파일 불러오기
            HFPS.Player.PlayerController.Instance.mentality.InitializationMentality(currentSaveFile?.Mentatilty ?? Mentality.MAX_MENTALITY);
            
            // 퀵슬롯 초기화
            _itemShortCutManager.InitializationShortCut(_inventoryManager, FindObjectOfType<EquipItemSwitcherManager>(), _uiNoManualUIManager, _cutSceneManager);

            // 인벤토리 세이브 파일 불러오기
            _inventoryManager.InitializationInventory(currentSaveFile?.InventoryItems, currentSaveFile?.ShortCutData);
            
            // 매니저 초기화 완료
            OnInitialized = true;
        }
        
        
        /// <summary>
        /// 게임 시작 fade
        /// </summary>
        public void PlayerStartFade(bool uiFadeIn, float uiFadeDuration, bool soundFadeIn, float soundTargetValue)
        {
            UiNoManualUIManager.FadePanel(uiFadeIn, uiFadeDuration);
            AudioManager.PlayBGM_Fade(soundFadeIn, soundTargetValue);
        }


        /// <summary>
        /// 메뉴판에서 로비로 이동 (일시정지 UI Exit Game 버튼 이벤트)
        /// </summary>
        public void OpenMainScene()
        {
            //OpenScene(GameManager.SceneName.Main);
            SceneMove(false, false, GameManager.SceneName.Main);
        }

        /// <summary>
        /// 게임 재시작
        /// </summary>
        public void RetryGame()
        {
            GameManager.Instance.NextScene = GameManager.SceneName.Hotel;
            SceneMove(false, false, GameManager.SceneName.Loading);
        }
        
        /// <summary>
        /// 상호작용으로 씬 이동
        /// </summary>
        public void SceneMove(bool needSaveFile, bool allSaveFile, GameManager.SceneName targetScene, float uiFadeDuration = 2.5f)
        {
            if(needSaveFile) SaveToOverWriteFile(allSaveFile);
            DisablePlayer();
            List<UnityEngine.Events.UnityAction> fadeEvent = new List<UnityEngine.Events.UnityAction>()
            {
                () => Cursor.visible = true, // 마우스 커서 활성화
                () => Cursor.lockState = CursorLockMode.None, // 마우스 잠금 비활성화
                () => OpenScene(targetScene), // 씬 변경
            };
            UiNoManualUIManager.FadePanel(true, uiFadeDuration, fadeEvent);
        }
        
        /// <summary>
        /// 씬 교체 
        /// </summary>
        public void OpenScene(GameManager.SceneName openScene)
        {
            if (GameManager.Instance)
            {
                GameManager.Instance.OpenScene(openScene);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene((int)openScene);
            }
        }
        
        /// <summary>
        /// 씬 교체 (딜레이 추가)
        /// </summary>
        public void OpenScene(GameManager.SceneName openScene, float delay)
        {
            StartCoroutine(DelayOpenScene(openScene, delay));
        }

        private IEnumerator DelayOpenScene(GameManager.SceneName openScene, float delay)
        {
            yield return new WaitForSeconds(delay);
            OpenScene(openScene);
        }

        /// <summary>
        /// 플레이어 모든 입력 제한 해체
        /// </summary>
        public void EnablePlayer()
        {
            ThunderWire.Input.InputHandler.InputActionLockControl(true, "Inventory");
            HFPS.Systems.HFPS_GameManager.Instance.gamePanels.MainGamePanel.SetActive(true);
            HFPS.Systems.HFPS_GameManager.Instance.LockPlayerControls(true, true, false);
            HFPS.Systems.HFPS_GameManager.Instance.PlayerActionAllControllerLock(true);
            HFPS.Systems.HFPS_GameManager.Instance.userInterface.Crosshair.enabled = true;
        }

        /// <summary>
        /// 플레이어 모든 입력 제한
        /// </summary>
        public void DisablePlayer(bool uiShow = false)
        {
            ThunderWire.Input.InputHandler.InputActionLockControl(false, "Inventory");
            HFPS.Systems.HFPS_GameManager.Instance.gamePanels.MainGamePanel.SetActive(uiShow);
            HFPS.Systems.HFPS_GameManager.Instance.LockPlayerControls(false, false, false);
            HFPS.Systems.HFPS_GameManager.Instance.PlayerActionAllControllerLock(false);
            HFPS.Systems.HFPS_GameManager.Instance.userInterface.Crosshair.enabled = uiShow;
        }
        

        #region 세이브파일 저장 및 불러오기

        public PlayerSaveData GetCurrentSaveFile()
        {
            PlayerSaveData saveData = GameManager.Instance.SaveGameManager.CurrentPlayerSaveData;
            if(saveData == null) ErrorCode.SendError(ErrorCode.ErrorCodeEnum.GetSaveFile);
            return saveData;
        }

        public void SaveToOverWriteFile(bool allSaveFile)
        {
            if (GameManager.Instance.SaveGameManager.CurrentPlayerSaveData != null)
            {
                // 인벤토리 세이브 데이터
                GameManager.Instance.SaveGameManager.CurrentPlayerSaveData.InventoryItems = _inventoryManager.SaveInventoryItems();
                
                // 퀵슬롯 세이브 데이터
                GameManager.Instance.SaveGameManager.CurrentPlayerSaveData.ShortCutData = _itemShortCutManager.SaveShortCutData();
                
                // 호텔 세이브 데이터
                HotelSaveDataOvreWriteEvent?.Invoke(allSaveFile);
            }
        }

        #endregion

        private void OnDestroy()
        {
            HotelSaveDataOvreWriteEvent = null;
        }
    }  
}