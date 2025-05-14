using System.Collections;
using System.Collections.Generic;
using HFPS.Player;
using HFPS.Systems;
using NoManual.Managers;
using NoManual.StateMachine;
using UnityEngine;

namespace NoManual.StateMachine
{
    public class Exhausted : BaseState
    {
        private readonly float EXHAUSTED_TIME;
        private float _exhaustedTimer = 0f;

        private Coroutine _inExhaustedProcess = null;
        private Coroutine _outExhaustedProcess = null;


        public Exhausted(float exhaustedTime)
        {
            this.EXHAUSTED_TIME = exhaustedTime;
        }

        public override void OnEnterState()
        {
            _exhaustedTimer = 0f;

            if (_outExhaustedProcess != null)
            {
                Managers.NoManualHotelManager.Instance.CoroutineManager.StopCoroutine(_outExhaustedProcess);
                _outExhaustedProcess = null;
            }

            if (_inExhaustedProcess != null)
            {
                Managers.NoManualHotelManager.Instance.CoroutineManager.StopCoroutine(_inExhaustedProcess);
                _inExhaustedProcess = null;
            }
            
            ScriptManager.Instance.C<JumpscareEffects>().ChangeArmsCamVolumeProfile(JumpscareEffects.ArmsCamVolumeProfileType.Exhausted);

            // 탈진 Start SFX 실행
            NoManualHotelManager.Instance.AudioManager.PlaySFX(AudioManager.SFX_Audio_List.Player, SfxEnum.ExhasutedBreath, false);
            PlayerController.Instance.cameraHeadBob.cameraStateType = PlayerController.CameraHeadBob.CameraStateType.Exhausted;
            _inExhaustedProcess = Managers.NoManualHotelManager.Instance.CoroutineManager.StartCoroutineProcess(InExhaustedEffect());
        }

        public override void OnUpdateState()
        {
  
        }

        public override void OnFixedUpdateState()
        {

        }

        public override void OnExitState()
        {
            _outExhaustedProcess = Managers.NoManualHotelManager.Instance.CoroutineManager.StartCoroutineProcess(OutExhaustedEffect());
        }

        public void ExitExhausted()
        {
            if (_inExhaustedProcess != null)
            {
                Managers.NoManualHotelManager.Instance.CoroutineManager.StopCoroutine(_inExhaustedProcess);
            }
           
            // 탈진 Stop SFX
            PlayerController.Instance.cameraHeadBob.cameraStateType = PlayerController.CameraHeadBob.CameraStateType.Idle;
            PlayerController.Instance.SpecialStateMachine.ChangeState(PlayerSpecialState.Idle);
        }

        private IEnumerator InExhaustedEffect()
        {
            JumpscareEffects effect = ScriptManager.Instance.C<JumpscareEffects>();

            while (true)
            {
                effect.InExhaustedEffect();
                yield return null;
            }
        }

        private IEnumerator OutExhaustedEffect()
        {
            JumpscareEffects effect = ScriptManager.Instance.C<JumpscareEffects>();
            bool isDone = false;
            while (!isDone)
            {
                isDone = effect.OutExhaustedEffect();
                yield return null;
            }

            effect.ChangeArmsCamVolumeProfile(JumpscareEffects.ArmsCamVolumeProfileType.Default);

        }
    }
}
