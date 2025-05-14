using System;
using System.Collections;
using UnityEngine;
using HFPS.Player;
using NoManual.JumpScare;
using NoManual.Utils;
using ThunderWire.Utility;


namespace NoManual.Managers
{
    public class JumpScareManager : MonoBehaviour
    {
        // Jump Scare DB
        [Header("Jump Scare 원본 데이터")]
        [SerializeField] private JumpScareDataBaseScriptable jumpScareDataBase;

        #region Jump Scare 세팅 값
        
        private JumpscareEffects _effects;
        private Transform _playerTransform;
        
        [ HideInInspector]
        public bool isPlayed;

        #endregion

        private void Awake()
        {
            this._effects = HFPS.Systems.ScriptManager.Instance.C<JumpscareEffects>();;
            this._playerTransform = PlayerController.Instance.transform;
        }
        

        /// <summary>
        /// 저장된 Jump Scare ID로 효과 생성
        /// </summary>
        private CameraShakeInstance CreateCameraShakePreset(int jumpScareId)
        {
            JumpScareCloneData jumpScareCloneData = GetJumScareCloneDataToJumpScareId(jumpScareId);

            if (jumpScareCloneData == null) return null;
           
           CameraShakeInstance shakeInstance = new CameraShakeInstance(jumpScareCloneData.scareShake.magnitude,
                                                                           jumpScareCloneData.scareShake.roughness,
                                                                           jumpScareCloneData.scareShake.startTime,
                                                                           jumpScareCloneData.scareShake.durationTime);

           // 위치 영향이 -1인 경우에는 positionInfluence 값을 그대로 사용하고,
           // 그렇지 않은 경우에는 positionInfluence 값에 positionInfluenceMux 값을 곱하여 사용
           shakeInstance.PositionInfluence = jumpScareCloneData.scarePositionInfluence.positionInfluenceMux == -1 ? 
                                             jumpScareCloneData.scarePositionInfluence.positionInfluence :
                                             jumpScareCloneData.scarePositionInfluence.positionInfluence * jumpScareCloneData.scarePositionInfluence.positionInfluenceMux;
           shakeInstance.RotationInfluence = jumpScareCloneData.scarePositionInfluence.rotationInfluence;

           return shakeInstance;
        }
        
        /// <summary>
        /// 저장된 Jump Scare 모듈로 효과 생성
        /// </summary>
        private CameraShakeInstance CreateCameraShakePreset(JumpScareCloneData jumpScareCloneData)
        {
            if (jumpScareCloneData == null) return null;
           
            CameraShakeInstance shakeInstance = new CameraShakeInstance(jumpScareCloneData.scareShake.magnitude, 
                                                                        jumpScareCloneData.scareShake.roughness, 
                                                                jumpScareCloneData.scareShake.startTime,
                                                                        jumpScareCloneData.scareShake.durationTime);
            // 위치 영향이 -1인 경우에는 positionInfluence 값을 그대로 사용하고,
            // 그렇지 않은 경우에는 positionInfluence 값에 positionInfluenceMux 값을 곱하여 사용
            shakeInstance.PositionInfluence = jumpScareCloneData.scarePositionInfluence.positionInfluenceMux == -1 ? 
                                                jumpScareCloneData.scarePositionInfluence.positionInfluence :
                                                jumpScareCloneData.scarePositionInfluence.positionInfluence * jumpScareCloneData.scarePositionInfluence.positionInfluenceMux;
            shakeInstance.RotationInfluence = jumpScareCloneData.scarePositionInfluence.rotationInfluence;

            return shakeInstance;
        }

        /// <summary>
        /// Jump Scare ID로 정보 얻기
        /// </summary>
        private JumpScareCloneData GetJumScareCloneDataToJumpScareId(int jumpScareId)
        {
            JumpScareScriptable jumpScareData = jumpScareDataBase.GetJumpScareDataToId(jumpScareId);
            if (jumpScareData == null)
            {
                ErrorCode.SendError(ErrorCode.ErrorCodeEnum.GetJumpScare);
                return null;
            }
            
            JumpScareCloneData jumpScareCloneData = new JumpScareCloneData(jumpScareData);
            return jumpScareCloneData;
        }

        /// <summary>
        /// Jump Scare ID로 효과 실행
        /// </summary>
        public void PlayJumpScareEffectToJumpScareId(int jumpScareId, float delayTimer = 0f)
        {
            JumpScareCloneData jumpScareCloneData = GetJumScareCloneDataToJumpScareId(jumpScareId);
            CameraShakeInstance jumpScareInstance = CreateCameraShakePreset(jumpScareCloneData);

            // Jump Scare 효과 딜레이 적용
            if (delayTimer != 0f)
            {
                StartCoroutine(PlayJumpScareEffectToDelay(jumpScareCloneData, jumpScareInstance, delayTimer));
            }
            // Jump Scare 효과 딜레이 적용 X
            else
            {
                PlayJumpScareEffect(jumpScareCloneData, jumpScareInstance);
            }
        }
        
        /// <summary>
        /// Jump Scare 실행
        /// </summary>
        private void PlayJumpScareEffect(JumpScareCloneData jumpScareCloneData, CameraShakeInstance jumpScareInstance)
        {
            // Jump Scare 전용 사운드가 있으면 실행
            if (jumpScareCloneData.jumpScareSound.jumpScareSound)
            {
                PlayJumpScareSound(jumpScareCloneData.jumpScareSound.jumpScareSound, jumpScareCloneData.jumpScareSound.scareVolume);
            }
            
            // Jump Scare 이펙스 실행
            _effects.Scare(jumpScareInstance, 
                jumpScareCloneData.scareEffects.chromaticAberrationAmount,
                jumpScareCloneData.scareEffects.vignetteAmount,
                jumpScareCloneData.jumpScareSound.scaredBreathTime,
                jumpScareCloneData.scareEffects.effectsTime,
                jumpScareCloneData.jumpScareSound.scaredBreath,
                jumpScareCloneData.scareEffectsSpeed.scareEffectSpeed,
                jumpScareCloneData.scareEffectsSpeed.chromaticOutSpeed,
                jumpScareCloneData.scareEffectsSpeed.vignetteOutSpeed);
        }
        
        /// <summary>
        /// Jump Scare 딜레이 후 실행
        /// </summary>
        private IEnumerator PlayJumpScareEffectToDelay(JumpScareCloneData jumpScareCloneData, CameraShakeInstance jumpScareInstance, float delayTimer)
        {
            yield return new WaitForSeconds(delayTimer);
            PlayJumpScareEffect(jumpScareCloneData, jumpScareInstance);
        }

        /// <summary>
        /// Jump Scare Sound 실행
        /// </summary>
        private void PlayJumpScareSound(AudioClip audioClip, float scareVolume)
        {
            Utilities.PlayOneShot2D(_playerTransform.position, OptionHandler.AudioMixerChanel.SFX, audioClip, scareVolume);
        }
    }
    
    /// <summary>
    /// Jump Scare 복사본
    /// </summary>
    public class JumpScareCloneData
    {
        public int jumpScareId;
        public JumpScareScriptable.ScarePositionInfluence scarePositionInfluence;
        public JumpScareScriptable.ScareEffects scareEffects;
        public JumpScareScriptable.ScareEffects.ScareEffectsSpeed scareEffectsSpeed;
        public JumpScareScriptable.ScareShake scareShake;
        public JumpScareScriptable.JumpScareSound jumpScareSound;
        
        public JumpScareCloneData(JumpScareScriptable jumpScareScriptable)
        {
            this.jumpScareId = jumpScareScriptable.jumpScareId;
            this.scarePositionInfluence = jumpScareScriptable.scarePositionInfluence;
            this.scareEffects = jumpScareScriptable.scareEffects;
            this.scareEffectsSpeed = jumpScareScriptable.scareEffects.scareEffectsSpeed;
            this.scareShake = jumpScareScriptable.scareShake;
            this.jumpScareSound = jumpScareScriptable.jumpScareSound;
        }
    }
}


