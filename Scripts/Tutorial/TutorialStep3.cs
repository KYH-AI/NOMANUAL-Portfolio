using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using HFPS.Player;
using NoManual.Managers;
using UnityEngine;
using UnityEngine.Events;

namespace NoManual.Tutorial
{

    /// <summary>
    /// 플레이어 이동
    /// </summary>
    public class TutorialStep3 : TutorialManager.TutorialNode
    {
        private List<TutorialManager.TutorialStepProcessNode> _tutorialStepProcessNodes;
        private int _step3TutorialIndex = 0;
        
        public TutorialStep3(TutorialStep step, PlayerController playerControl, MouseLook mouseLook, MeshCollider chairCollider) : base(step)
        {
            HintTextDelayTime = 3f;
            TutorialProcessDelayTime = HintTextDelayTime + 2f;
            
            // Wake Up 애니메이션 및 마우스 각도 제한 이벤트
            UnityAction playerChangeState = () =>
            {
                playerControl.basicSettings.stateChangeSpeed = 0.25f;
               playerControl.characterState = PlayerController.CharacterState.Stand;
               
               // 동시에 일어서는 애니메이션과 동시에 강제로 캐릭터 위치를 X,Y 축 이동
               Vector3 targetXPosition = playerControl.transform.position + (Vector3.left * 0.5f) + (Vector3.up * 0.22f);
               playerControl.transform.DOMove(targetXPosition, 2f);
            };
            // Wake Up 애니메이션 및 마우스 각도 제한 해체 이벤트 그리고 의자 콜라이더 활성화
            UnityAction mouseAngleAndStateChangeSpeedDefault = () =>
            {
                playerControl.basicSettings.stateChangeSpeed = 3f;
                mouseLook.minimumX = -360f;
                mouseLook.maximumX = 360f;
                chairCollider.enabled = true; 
            };

            // 단계별 튜토리얼 프로세스 등록
            _tutorialStepProcessNodes = new List<TutorialManager.TutorialStepProcessNode>(2)
            {
                new WakeUpTutorialStep(playerChangeState),
                new MoveTutorialStep(mouseAngleAndStateChangeSpeedDefault)
            };
                
            _tutorialStepProcessNodes[0].TutorialStepStartProcess();
            HintTextKey = _tutorialStepProcessNodes[0].hintTableTextKey;
            // 튜토리얼 진행 딜레이
            TutorialProcessDelayTime = _tutorialStepProcessNodes[0].TutorialProcessDelayTime;
            // 가이드 텍스트 딜레이
            HintTextDelayTime = _tutorialStepProcessNodes[0].HintTextDelayTime;
      
        }
        
        public override void TutorialUpdateProcess()
        {
            HandleHintTextDelay();
            if (!SendSubtitleTriggered)
            {
                SendSubtitle = true;
            }

            if (HandleTutorialProcessDelay())
            {
                if (_tutorialStepProcessNodes[_step3TutorialIndex].TutorialStepUpdateProcess())
                {
                    // 힌트 텍스트 닫기
                    NoManualHotelManager.Instance.UiNoManualUIManager.HideHintText(HintFadeHideSpeed);
                        
                    _step3TutorialIndex++;
                    // 모든 Step2 튜토리얼 모두 클리어
                    if (_step3TutorialIndex >= _tutorialStepProcessNodes.Count)
                    {
                        IsClear = true;
                        return;
                    }
                            
                    // 가이드 텍스트 출력 교체
                    HintTextKey = _tutorialStepProcessNodes[_step3TutorialIndex].hintTableTextKey;
                    SendHintTriggered = false;
                    SendHint = false;

                    // 튜토리얼 진행 딜레이
                    TutorialProcessDelayTimer = 0f;
                    TutorialProcessDelayTime = _tutorialStepProcessNodes[_step3TutorialIndex].TutorialProcessDelayTime;
                            
                    // 가이드 텍스트 딜레이 
                    HintTextDelayTime = _tutorialStepProcessNodes[_step3TutorialIndex].HintTextDelayTime;
                            
                    _tutorialStepProcessNodes[_step3TutorialIndex].TutorialStepStartProcess();
                }
            }
        }

        public override LocalizationTable.HintTableTextKey SendHintText()
        {
            HintFadeDuration = 2.5f;
            HintTextPingPong = true;
            SendHintTriggered = true; // 더 이상 가이드 텍스트 출력 X 트리거 작동
            SendHint = false; // UI에게 가이드 텍스트 출력 완료
            return HintTextKey;
        }

        public override TutorialManager.NpcTalkMapper SendTitleText()
        {
            return NpcTalkMapper;
        }
        
        #region Step3 튜토리얼 순차적 스텝
        
        /// <summary>
        /// 플레이어 WakeUp 애니메이션 진행
        /// </summary>
        public class WakeUpTutorialStep : TutorialManager.TutorialStepProcessNode
        {
            private UnityAction _wakeUpAnimation;

            public WakeUpTutorialStep(UnityAction wakeUpAnimation)
            {
                this._wakeUpAnimation = wakeUpAnimation;
                HintTextDelayTime = 0f;
                TutorialProcessDelayTime = 3f;
            }
            
            public override bool TutorialStepUpdateProcess()
            {
                // 플레이어 캐릭터 일어나는 애니메이션
                _wakeUpAnimation?.Invoke();
                _wakeUpAnimation = null;
                return true;
            }
        }
        
        /// <summary>
        /// 이동 튜토리얼
        /// </summary>
        public class MoveTutorialStep : TutorialManager.TutorialStepProcessNode
        {
            private UnityAction _moveActionUnLock;

            public MoveTutorialStep(UnityAction moveActionUnLock)
            {
                this._moveActionUnLock = moveActionUnLock;
                hintTableTextKey = LocalizationTable.HintTableTextKey.TutorialHint_KeyBoardMove;
                HintTextDelayTime = 3f;
                TutorialProcessDelayTime = 2f + HintTextDelayTime;
            }
            
            public override bool TutorialStepUpdateProcess()
            {
                // 플레이어 마우스 각도 제한 해체
                _moveActionUnLock?.Invoke();
                _moveActionUnLock = null;
                // 플레이어 모든 행동 제한 해체
                HFPS.Systems.HFPS_GameManager.Instance.PlayerActionAllControllerLock(true);
  
                return true;
            }
        }
        
        #endregion
    }
}

