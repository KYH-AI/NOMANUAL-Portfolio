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
        /// �ڵ� �ʱ�ȭ
        /// </summary>
        protected void Awake()
        {
            if (autoInit || !InitState)
            {
                Init();
            }
        }

        /// <summary>
        /// �ʱ�ȭ
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
        /// ANO START ����
        /// </summary>
        /// <param name="anoTriggerZone">ANO Start</param>
        public abstract void Run(Collider anoTriggerZone);
        

        /// <summary>
        /// ANO START �� ����� �ҽ� ����
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
        /// ANO Ʈ���� ���� �� ����� �ҽ� ����
        /// </summary>
        /// <param name="audioStop">����� ����?</param>
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
        /// �Ҹ� ���
        /// </summary>
        /// <param name="audioClip">Ŀ���� ����� Ŭ��</param>
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
        /// ���� ���ɾ�
        /// </summary>
        public virtual void JumpScare()
        {
            if(useJumpScare && jumpScareModule != null)
                NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(jumpScareModule.jumpScareLevel, jumpScareModule.jumpScareDelay);
        }

        /// <summary>
        /// ���ŷ� ����
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


