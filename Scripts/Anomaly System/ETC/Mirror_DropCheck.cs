using UnityEngine;

public class MirrorDropCheck : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip[] dropSfx;
    private AudioClip[] treadSfx;
    private bool isPlayed = false;


    public void InitMirror(AudioClip[] dropSfx, AudioClip[] treadSfx)
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;

        // audioSource 값 설정 
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 10f;
        audioSource.volume = 0.5f;

        this.dropSfx = dropSfx;
        this.treadSfx = treadSfx;
    }


    private void OnCollisionEnter(Collision collision)
    {
        // floor와 충돌 시 dropSfx 재생
        if (collision.gameObject.CompareTag("Floor") && !isPlayed)
        {
            audioSource.clip = dropSfx[Random.Range(0, dropSfx.Length)];
            audioSource.Play();
            Invoke(nameof(RigidbodyKinematic), 1.5f);
            isPlayed = true;
        }
    }

    private void RigidbodyKinematic()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 특정 레이어에 있을 때 treadSfx 재생
        int playerLayer = LayerMask.NameToLayer("Ignore Raycast"); // "Player" 레이어 번호 가져오기

        if (other.gameObject.layer == playerLayer)
        {
            audioSource.clip = treadSfx[Random.Range(0, treadSfx.Length)];
            audioSource.Play();
        }
    }
}