using System;
using System.Collections;
using System.Collections.Generic;
using NoManual.ANO;
using UnityEngine;

public class ANO55_NeckMan3 : ANO_Component
{
    [SerializeField] private GameObject neckMan;
    [SerializeField] private Collider forceNeckManLook;
    [SerializeField] private AudioSource duringLookSound;
    [SerializeField] private Transform anoPlaceStart;
    // anoPlace이 시작됐는지 검사하는 콜라이더, setfocusCamera 때문에 추가됨
    [SerializeField] private Collider anoPlaceStartCollider;
    [Space(30)]
    [SerializeField] private GameObject runningNeckMan;
    [SerializeField] private AudioSource anoPlaceSfx1;
    [SerializeField] private AudioSource anoPlaceSfx2;
    [SerializeField] private Collider neckManCollider;
    
    [Space(30)]
    [SerializeField] private Collider anoPlaceEnd;
    [SerializeField] private Transform anoEndPos;

    [Space(30)]
    [Header("복도 불빛 트리거")]
    [SerializeField] private Collider[] corridorLightsCollider;
    [SerializeField] private GameObject[] corridorLights;
    [SerializeField] private AudioSource corridorLightSound;
    
    private bool isRaycasting = false;
    private bool isTeleporting = false;
    private float teleportRaycastTime = 0f;
    private float maxVolume = 1f;
    private Camera _playerCamera;
    private Transform playerPosition;

    private Dictionary<Collider, int> colliderToIndexMap = new Dictionary<Collider, int>();
    private HashSet<Collider> activatedColliders = new HashSet<Collider>(); // 이미 처리된 콜라이더 추적

    /*
     *  forceNeckManLook 콜라이더 충돌 시:              
      PlayerAPI.SetFocusCameraTarget(CamFollowPos);  
     *  forceNeckManLook 콜라이더 벗어날 때 PlayerAPI 리셋
     *
     * neckman 쳐다보는 동안:  
     *   - 5초 이상 쳐다보면 플레이어를 anoPlaceStart로 이동
     *   - 이동하기 전 player Position 저장
     *   - runningNeckMan 활성화
     *
     * neckManCollider 충돌 시:
     *   - Mentality Damage 100 적용
     * anoPlaceEnd 충돌 시:
     *   - 저장한 player Position으로 이동
     *   - neckMan 제거
     * 
     * 
     */

    private void Start()
    { 
        playerPosition = HFPS.Player.PlayerController.Instance.transform;
        _playerCamera = Camera.main;

        for (int i = 0; i < corridorLightsCollider.Length; i++)
        {
            if (i < corridorLightsCollider.Length)
            {
                colliderToIndexMap[corridorLightsCollider[i]] = i;
            }
        }

        // 모든 corridorLights를 꺼서 초기 상태로 설정
        foreach (var light in corridorLights)
        {
            if (light != null)
            {
                light.SetActive(false);
            }
        }
    }

    // ANO_TriggerCheck: 
    // 주어진 콜라이더가 특정 조건을 만족할 때 발생하는 이벤트를 처리합니다.
    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        // 이미 처리된 콜라이더는 무시
        if (activatedColliders.Contains(anoTriggerZone))
        {
            return;
        }

        // corridorLightsCollider 중 하나와 충돌 시 처리
        for (int i = 0; i < corridorLightsCollider.Length; i++)
        {
            // 충돌한 콜라이더 확인
            if (anoTriggerZone == corridorLightsCollider[i])
            {
                // 중복 처리 방지: 현재 콜라이더를 HashSet에 추가
                activatedColliders.Add(anoTriggerZone);

                // 해당하는 불빛의 인덱스 계산
                int lightIndex1 = 2 * i;     // 첫 번째 불빛 인덱스
                int lightIndex2 = 2 * i + 1; // 두 번째 불빛 인덱스

                // 배열 범위를 초과하지 않도록 안전하게 처리
                if (lightIndex1 < corridorLights.Length)
                {
                    corridorLights[lightIndex1].SetActive(true);
                }

                if (lightIndex2 < corridorLights.Length)
                {
                    corridorLights[lightIndex2].SetActive(true);
                }

                // 오디오 재생 (공통 오디오)
                PlaySoundOnce();

                break; // 처리 완료 후 루프 종료
            }
        }

        // forceNeckManLook 콜라이더와 충돌 시: 
        // 플레이어가 neckMan을 바라보는 동안 LockOffDelay와 LookAtNeckMan 코루틴 시작
        if (anoTriggerZone == forceNeckManLook && !isTeleporting)
        {
            StartCoroutine(LockOffDelay());
            StartCoroutine(LookAtNeckMan(playerPosition));
        }

        // anoPlaceStartCollider 콜라이더와 충돌 시:
        // LockOffDelay 코루틴 중지
        if (anoTriggerZone == anoPlaceStartCollider)
        {
            StopCoroutine(LockOffDelay());
        }

        // neckManCollider와 충돌 시:
        // 플레이어의 Mentality를 100 감소
        if (anoTriggerZone == neckManCollider)
        {
            HFPS.Player.PlayerController.Instance.DecreaseMentality(100);
        }

        // anoPlaceEnd와 충돌 시:
        // 플레이어를 anoEndPos 위치로 이동시키고 현재 게임 오브젝트를 파괴
        if (anoTriggerZone == anoPlaceEnd)
        {
            HFPS.Player.PlayerController.Instance.transform.position = anoEndPos.transform.position;
            Destroy(gameObject);
        }
    }

    // LookAtNeckMan: 
    // 플레이어가 neckMan을 바라보는 동안의 로직을 처리하며, 
    // 일정 시간 동안 바라보면 플레이어를 특정 위치로 이동시킵니다.
    private IEnumerator LookAtNeckMan(Transform player)
    {
        // 플레이어가 neckMan을 바라보는 동안의 로직
        isRaycasting = true;
        teleportRaycastTime = 0f;
        float duration = 4.3f;
        float elapsedTime = 0f;

        duringLookSound.volume = 0f;
        duringLookSound.Play();

        while (isRaycasting)
        {
            RaycastHit hit;
            Vector3 direction = neckMan.transform.position - _playerCamera.transform.position;

            if (Physics.Raycast(_playerCamera.transform.position, direction, out hit))
            {
                if (hit.collider.gameObject == neckMan)
                {
                    teleportRaycastTime += Time.deltaTime;
                    elapsedTime += Time.deltaTime;
                    
                    duringLookSound.volume = Mathf.Clamp01(elapsedTime / duration);
                    
                    if (teleportRaycastTime >= 7f && !isTeleporting)
                    {
                        isTeleporting = true;
                        playerPosition.position = anoPlaceStart.position;
                        runningNeckMan.SetActive(true);
                        anoPlaceSfx1.Play();
                        anoPlaceSfx2.Play();
                        yield break;
                    }
                }
                else
                {
                    teleportRaycastTime = 0f;
                    elapsedTime = 0f;
                    duringLookSound.volume = 0f;
                }
            }
            yield return null;
        }

        while (duringLookSound.volume > 0)
        {
            duringLookSound.volume -= Time.deltaTime / 2f;
            yield return null;
        }
    }

    // LockOffDelay: 
    // neckMan을 바라보는 동안 카메라의 포커스를 설정하고, 
    // 일정 시간 후에 포커스를 리셋합니다.
    private IEnumerator LockOffDelay()
    {
        PlayerAPI.SetFocusCameraTarget(neckMan.transform);
        yield return new WaitForSeconds(5f);
        PlayerAPI.ResetFocusCameraTarget();
    }

    /// <summary>
    /// 오디오를 한 번만 재생
    /// </summary>
    private void PlaySoundOnce()
    {
        if (corridorLightSound != null && !corridorLightSound.isPlaying)
        {
            corridorLightSound.Play();
        }
    }
}
