using System.Collections;
using System.Collections.Generic;
using HFPS.Player;
using HFPS.Systems;
using UnityEngine;

namespace NoManual.StateMachine
{
    public class BlinkEyeState : BaseState
    {
        private const float _VIGNETTE_MAX_VALUE = 1f;
        private const float _CHROMATIC_MAX_VALUE = 1f;
        private const float _BLINK_EYE_IN_SPEED = 5f;
        private const float _BLINK_EYE_OUT_SPEED = 5f;

        private Coroutine _eyeOpenProcess = null;
        
        public override void OnEnterState()
        {
            if (_eyeOpenProcess != null)
            {
                Managers.NoManualHotelManager.Instance.CoroutineManager.StopCoroutine(_eyeOpenProcess);
            }
            Managers.NoManualHotelManager.Instance.CoroutineManager.StartCoroutineProcess(CloseEyeProcess());
        }

        public override void OnUpdateState()
        {
            
        }

        public override void OnFixedUpdateState()
        {
            
        }

        public override void OnExitState()
        {
            _eyeOpenProcess = Managers.NoManualHotelManager.Instance.CoroutineManager.StartCoroutineProcess(OpenEyeProcess());
        }

        private IEnumerator OpenEyeProcess()
        {
            JumpscareEffects effect = ScriptManager.Instance.C<JumpscareEffects>();
            bool isDone = false;
            
            while (!isDone)
            {
               isDone = effect.BlinkEyeEffect(50f, 0f, 0f, 5f, 5f, 50f);
               
               yield return null;
            }
        }
        
        private IEnumerator CloseEyeProcess()
        {
            JumpscareEffects effect = ScriptManager.Instance.C<JumpscareEffects>();
            
            while (PlayerController.Instance.isBlinkEye)
            {
                effect.BlinkEyeEffect(0f, 1f, 1f, 300f , 5f, 10f);

                yield return null;
            }
            
            PlayerController.Instance.SpecialStateMachine.ChangeState(PlayerSpecialState.Idle);
        }
    }
}


