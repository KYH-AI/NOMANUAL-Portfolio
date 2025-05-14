using System;
using NoManual.Managers;
using UnityEngine;

public class ANOPic_Drop : MonoBehaviour
{
    [SerializeField] private AudioClip[] possibleSounds;
    private bool isSfxPlayed = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor") && !isSfxPlayed)
        {
            PlayRandomSound();
        }
    }

    void PlayRandomSound()
    {
        if (possibleSounds.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, possibleSounds.Length);
            
            // 1. Audio Source 컴포넌트 부착
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            
            // 2. 랜덤한 clip 삽입
            audioSource.clip = possibleSounds[randomIndex];
            
            // 3. audioSource 속성 중 Spatial Blend 값을 3D로 변경 
            audioSource.spatialBlend = 1f;
            
            // 4. 3D 최대로 들리는 거리 (Max Distance) 값을 7로 변경
            audioSource.maxDistance = 7f;
            
            // 5. (24.04.05) audioSource audio mixer SFX 할당
            audioSource.outputAudioMixerGroup =
                NoManualHotelManager.Instance.AudioManager.GetAudioMixerGroup(OptionHandler.AudioMixerChanel.SFX);
            
            // 오디오 플레이
            audioSource.Play();
        }
    }
}
