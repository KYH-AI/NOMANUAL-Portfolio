using System;
using System.Collections;
using System.Collections.Generic;
using NoManual.Managers;
using UnityEngine;

namespace NoManual.ANO
{
    public abstract class ANO_BaseModule : MonoBehaviour
    {
        public event Action<Collider> ANO_StartTriggerCompleteEvent;
        
        [SerializeField] private bool autoInit;

        public bool AutoInit => autoInit;
        public bool InitState { get; private set; } = false;

        [Space(10)]
        [SerializeField] protected Collider anoStartCollider;
        [SerializeField] private bool anoStartColliderAutoDisable;
        public Collider GetAnoStartCollider => anoStartCollider;
        
        [Space(10)]
        [SerializeField] private bool useJumpScare;
        public bool UseJumpScare => useJumpScare;
        public ANO_JumpScare_Module jumpScareModule;
        
        [Space(10)]
        [SerializeField] private bool useDamage;
        public bool UseDamage => useDamage;
        [SerializeField] private int damageValue = 0;

        [Space(10)]
        [SerializeField] private bool useAudio;
        public bool UseAudio => useAudio;
        public ANO_Audio_Module audioModule;

        /// <summary>
        /// 자동 초기화
        /// </summary>
        protected void Awake()
        {
            if (autoInit || !InitState)
            {
                Init();
            }
        }

        /// <summary>
        /// 초기화
        /// </summary>
        public virtual void Init()
        {
            if (InitState) return;

            if (UseAudio && audioModule.audioSource)
            {
                audioModule.audioSource.clip = audioModule.audioClip;
                audioModule.audioSource.playOnAwake = false;
                audioModule.audioSource.spatialBlend = audioModule.audioBlend;
                audioModule.audioSource.rolloffMode = audioModule.audioRolloffMode;
                audioModule.audioSource.minDistance = audioModule.minVolumeDistance;
                audioModule.audioSource.maxDistance = audioModule.maxVolumeDistance;
                audioModule.audioSource.volume = audioModule.audioVolumeSize;
                audioModule.audioSource.outputAudioMixerGroup = NoManualHotelManager.Instance.AudioManager.GetAudioMixerGroup(OptionHandler.AudioMixerChanel.SFX);
                if (audioModule.oneShot)
                {
                    audioModule.audioSource.loop = false;
                }
                else
                {
                    audioModule.audioSource.loop = true;    
                }
            }
            InitState = true;
        }
        
        /// <summary>
        /// ANO START 실행
        /// </summary>
        /// <param name="anoTriggerZone">ANO Start</param>
        public abstract void Run(Collider anoTriggerZone);
        

        /// <summary>
        /// ANO START 및 오디오 소스 중지
        /// </summary>
        private void StopAnoStart()
        {
            if (anoStartColliderAutoDisable && anoStartCollider)
            {
                if (ANO_StartTriggerCompleteEvent == null)
                {
                    anoStartCollider.enabled = false;
                }
                else
                {
                    ANO_StartTriggerCompleteEvent?.Invoke(anoStartCollider);
                }
            }
        }

        /// <summary>
        /// ANO 트리거 정지 및 오디오 소스 중지
        /// </summary>
        /// <param name="audioStop">오디오 정지?</param>
        public virtual void Stop(bool audioStop)
        {
            if (audioStop)
            {
                if (useAudio && audioModule.audioSource)
                {
                    audioModule.audioSource.Stop();
                }
            }
            StopAnoStart();
        }


        /// <summary>
        /// 소리 재생
        /// </summary>
        /// <param name="audioClip">커스텀 오디오 클립</param>
        public virtual void PlayAudio(AudioClip audioClip = null)
        {
            AudioClip clip = audioClip;
            
            if (!clip)
            {
                clip = audioModule.audioClip;
            }
     

            if (audioModule.loop)
            {
                audioModule.audioSource.Play();
            }
            else
            {
                audioModule.audioSource.PlayOneShot(clip);
            }
        }
        

        /// <summary>
        /// 점프 스케어
        /// </summary>
        public virtual void JumpScare()
        {
            if(useJumpScare && jumpScareModule != null)
                NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(jumpScareModule.jumpScareLevel, jumpScareModule.jumpScareDelay);
        }

        /// <summary>
        /// 정신력 공격
        /// </summary>
        public virtual void GiveDamage()
        {
            if(useDamage && damageValue > 0f) 
               HFPS.Player.PlayerController.Instance.DecreaseMentality(damageValue);
        }
        

        private void OnDestroy()
        {
            ANO_StartTriggerCompleteEvent = null;
        }
    }
}


