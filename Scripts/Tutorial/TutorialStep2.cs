using System.Collections;
using System.Collections.Generic;
using HFPS.Player;
using NoManual.Managers;
using UnityEngine;
using NoManual.NPC;
using NoManual.StateMachine;
using UnityEngine.Events;

namespace NoManual.Tutorial
{

    /// <summary>
    /// 간호사 관련 튜토리얼 
    /// </summary>
    public class TutorialStep2 : TutorialManager.TutorialNode
    {
        public NPC_Nurse NurseNpc { get; set; }
        private List<TutorialManager.TutorialStepProcessNode> _tutorialStepProcessNodes;
        private int _step2TutorialIndex = 0;
        private UnityAction _nurseDisableIk;
        private UnityAction _nurseEnableIk;
        
        public TutorialStep2(TutorialStep step, NPC_Nurse nurse, PlayerController playerControl, Camera playerCam) : base(step)
        {
            this.NurseNpc = nurse;
            _nurseDisableIk += () => nurse.SetRigWeight(0f);
            _nurseEnableIk += () => nurse.SetRigWeight(1f);

           // 단계별 튜토리얼 프로세스 등록
            _tutorialStepProcessNodes = new List<TutorialManager.TutorialStepProcessNode>(4)
            {
                new RayCastTutorialStep(playerCam),
                new DeepBreathTutorialStep(playerControl),
                new BlinkEyeTutorial(playerControl),
                new NurseReturnTutorialStep(NurseNpc)
            };
            
            _tutorialStepProcessNodes[0].TutorialStepStartProcess();
            
            // 간호사 자막 및 음성 매핑
            NpcTalkMapper = new TutorialManager.NpcTalkMapper(NurseNpc, LocalizationTable.NPCTableTextKey.None);
            NpcTalkMapper.NpcTableTextKey = _tutorialStepProcessNodes[0].npcTableTextKey;
            HintTextKey = _tutorialStepProcessNodes[0].hintTableTextKey;
            // 튜토리얼 진행 딜레이
            TutorialProcessDelayTime = _tutorialStepProcessNodes[0].TutorialProcessDelayTime;
            // 가이드 텍스트 딜레이
            HintTextDelayTime = _tutorialStepProcessNodes[0].HintTextDelayTime;
        }
        
        public override void TutorialUpdateProcess()
        {
            // 간호사가 플레이어 앞에 옴
            if (NurseNpc.ArrivedAtLastWayPoint)
            {
                HandleHintTextDelay();
                if (!SendSubtitleTriggered)
                {
                    SendSubtitle = true;
                }

                if (HandleTutorialProcessDelay())
                {
                    if (_tutorialStepProcessNodes[_step2TutorialIndex].TutorialStepUpdateProcess())
                    {
                        _nurseDisableIk?.Invoke();
                      
                        // 힌트 텍스트 닫기
                        NoManualHotelManager.Instance.UiNoManualUIManager.HideHintText(HintFadeHideSpeed);
                    
                        _step2TutorialIndex++;
                        // 모든 Step2 튜토리얼 모두 클리어
                        if (_step2TutorialIndex >= _tutorialStepProcessNodes.Count)
                        {
                            IsClear = true;
                            return;
                        }
                        
                        // 자막 및 가이드 텍스트 출력 교체
                        HintTextKey = _tutorialStepProcessNodes[_step2TutorialIndex].hintTableTextKey;
                        NpcTalkMapper.NpcTableTextKey = _tutorialStepProcessNodes[_step2TutorialIndex].npcTableTextKey;
                        SendHintTriggered = false;
                        SendHint = false;
                        SendSubtitleTriggered = false;
                        SendSubtitle = false;
                        
                        // 튜토리얼 진행 딜레이
                        TutorialProcessDelayTimer = 0f;
                        TutorialProcessDelayTime = _tutorialStepProcessNodes[_step2TutorialIndex].TutorialProcessDelayTime;
                        
                        // 가이드 텍스트 딜레이 
                        HintTextDelayTime = _tutorialStepProcessNodes[_step2TutorialIndex].HintTextDelayTime;
                        
                        _tutorialStepProcessNodes[_step2TutorialIndex].TutorialStepStartProcess();
                    }
                }
            }
        }
        

        public override LocalizationTable.HintTableTextKey SendHintText()
        {
            _nurseEnableIk?.Invoke();
            HintTextPingPong = false;
            SendHintTriggered = true; // 더 이상 가이드 텍스트 출력 X 트리거 작동
            SendHint = false; // UI에게 가이드 텍스트 출력 완료
            return HintTextKey;
        }

        public override TutorialManager.NpcTalkMapper SendTitleText()
        {
            SendSubtitleTriggered = true; // 더 이상 자막 텍스트 출력 X 트리거 작동
            SendSubtitle = false;
            return NpcTalkMapper;
        }

   

        #region Step2 튜토리얼 순차적 스텝
        
        /// <summary>
        /// 레이캐스트 튜토리얼
        /// </summary>
        public class RayCastTutorialStep : TutorialManager.TutorialStepProcessNode
        {
            private Camera _playerCam;
            private float _raycastRange = 10f;
            private LayerMask _rayCastNurseNpcLayer = LayerMask.GetMask(Utils.Layer.LayerIndex.Tutorial.ToString());

            public RayCastTutorialStep(Camera playerCam)
            {
                _playerCam = playerCam;
                npcTableTextKey = LocalizationTable.NPCTableTextKey.Tutorial_Nures_Hello;
                hintTableTextKey = LocalizationTable.HintTableTextKey.TutorialHint_LookAtNures;
                HintTextDelayTime = 1f;
                TutorialProcessDelayTime = 3f;
            }

            /// <summary>
            /// 간호사 RayCast 확인
            /// </summary>
            public override bool TutorialStepUpdateProcess()
            {
                Ray playerAim = _playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

                if (Physics.Raycast(playerAim, out RaycastHit hit, _raycastRange, _rayCastNurseNpcLayer))
                {
                    if (hit.collider.gameObject.TryGetComponent<NPC_Component>(out var npcComponent))
                    {
                        if (npcComponent.NpcKey == NPC.NPC.Nurse)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        
        /// <summary>
        /// 숨쉬기 튜토리얼
        /// </summary>
        public class DeepBreathTutorialStep : TutorialManager.TutorialStepProcessNode
        {
            private PlayerController _player;

            public DeepBreathTutorialStep(PlayerController player)
            {
                this._player = player;
                npcTableTextKey = LocalizationTable.NPCTableTextKey.Tutorial_Nures_HoldBreath;
                hintTableTextKey = LocalizationTable.HintTableTextKey.TutorialHint_HoldBreath;
                HintTextDelayTime = 2.5f;
                TutorialProcessDelayTime = 1.5f;
            }
            
            public override void TutorialStepStartProcess()
            {
                HFPS.Systems.HFPS_GameManager.Instance.PlayerActionControllerLock(PlayerController.ControllerFeatures.ControllerLock.HoldBreath, true); // 숨쉬기 행동 허용
            }
            
            public override bool TutorialStepUpdateProcess()
            {
                // 플레이어 상태가 HoldBreathState이고, 상태가 'Keep'인 경우에만 true 반환
                return _player.SpecialStateMachine.CurrentState is HoldBreathState { holdBreathStateEnum: HoldBreathState.HoldBreathStateEnum.Keep };
            }
        }

        /// <summary>
        /// 눈감기 튜토리얼
        /// </summary>
        public class BlinkEyeTutorial : TutorialManager.TutorialStepProcessNode
        {
            private PlayerController _player;
            private float _blinkEyeDelay = 1.5f;
            private bool _closedEyeTrigger = false;
            public BlinkEyeTutorial(PlayerController player)
            {
                this._player = player;
                npcTableTextKey = LocalizationTable.NPCTableTextKey.Tutorial_Nures_BlinkEye;
                hintTableTextKey = LocalizationTable.HintTableTextKey.TutorialHint_BlinkEye;
                HintTextDelayTime = 2.5f;
                TutorialProcessDelayTime = 1.5f;
            }

            public override void TutorialStepStartProcess()
            {
                HFPS.Systems.HFPS_GameManager.Instance.PlayerActionControllerLock(PlayerController.ControllerFeatures.ControllerLock.BlinkEye, true); // 눈감기 행동 허용
            }
            
            public override bool TutorialStepUpdateProcess()
            {
                if (!_closedEyeTrigger)
                {
                    _closedEyeTrigger = _player.SpecialStateMachine.CurrentState is BlinkEyeState;
                }

                if (_closedEyeTrigger)
                {
                    _blinkEyeDelay -= Time.deltaTime;
                }

                return _player.SpecialStateMachine.CurrentState is not BlinkEyeState && _blinkEyeDelay <= 0f;
            }
        }
        
        /// <summary>
        /// 이동 튜토리얼
        /// </summary>
        public class NurseReturnTutorialStep : TutorialManager.TutorialStepProcessNode
        {
            private float _nurseReturnDelay; // 간호사 돌아가는 대기 시간
            private NPC_Nurse _npcNurse;

            public NurseReturnTutorialStep(NPC_Nurse nurseNpc)
            {
                this._npcNurse = nurseNpc;
                // 간호사 음성 파일 길이 가져오기
                _nurseReturnDelay = NoManualHotelManager.Instance.AudioManager.GetAudioClip(LocalizationTable.NPCTableTextKey.Tutorial_Nures_WellDone).length;
                npcTableTextKey = LocalizationTable.NPCTableTextKey.Tutorial_Nures_WellDone;
                HintTextDelayTime = 2.5f;
                _nurseReturnDelay += 5f;
                TutorialProcessDelayTime = 2f;
            }
            
            public override bool TutorialStepUpdateProcess()
            {
                // 간호사 대사 딜레이
                if (_nurseReturnDelay >= 0f)
                {
                    _nurseReturnDelay -= Time.deltaTime;
                    return false;
                }
                
                // 간호사 Return
                if (!_npcNurse.IsReverseWayPoint)
                {
                    _npcNurse.IsTurn = true;
                    _npcNurse.ReverseWayPoint();
                }
                
                return true;
            }
        }
        
        #endregion
    }
}
