using NoManual.Managers;
using UnityEngine;

public class ANO_DropSound : MonoBehaviour
{
    private AudioClip dropSfx;
    private AudioSource dropSfxAudioSource;

    private void Start()
    {
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = false;
    }


    /// <summary>
    /// DropSound의 Initialize 호출 시 자동설정됨
    /// </summary>
    /// <param name="sfx"></param>
    public void Initialize(AudioClip sfx)
    {
        dropSfx = sfx;
        dropSfxAudioSource = gameObject.AddComponent<AudioSource>();
        dropSfxAudioSource.clip = dropSfx;
        dropSfxAudioSource.playOnAwake = false;
        dropSfxAudioSource.spatialBlend = 1.0f;
        dropSfxAudioSource.rolloffMode = AudioRolloffMode.Linear;
        dropSfxAudioSource.minDistance = 1;
        dropSfxAudioSource.maxDistance = 10;
        dropSfxAudioSource.outputAudioMixerGroup = NoManualHotelManager.Instance.AudioManager.GetAudioMixerGroup(OptionHandler.AudioMixerChanel.SFX);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // "Floor" 태그가 있는 오브젝트와 충돌 시 소리 재생
        if (collision.gameObject.CompareTag("Floor"))
        {
            dropSfxAudioSource.Play(); // 소리 재생
            Invoke("RigidKinematic", 1f); // RigidKinematic 활성화 딜레이
        }
    }

    private void RigidKinematic()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }
}