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
    /// DropSound�� Initialize ȣ�� �� �ڵ�������
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
        // "Floor" �±װ� �ִ� ������Ʈ�� �浹 �� �Ҹ� ���
        if (collision.gameObject.CompareTag("Floor"))
        {
            dropSfxAudioSource.Play(); // �Ҹ� ���
            Invoke("RigidKinematic", 1f); // RigidKinematic Ȱ��ȭ ������
        }
    }

    private void RigidKinematic()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }
}