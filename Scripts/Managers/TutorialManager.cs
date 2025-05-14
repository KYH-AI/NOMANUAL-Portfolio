using System;
using System.Collections;
using System.Collections.Generic;
using HFPS.Player;
using HFPS.Systems;
using NoManual.Interaction;
using UnityEngine;
using NoManual.NPC;
using NoManual.Tutorial;
using TMPro;
using UnityEngine.Events;

namespace NoManual.Managers
{
    public class TutorialManager : MonoBehaviour
    {
        
       [SerializeField] private NpcManager _npcManager;
        private PlayerController _playerController;
        private MouseLook _mouseLook;
        private CutSceneManager _cutSceneManager;
        private Camera _playerCam;
        private InventoryManager _inventory;
        
        private bool _stepByClearTutorial;
        private LinkedList<TutorialNode> _tutorialLinkedList;
        private LinkedListNode<TutorialNode> _currentNode;

        [SerializeField] private AudioSource monitorAudio;
        [SerializeField] private TextMeshProUGUI monitorText;
        [SerializeField] private DoorComponent treatmentDoor;
        [SerializeField] private NPC.NPC[] tutorialNpcArray;
        [SerializeField] private ItemComponent[] itemComponents;
        [SerializeField] private Transform[] nurseWayPoints;
        [SerializeField] private Transform doctorSpawnPoint;
        [SerializeField] private MeshCollider playerChairCollider;
        
        /// <summary>
        /// 튜토리얼 노드
        /// </summary>
        public abstract class TutorialNode
        {
            public enum TutorialStep
            {
                None = -1,
                Step1 = 0,
                Step2 = 1,
                Step3 = 2,
                Step4 = 3,
                Step5 = 4,
            }
            
            public TutorialStep tutorialStep { get; private set; }
            
            /// <summary>
            /// 튜토리얼 프로세스 클리어 여부
            /// </summary>
            public bool IsClear { get; protected set; } = false;

            /// <summary>
            /// 가이드 텍스트 출력 여부
            /// </summary>
            public bool SendHint { get; protected set; } = false;

            /// <summary>
            /// 가이드 텍스트 Fade 핑퐁 여부
            /// </summary>
            public bool HintTextPingPong { get; protected set; } = false;
            
            /// <summary>
            /// 가이드 텍스트 Fade In 전환속도
            /// </summary>
            public float HintFadeSpeed { get; protected set; } = 1.5f;

            /// <summary>
            /// 가이드 텍스트 Fade Out 전환속도
            /// </summary>
            protected readonly float HintFadeHideSpeed = 1f;
            
            /// <summary>
            /// 가이드 텍스트 Fade 지속시간
            /// </summary>
            public float HintFadeDuration { get; protected set; } = 0f;
            
            /// <summary>
            /// 자막 텍스트 출력 여부
            /// </summary>
            public bool SendSubtitle { get; protected set; } = false;
            
            // 튜토리얼 프로세스 딜레이
            protected float TutorialProcessDelayTime;
            // 튜토리얼 프로세스 딜레이 타이머
            protected float TutorialProcessDelayTimer;
            // 가이드 텍스트 출력을 한 번만 작동하도록 보장하기 위한 트리거 변수
            protected bool SendHintTriggered = false;
            // 가이트 텍스트 딜레이
            protected float HintTextDelayTime;
            // 가이드 텍스트 출력을 한 번만 작동하도록 보장하기 위한 트리거 변수
            protected bool SendSubtitleTriggered = false;
            
            public LocalizationTable.HintTableTextKey HintTextKey = LocalizationTable.HintTableTextKey.None;
            public LocalizationTable.NPCTableTextKey NpcTableTextKey = LocalizationTable.NPCTableTextKey.None;
            protected NpcTalkMapper NpcTalkMapper;

            /// <summary>
            /// 튜토리얼 노드 생성자
            /// </summary>
            public TutorialNode(TutorialStep step)
            {
                this.tutorialStep = step;
            }
            /// <summary>
            /// 튜토리얼 시작 프로세스
            /// </summary>
            public virtual void TutorialStartProcess() { }
            /// <summary>
            /// 튜토리얼 업데이트 프로세스
            /// </summary>
            public abstract void TutorialUpdateProcess();
            /// <summary>
            /// 가이드 텍스트 출력
            /// </summary>
            public abstract LocalizationTable.HintTableTextKey SendHintText();
            /// <summary>
            /// 자막 텍스트 출력
            /// </summary>
            public abstract NpcTalkMapper SendTitleText();
            /// <summary>
            /// 가이드 텍스트 출력 딜레이
            /// </summary>
            protected void HandleHintTextDelay()
            {
                if (!SendHintTriggered && HintTextDelayTime <= 0f)
                {
                    SendHint = true;
                }
                else if(HintTextDelayTime >= 0f)
                {
                    HintTextDelayTime -= Time.deltaTime;
                }
            }
            /// <summary>
            /// 튜토리얼 프로세스 딜레이
            /// </summary>
            protected bool HandleTutorialProcessDelay()
            {
                if (TutorialProcessDelayTimer <= TutorialProcessDelayTime)
                {
                    TutorialProcessDelayTimer += Time.deltaTime;
                    return false;
                }
                return true;
            }
        }
        /// <summary>
        /// 튜토리얼 단계별 진행 노드
        /// </summary>
        public abstract class TutorialStepProcessNode
        {
            // 가이트 텍스트 딜레이
            public float HintTextDelayTime { get; protected set; } = 0f;
            // 튜토리얼 프로세스 딜레이
            public float TutorialProcessDelayTime { get; protected set; } = 0f;
            

            public LocalizationTable.HintTableTextKey hintTableTextKey { get; protected set; } = LocalizationTable.HintTableTextKey.None;
            public LocalizationTable.NPCTableTextKey npcTableTextKey { get; protected set; } = LocalizationTable.NPCTableTextKey.None;
            
            /// <summary>
            /// 튜토리얼 Awake 노드
            /// </summary>
            public virtual void TutorialStepAwakeProcess(){ }
            /// <summary>
            /// 튜토리얼 Start 노드
            /// </summary>
            public virtual void TutorialStepStartProcess() { }
            /// <summary>
            /// 튜토리얼 단계별 진행 프로세스
            /// </summary>
            public abstract bool TutorialStepUpdateProcess();
        }
        /// <summary>
        /// NPC 대사 및 음성 매핑
        /// </summary>
        public class NpcTalkMapper
        {
            public NPC_Component NpcComponent { get; set; }
            public LocalizationTable.NPCTableTextKey NpcTableTextKey { get; set; }

            public NpcTalkMapper(NPC_Component npcComponent, LocalizationTable.NPCTableTextKey textKey)
            {
                this.NpcComponent = npcComponent;
                this.NpcTableTextKey = textKey;
            }
        }
        

        /// <summary>
        /// 튜토리얼 노드 생성 및 첫 번째 노드 가져오기
        /// </summary>
        private void Start()
        {
            // 스크립트 참조
            _playerController = PlayerController.Instance;
            _playerCam = ScriptManager.Instance.MainCamera;
            _cutSceneManager = NoManualHotelManager.Instance.CutSceneManager;
            _mouseLook = ScriptManager.Instance.C<MouseLook>();
            _inventory = NoManualHotelManager.Instance.InventoryManager;
            
            // 플레이어 의자 콜라이더 비활성화
            playerChairCollider.enabled = false;
            
            // NPC 프리팹 생성
            Dictionary<NPC.NPC, NPC_Component> tutorialNpc = new Dictionary<NPC.NPC, NPC_Component>(tutorialNpcArray.Length);
            for (int i = 0; i < tutorialNpcArray.Length; i++)
            {
                NPC.NPC npc = tutorialNpcArray[i];
                _npcManager.CreateNpcPrefab(npc);
                NPC_Component tutorialNpcComponent = _npcManager.GetNpcComponent(npc);
                if (tutorialNpcComponent)
                {
                    tutorialNpc.Add(tutorialNpcArray[i], tutorialNpcComponent);
                }
            }

            // 간호사 AI 이동경로 설정
            NPC_Nurse nurse = (NPC_Nurse)tutorialNpc[NPC.NPC.Nurse];
            nurse.wayPoints = nurseWayPoints;
            // 간호사 IK 가중치 1설정
            nurse.SetRigWeight(0.4f);
            nurse.LookAtAim(_playerController.mouseLook);

            
            // 튜토리얼 노드 생성
            _tutorialLinkedList = new LinkedList<TutorialNode>();
            _tutorialLinkedList.AddLast(new TutorialStep1(TutorialNode.TutorialStep.Step1));
            _tutorialLinkedList.AddLast(new TutorialStep2(TutorialNode.TutorialStep.Step2, nurse, _playerController, _playerCam));
            _tutorialLinkedList.AddLast(new TutorialStep3(TutorialNode.TutorialStep.Step3, _playerController, _mouseLook, playerChairCollider));
            _tutorialLinkedList.AddLast(new TutorialStep4(TutorialNode.TutorialStep.Step4, this, _inventory, itemComponents[0], itemComponents[1]));
            _tutorialLinkedList.AddLast(new TutorialStep5(TutorialNode.TutorialStep.Step5, treatmentDoor, monitorAudio, monitorText));
            
            Step0FreezePlayer();
            _currentNode = _tutorialLinkedList.First;

            // Fade In 즉시 설정  후 2초 뒤 Fade Out 효과 진행
            NoManualHotelManager.Instance.UiNoManualUIManager.FadePanel(true);
           Invoke(nameof(FadeOutDelay), 2f);
         
 
        }

        [ContextMenu("Wake Up")]
        public void TestWakeUp()
        {
            // 일어서기
            _playerController.basicSettings.stateChangeSpeed = 0.25f;
            _playerController.characterState = PlayerController.CharacterState.Stand;
        }
        
        [ContextMenu("Sit Down")]
        public void TestSitDown()
        {
            // 자리 앉기
            _playerController.basicSettings.stateChangeSpeed = 3f;
            _playerController.characterState = PlayerController.CharacterState.Sit;
        }

        private void FadeOutDelay()
        {
            // 튜토리얼 시작 Fade Out
            NoManualHotelManager.Instance.PlayerStartFade(false, 20f, true, 0.45f);
        }

        private void Update()
        {
            if (_currentNode == null)
            {
                if (!_cutSceneManager.CutSceneRunning)
                {
                    // 강제로 Fade In 진행 후 타임라인으로 Fade 제어
                    NoManualHotelManager.Instance.UiNoManualUIManager.FadePanel(true);
                    
                    /******** Handover 씬에서 진행 ********/
                    // 인벤토리에 진정제 아이템 등록
                    // GameManager.Instance.SaveGameManager.CurrentPlayerSaveData.InventoryItems = new InventorySaveData[] { new InventorySaveData(_MEDICINE_ITEM_ID, 1) };
                    // 튜토리얼 클리어 
                    // GameManager.Instance.SaveGameManager.CurrentPlayerSaveData.IsTutorialClear = true;
                    // 세이브 파일 덮어쓰기
                    // GameManager.Instance.SaveGameManager.SaveFile();
                    
                    // 인수인계 씬 이동
                    GameManager.Instance.NextScene = GameManager.SceneName.Handover;
                    
                    // 의사면담 타임라인 시작 (Fade In, 씬 변경 이벤트 등록)
                    _cutSceneManager.PlayTimeLine(CutSceneManager.CutSceneType.Tutorial_Meet_To_The_Doctor, 
                                    new Action[]
                                    {
                                        () => NoManualHotelManager.Instance.UiNoManualUIManager.FadePanelHandleSubtitleText(false), // Fade 자막 끄기
                                        () => NoManualHotelManager.Instance.OpenScene(GameManager.SceneName.Loading, 0.15f), // 씬 넘기기
                                    });
                    
                    // 튜토리얼 매니저 스크립트 비활성화 
                    this.enabled = false;
                    return;
                }
            }
            
            // 튜토리얼 클리어 확인 후 다음 튜토리얼 진행
            if (_currentNode.Value.IsClear)
            {
                // 더 이상 진행할 튜토리얼 노드가 없음
                if (_currentNode.Next == null)
                {
                    _currentNode = null;
                    return;
                }
                _currentNode = _currentNode.Next;
                // 다음 튜토리얼 진행
                _currentNode.Value.TutorialStartProcess();
            }
            
            // 튜토리얼 프로세스 및 가이드 텍스트 진행
            if(_currentNode != null)
            {
                // 가이드 텍스트 출력
                if (_currentNode.Value.SendHint)
                {
                   var hintText = _currentNode.Value.SendHintText();
                   if (hintText != LocalizationTable.HintTableTextKey.None)
                   {
                       ShowHintText(hintText, _currentNode.Value.HintTextPingPong, _currentNode.Value.HintFadeSpeed, _currentNode.Value.HintFadeDuration);
                   } 
                }
                // 자막 텍스트 출력
                if (_currentNode.Value.SendSubtitle)
                {      
                    NpcTalkMapper subTitleText = _currentNode.Value.SendTitleText();
                    if (subTitleText != null)
                    {
                        ShowSubtitleText(subTitleText.NpcComponent, subTitleText.NpcTableTextKey);
                    } 
                    
                }
                // 튜토리얼 진행
                _currentNode.Value.TutorialUpdateProcess();
            }
        }

        /// <summary>
        /// 플레이어 이동 제한
        /// </summary>
        private void Step0FreezePlayer()
        {
            Invoke(nameof(InventoryLock), 2f);
            HFPS_GameManager.Instance.PlayerActionAllControllerLock(false); // 플레이어 행동 입력 Lock
            HFPS_GameManager.Instance.LockPlayerControls(false, false, false); // 키보드 및 마우스 입력 Lock
            _playerController.characterState = PlayerController.CharacterState.Sit; // 자리에 앉아 있는 상태
            // 마우스 회전 X 축 각도 제한
            _mouseLook.minimumX = -45f;
            _mouseLook.maximumX = 45f;
        }

        /// <summary>
        /// 인벤토리 입력 Lock
        /// </summary>
        private void InventoryLock()
        {
            ThunderWire.Input.InputHandler.InputActionLockControl(false, "Inventory");
        }
        

        private void ShowHintText(LocalizationTable.HintTableTextKey hintTextKey, bool isPingPong, float fadeSpeed, float fadeDuration)
        {
            string hintText = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.Hint_Table, hintTextKey);
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowHintText(hintText, isPingPong, fadeSpeed, fadeDuration);
        }

        private void ShowSubtitleText(NPC_Component npcComponent, LocalizationTable.NPCTableTextKey npcTableTextKey)
        {
            _npcManager.PlayNpcTalk(npcComponent, npcTableTextKey, true);
        }
        

        /// <summary>
        /// 튜토리얼 노드 찾기
        /// </summary>
        private T FindTutorialNode<T>(TutorialNode.TutorialStep findTutorialStep) where T : TutorialNode
        {
            LinkedListNode<TutorialNode> currentNode = _tutorialLinkedList.First;

            while (currentNode != null)
            {
                if (currentNode.Value.tutorialStep == findTutorialStep)
                {
                    return (T)currentNode.Value; // 노드를 찾았으면 해당 노드를 반환
                }

                currentNode = currentNode.Next; // 다음 노드로 이동
            }

            return null; // 노드를 찾지 못했을 경우 null을 반환
        }
    }
}
