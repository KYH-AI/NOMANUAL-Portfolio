using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HFPS.Player;
using NoManual.ANO;

public class GLMan_DropCheck : ANO_Component
{
    [Header("Head Collider 설정")]
    [SerializeField] private AudioSource anoSfx; // 충돌 시 재생할 효과음

    private bool hasCollided = false; // 중복 재생 방지 플래그

    private void OnCollisionEnter(Collision collision)
    {
        // Floor와 충돌한 경우만 처리
        if (collision.gameObject.CompareTag("Floor") && !hasCollided)
        {
            hasCollided = true; // 중복 재생 방지

            // 효과음 재생
            if (anoSfx != null)
            {
                anoSfx.Play();
            }

            // 플레이어에게 데미지 적용
            PlayerController.Instance.DecreaseMentality(AnoCloneData.MentalityDamage);

            // Jumpscare 연출
            // 여기에 Jumpscare 연출 코드 추가
        }
    }
}