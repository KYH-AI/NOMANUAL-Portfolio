using UnityEngine;

public class Statue_DropCheck : MonoBehaviour
{

    // Statue Broken Sfx를 1번만 실행하기 위한 변수
    private bool isSfxPlayed = false;
    
    public AudioSource StatueBrokenSfx;

    // 땅과 충돌 여부 검사. collider
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            Debug.Log("스태츄가 땅과 충돌함!");
            // 자식 오브젝트에게 Rigidbody 붙이기 실행
            AddRigidbodyToChildren();
            
            // 수정 요망. 재사용 시 플레이가 안되나?
            if (!isSfxPlayed)
            {
                StatueBrokenSfx.Play();
                isSfxPlayed = true;
            }
        }
    }

    // 모든 자식 오브젝트들에게 Rigidbody를 추가함
    private void AddRigidbodyToChildren()
    {
        foreach (Transform child in transform)
        {
            Rigidbody rb;
            if (child.GetComponent<Rigidbody>() == null)
            {
                rb = child.gameObject.AddComponent<Rigidbody>();
            }
            else
            {
                rb = child.gameObject.GetComponent<Rigidbody>();
            }

            // Rigidbody 속성 조절
            rb.mass = 1000.0f;        // 질량 조절
            rb.drag = 0.01f;        // 저항력 조절
            rb.angularDrag = 1.0f; // 회전 저항력 조절
        }
    }
}

