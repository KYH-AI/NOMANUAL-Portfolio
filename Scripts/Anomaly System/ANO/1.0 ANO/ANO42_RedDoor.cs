using System.Collections;
using UnityEngine;
using NoManual.ANO;
using NoManual.Managers;

public class ANO42_RedDoor : ANO_Component
{
    [Header("ANO 설정")]
    [SerializeField] private Collider[] anoObjs; // 유저가 보면 이동할 오브젝트
    [SerializeField] private float raycastDistance = 5f; // 레이캐스트 최대 거리

    [Header("ANO Place 관련")]
    [SerializeField] private Transform anoPlaceStartPos; // 플레이어가 이동할 위치
    [SerializeField] private GameObject firstExitObj;
    [SerializeField] private AudioSource anoSpaceBGM;
    [SerializeField] private AudioSource anoSpaceAlarm;
    [SerializeField] private Collider anoPlaceExit;
    [SerializeField] private Transform anoEndPos; // 플레이어가 돌아올 위치

    private Transform playerTransform;
    private Camera playerCam;
    private bool isInAnoPlace = false; // 플레이어가 ANO 공간에 있는지 여부
    private Coroutine damageCoroutine; // 데미지 코루틴 참조
    
    // 레이와 히트 결과 캐싱
    private Ray ray;
    private RaycastHit hit;

    private void Start()
    {
        // 플레이어 트랜스폼 및 카메라 초기화
        playerTransform = HFPS.Player.PlayerController.Instance.transform;
        playerCam = HFPS.Systems.ScriptManager.Instance.MainCamera;

        // 레이캐스트 감지 코루틴 시작
        StartCoroutine(CheckForLook());
    }

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoPlaceExit && isInAnoPlace)
        {
            playerTransform.position = anoEndPos.position;
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine); // 데미지 코루틴 중지
            }
            isInAnoPlace = false;
            
            anoSpaceBGM.Stop();
            anoSpaceAlarm.Stop();
            
            DisableAllColliders(anoPlaceExit);
            DeleteAllANOObjs(anoObjs);
        }
    }

    private void DisableAllColliders(Collider collider)
    {
        collider.enabled = false;
    }

    private IEnumerator CheckForLook()
    {
        // 유저가 anoObj 중 어느 하나라도 보고 있는지 판단
        while (true)
        {
            foreach (Collider obj in anoObjs)
            {
                Vector3 direction = (obj.transform.position - playerCam.transform.position).normalized;
                ray.origin = playerCam.transform.position;
                ray.direction = direction; 

                if (Physics.Raycast(ray, out hit, raycastDistance))
                {
                    if (hit.collider == obj)
                    {
                        // 플레이어 위치를 저장하고 이동
                        Vector3 originalPosition = playerTransform.position;
                        playerTransform.position = anoPlaceStartPos.position;

                        // 카메라 고정 후 즉시 해제
                        PlayerAPI.SetFocusCameraTarget(firstExitObj.transform);
                        yield return new WaitForSeconds(0.5f);
                        PlayerAPI.ResetFocusCameraTarget();

                        // 사운드 재생
                        anoSpaceBGM.Play();
                        anoSpaceAlarm.Play();

                        // 점프스케어 2
                        NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(2, 0f);

                        isInAnoPlace = true;

                        // 5초마다 멘탈 데미지 주기 시작
                        damageCoroutine = StartCoroutine(ApplyDamageOverTime());

                        yield break; // 레이캐스트 감지 중단
                    }
                }
            }
            yield return null; // 다음 프레임까지 대기
        }
    }

    private IEnumerator ApplyDamageOverTime()
    {
        while (isInAnoPlace)
        {
            HFPS.Player.PlayerController.Instance.DecreaseMentality(10);
            yield return new WaitForSeconds(3f);
        }
    }
    
    // anoObjs 배열에 있는 모든 오브젝트를 삭제
    void DeleteAllANOObjs(Collider[] anoObj)
    {
        foreach (Collider obj in anoObj)
        {
            if (obj != null) 
            {
                Destroy(obj.gameObject);
            }
        }
    }
}
