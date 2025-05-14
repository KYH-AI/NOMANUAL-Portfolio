using UnityEngine;

public class Megan_DropCheck : MonoBehaviour
{
    [Header("Audio Settings")] [SerializeField]
    private AudioSource audioSource; 

    private void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� "Floor" �±׸� ������ �ִ��� Ȯ��
        if (collision.gameObject.CompareTag("Floor"))
        {
            // ���� ���
            audioSource.Play();
        }
    }
}