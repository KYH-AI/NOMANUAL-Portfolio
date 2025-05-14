using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeachBall_DropCheck : MonoBehaviour
{
    [SerializeField] private AudioSource dropSfx;   // 충돌 시 재생할 효과음

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))  // 바닥과 충돌하면
        {
            if (dropSfx != null)
            {
                dropSfx.Play();  // 효과음 재생
                Debug.Log("BeachBall이 바닥과 충돌: 효과음 재생됨");
            }
        }
    }
}