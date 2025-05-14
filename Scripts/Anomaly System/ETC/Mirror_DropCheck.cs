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

        // audioSource �� ���� 
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
        // floor�� �浹 �� dropSfx ���
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
        // �÷��̾ Ư�� ���̾ ���� �� treadSfx ���
        int playerLayer = LayerMask.NameToLayer("Ignore Raycast"); // "Player" ���̾� ��ȣ ��������

        if (other.gameObject.layer == playerLayer)
        {
            audioSource.clip = treadSfx[Random.Range(0, treadSfx.Length)];
            audioSource.Play();
        }
    }
}