using UnityEngine;

public class Megan_DropCheck : MonoBehaviour
{
    [Header("Audio Settings")] [SerializeField]
    private AudioSource audioSource; 

    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트가 "Floor" 태그를 가지고 있는지 확인
        if (collision.gameObject.CompareTag("Floor"))
        {
            // 사운드 재생
            audioSource.Play();
        }
    }
}