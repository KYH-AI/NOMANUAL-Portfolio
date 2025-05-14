using UnityEngine;
using Random = UnityEngine.Random;

namespace NoManual.Creature
{
    public class CreatureAudioHandler : MonoBehaviour
    {
        [Space(10)] 
        [SerializeField] private AudioSource footStepAudioSource;
        [SerializeField] private AudioClip[] footStepList;
        
        [Space(5)]
        [SerializeField] private AudioSource voiceAudioSource;
        [Space(5)]
        [SerializeField] private AudioSource deadAreaAudioSource;

        /// <summary>
        /// 발소리 애니메이션 이벤트 호출 (상시)
        /// </summary>
        public void PlayCreatureFootStepAudio()
        {
            footStepAudioSource.PlayOneShot(footStepList[Random.Range(0, footStepList.Length)], 1f);
        }

        /// <summary>
        /// 추적 시작
        /// </summary>
        public void PlayCreatureChaseAudioSet(SfxEnum voiceSfx)
        {
            PlayDeadAreaAudio();
            PlayCreatureVoiceAudio(voiceSfx);
        }

        /// <summary>
        /// 추적 종료
        /// </summary>
        public void StopCreatureChaseAudioSet()
        {
            StopDeadAreaAudio();
            StopCreatureVoiceAudio();
        }
        
        /// <summary>
        /// 크리처 목소리 SFX 실행
        /// </summary>
        public void PlayCreatureVoiceAudio(SfxEnum voiceSfx, bool isLoop = true)
        {
            var voiceClip = Managers.NoManualHotelManager.Instance.AudioManager.GetAudioClip(voiceSfx);
            if (voiceAudioSource.clip == voiceClip) return;
            
            StopCreatureVoiceAudio();
            voiceAudioSource.loop = isLoop;
            voiceAudioSource.clip = Managers.NoManualHotelManager.Instance.AudioManager.GetAudioClip(voiceSfx);
            voiceAudioSource.Play();
        }
        
        public void StopCreatureVoiceAudio()
        {
            voiceAudioSource.Stop();
        }
        
       // private void 

        /// <summary>
        /// 크리처 Dead 오디오 SFX 실행
        /// </summary>
        private void PlayDeadAreaAudio()
        {
            deadAreaAudioSource.loop = true;
            deadAreaAudioSource.Play();
        }

        private void StopDeadAreaAudio()
        {
            deadAreaAudioSource.Stop();
        }
    }

}

