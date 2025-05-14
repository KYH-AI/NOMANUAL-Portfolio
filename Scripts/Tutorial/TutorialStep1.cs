using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoManual.Managers;


namespace NoManual.Tutorial
{
    /// <summary>
    /// 마우스를 이용하여 주위를 둘러보십시오
    /// </summary>
    public class TutorialStep1 : TutorialManager.TutorialNode
    {
        public TutorialStep1(TutorialStep step) : base(step)
        {
            TutorialProcessDelayTimer = 0f;
            TutorialProcessDelayTime = 5f;
            HintTextDelayTime = 3f;
        }
        
        public override void TutorialUpdateProcess()
        {
            HandleHintTextDelay();
            if (HandleTutorialProcessDelay())
            {
                // 키보드 및 마우스 입력 UnLock (But 아직 행동 입력은 Lock 상태)
                HFPS.Systems.HFPS_GameManager.Instance.LockPlayerControls(true, true, false);
                IsClear = true;
                return;
            }
        }

        public override LocalizationTable.HintTableTextKey SendHintText()
        {
            HintTextKey = LocalizationTable.HintTableTextKey.TutorialHint_MouseMove;
            HintFadeDuration = 2f;
            HintTextPingPong = true;
            SendHintTriggered = true; // 더 이상 가이드 텍스트 출력 X 트리거 작동
            SendHint = false; // UI에게 가이드 텍스트 출력 완료
            return HintTextKey;
        }

        public override TutorialManager.NpcTalkMapper SendTitleText()
        {
            return NpcTalkMapper;
        }

   
        
    }
}


