using System;
using HFPS.Player;
using NoManual.ANO;
using NoManual.Managers;
using UnityEngine;

public class ANO34_OpenDoor2 : ANO_Component
{
     [Header("ANO 설정")]
    [SerializeField] private Collider anoStart;
    [SerializeField] private Collider anoPlaceTeleport;
    [SerializeField] private Animator anoDoor;
    [SerializeField] private Transform anoPlaceStart;
    [SerializeField] private Transform anoPlaceEnd;

    [Header("ANO Place 설정")]
    [SerializeField] private Collider anoBodiesStart;
    [SerializeField] private GameObject[] anoBodies;
    [SerializeField] private AudioSource doorOpenSfx;
    [SerializeField] private AudioSource anoSfx0;
    [SerializeField] private AudioSource anoSfx1;
    [SerializeField] private Transform CamFollowPos;
    [SerializeField] private float volumeChangeSpeed = 0.5f; // 볼륨 조절 속도
    
    private bool isRaycasting = true;
    private bool isTeleporting = false;
    private float teleportRaycastTime = 0f;
    private float maxVolume = 1f;
    private float rotationSpeed = 1f;
    private bool isSfx0Played = false;
    private Ray ray;

    private void HandleRaycast()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit) && hit.collider == anoPlaceTeleport)
        {
            if (!isSfx0Played)
            {
                isSfx0Played = true;
            }
            
            teleportRaycastTime += Time.deltaTime;
            anoSfx0.volume = Mathf.Min(anoSfx0.volume + volumeChangeSpeed * Time.deltaTime, maxVolume);
            
            
            // anoPlace 진입
            if (teleportRaycastTime >= 1.5f && !isTeleporting)
            {
                isRaycasting = false;
                isTeleporting = true;
                PlayerController.Instance.transform.position = anoPlaceStart.position;
                
                // 시선 고정
                PlayerAPI.SetFocusCameraTarget(CamFollowPos);    
                
            }
        }
        else
        {
            teleportRaycastTime = 0f;
            anoSfx0.volume = Mathf.Max(anoSfx0.volume - volumeChangeSpeed * Time.deltaTime, 0f);
        }
    }

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            // 문 열기
            anoDoor.SetTrigger("Open");
            doorOpenSfx.Play();
            anoStart.enabled = false;
        }
        else if (anoTriggerZone == anoBodiesStart)
        {
                anoSfx1.Play();
                anoBodiesStart.enabled = false;


            // anoBodies 하위 오브젝트 Rigidbody 활성화
            foreach (var body in anoBodies)
            {
                Rigidbody rb = body.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                }
            }
            Invoke(nameof(TriggerJumpScare), 1.5f);
        }
    }

    private void TriggerJumpScare()
    {
        NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(3);
        PlayerController.Instance.DecreaseMentality(10);
        PlayerController.Instance.transform.position = anoPlaceEnd.position;
        Destroy(gameObject);
    }
    
    private void Update()
    {
        if(isRaycasting)  HandleRaycast();
        else if(isTeleporting)
        {
            // anoPlace 이벤트 종료 지점
            if (PlayerController.Instance.transform.position == anoPlaceEnd.position)
            {
                anoSfx0.Stop();
                PlayerAPI.ResetFocusCameraTarget();
                isTeleporting = false;
            }
        }
    }
}

