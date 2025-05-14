using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using HFPS.Player;
using UnityEngine;

namespace NoManual.StateMachine
{
    public class HoldBreathState : BaseState
    {
        public enum HoldBreathStateEnum
        {
            None = -1,
            Ready = 0,
            Keep = 1,
            Out = 2,
        }

        public HoldBreathStateEnum holdBreathStateEnum { get; private set; } = HoldBreathStateEnum.None;
  
        private readonly float HOLD_BREATH_IN_TIME;    // 심호흡 In 애니메이션 시간 (3초) -> 2초는 offset 값 
        private readonly float HOLD_BREATH_KEEP_TIME; //  심호흡 Keep 애니메이션 시간
        private readonly float HOLD_BREATH_OUT_TIME;   // 심호흡 Out 애니메이션 시간 (1초)
        private const float DEEP_BREATH_TIME = 4.5f;   // 깊은 심호흡 유지 시간
        public float holdBreathTimer = 0f; // 심호흡 유지 시간
        
        private int _deepBreathCount = 0;  // 깊은 심호흡 횟수
        
        
        public HoldBreathState(float inTime, float keepTime, float outTime)
        {
            HOLD_BREATH_IN_TIME = inTime - 0.5f;
            HOLD_BREATH_KEEP_TIME = keepTime - 0.5f;
            HOLD_BREATH_OUT_TIME = outTime - 0.5f;
        }

        public override void OnEnterState()
        {
            holdBreathTimer = 0f;
            holdBreathStateEnum = HoldBreathStateEnum.Ready;
            PlayerController.Instance.cameraHeadBob.cameraStateType = PlayerController.CameraHeadBob.CameraStateType.HoldBreath;
            Managers.NoManualHotelManager.Instance.CoroutineManager.StartCoroutineProcess(HoldBreathTaskCoroutine());
        }

        public override void OnUpdateState()
        {
    
        }

        public override void OnFixedUpdateState()
        {
        
        }
        
        public override void OnExitState()
        {
            PlayerController.Instance.cameraHeadBob.cameraStateType = PlayerController.CameraHeadBob.CameraStateType.Idle;
            holdBreathStateEnum = HoldBreathStateEnum.None;
        }
        
        private IEnumerator HoldBreathTaskCoroutine()
        {
            yield return new WaitForSeconds(HOLD_BREATH_IN_TIME);

            //Debug.Log("숨 참기 코루틴");
            
            holdBreathStateEnum = HoldBreathStateEnum.Keep;

            while (PlayerController.Instance.isHoldBreath && PlayerController.Instance.CurrentStamina > 0f)
            {
                // TODO: 스테미너 감소

                // 4.5초 동안 심호흡을 했으면 정신력 회복
                if (holdBreathTimer >= DEEP_BREATH_TIME)
                {
                    // TODO: 정신력 회복 로직
                   // Debug.Log("정신력 회복 완료");
                    holdBreathTimer = 0f;
                }

                holdBreathTimer += Time.deltaTime;
                
                yield return null;
            }

            if (PlayerController.Instance.SpecialStateMachine.CurrentState is Exhausted)
            {
             //   Debug.Log("숨 참기 -> 탈진");
                yield break;
            }

           // Debug.Log("숨 참기 -> 대기");
            holdBreathStateEnum = HoldBreathStateEnum.Out;
            yield return new WaitForSeconds(HOLD_BREATH_OUT_TIME);
            PlayerController.Instance.SpecialStateMachine.ChangeState(PlayerSpecialState.Idle);
        }
        
    }
}


