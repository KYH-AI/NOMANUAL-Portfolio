using System.Collections;
using NoManual.ANO;
using UnityEngine;

// 현재 Neckman의 로직을 복붙한 상태. 장소 이동만 참고하자. 
public class ANO59_OpenDoor3 : ANO_Component
{
    [SerializeField] private Collider anoStart;
    [SerializeField] private Animator anoDoor; // 문 애니메이션
    [SerializeField] private AudioSource doorOpenSfx;
    [SerializeField] private Collider doorCloseCollider;
    [SerializeField] private AudioSource doorCloseSfx;
    [SerializeField] private Collider teleportCollider; // 플레이어가 들어가야 할 콜라이더
    [SerializeField] private Transform anoPlaceStart; // 이동할 위치

    [Space(30)]
    [SerializeField] private AudioSource anoPlaceSfx1; // 사운드 효과 1
    [SerializeField] private AudioSource anoPlaceSfx2; // 사운드 효과 2
    [SerializeField] private AudioSource anoPlaceSfx3; 
    [SerializeField] private Collider deadCollider; // 죽음 관련 콜라이더
    
    private Transform playerPosition;

    private void Start()
    { 
        playerPosition = HFPS.Player.PlayerController.Instance.transform; // 플레이어 위치 가져오기
    }

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            if (anoDoor != null)
            {
                anoDoor.SetTrigger("Open");
                doorOpenSfx.Play();
                anoStart.enabled = false;
            }
        }

        if (anoTriggerZone == doorCloseCollider)
        {
            anoDoor.SetTrigger("Close");
            doorCloseSfx.Play();
            doorCloseCollider.enabled = false;

        }
        
        if (anoTriggerZone == teleportCollider)
        {
            // 플레이어를 anoPlaceStart로 이동
            playerPosition.position = anoPlaceStart.position;
            playerPosition.rotation = anoPlaceStart.rotation;
            
            // 소리 재생
            anoPlaceSfx1.Play();
            anoPlaceSfx2.Play();
            anoPlaceSfx3.Play();
        }

        if (anoTriggerZone == deadCollider)
        {
            // 정신적 피해 100
            anoPlaceSfx1.Stop();
            anoPlaceSfx2.Stop();
            anoPlaceSfx3.Stop();
            HFPS.Player.PlayerController.Instance.DecreaseMentality(100);
        }
    }
}