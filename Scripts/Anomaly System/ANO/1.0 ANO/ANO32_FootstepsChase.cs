using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoManual.ANO;
using Random = UnityEngine.Random;

public class ANO32_Footsteps : ANO_Component
{
    [Header("ANO 설정")]
    [SerializeField] private Collider[] anoStarts; // 여러 트리거 중 하나 활성화
    [SerializeField] private Collider anoCollider; // 2차 트리거

    [Header("발자국 관련")]
    // + 노이즈 값 추가, 발자국이 좀 더 랜덤한 위치에 생성됨
    [Tooltip("랜덤한 Noise Position(높을수록 발자국이 더 흩어짐)")]
    [Range(0.1f, 1f)] 
    [SerializeField] private float noiseRange;
    [SerializeField] private AudioClip[] footstepClips;  // 발자국 소리 클립 배열
    [SerializeField] private AudioSource footstepSfxSource;  // 발자국 소리를 재생할 AudioSource
    [SerializeField] private GameObject leftStepPrefab; // 왼발자국 프리팹
    [SerializeField] private GameObject rightStepPrefab; // 오른발자국 프리팹
    [SerializeField] private Transform leftStepInsPos; // 왼발자국 생성 위치
    [SerializeField] private Transform rightStepInsPos; // 오른발자국 생성 위치
    
    [Header("1틱 추적 값")]
    [Range(0.5f, 2f)] 
    [SerializeField] private float distRed = 0.5f; // 거리를 줄일 값

    private bool isFirstStep = false; // 왼발오른발 번갈아
    private float currentRange; // 플레이어와 anoCollider의 거리
    private Transform playerTransform; // 플레이어의 트랜스폼
    private Camera playerCam;
    private float floorHold = 40.02f; // 층 고정 높이

    private bool isFootstepActive = false; // 발자국/발소리 활성화 상태

    private void Start()
    {
        playerTransform = HFPS.Player.PlayerController.Instance.transform;
        playerCam = HFPS.Systems.ScriptManager.Instance.MainCamera;
    }

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        // 트리거 체크 및 활성화
        foreach (var start in anoStarts)
        {
            if (anoTriggerZone == start)
            {
                // 트리거된 콜라이더 외에는 비활성화
                foreach (var col in anoStarts)
                {
                    col.enabled = false;
                }

                // anoCollider 초기 위치 설정
                SetInitialColliderPos();

                StartFootsteps(); // 발자국 생성 로직 시작
                return;
            }
        }
    }

    private void SetInitialColliderPos()
    {
        // 플레이어의 현재 위치에서 일정 거리 뒤로 anoCollider를 이동
        Vector3 directionToPlayer = playerTransform.position - anoCollider.transform.position;
        anoCollider.transform.position = playerTransform.position - directionToPlayer.normalized * currentRange;

        // y값을 40.01로 고정
        Vector3 newPosition = anoCollider.transform.position;
        newPosition.y = floorHold;
        anoCollider.transform.position = newPosition;
    }

    private void StartFootsteps()
    {
        isFootstepActive = true;
        currentRange = Vector3.Distance(playerTransform.position, anoCollider.transform.position);
        StartCoroutine(FootstepRoutine());
    }

    private IEnumerator FootstepRoutine()
    {
        while (isFootstepActive)
        {
            if (!PlayerLookingAtCollider()) // 플레이어가 anoCollider를 보고 있지 않다면
            {
                CreateFootstep();
                MoveColliderCloser();
            }
            yield return new WaitForSeconds(Random.Range(0.5f, 2f)); // 0.5 ~ 2 초 랜덤 대기
        }
    }

    private bool PlayerLookingAtCollider()
    {
        // 카메라 시야에 anoCollider가 있는지 Raycast로 판단
        Vector3 directionToCollider = anoCollider.transform.position - playerCam.transform.position;
        if (Physics.Raycast(playerCam.transform.position, directionToCollider, out RaycastHit hit))
        {
            if (hit.collider == anoCollider)
            {
                return true;
            }
        }
        return false;
    }

    private void CreateFootstep()
    {
        // 발소리 배열에서 랜덤하게 하나 선택
        int randomIndex = Random.Range(0, footstepClips.Length);
        footstepSfxSource.clip = footstepClips[randomIndex];
    
        // 선택된 발소리 재생
        footstepSfxSource.Play();
        
        // 발자국 생성
        GameObject stepPrefab = isFirstStep ? rightStepPrefab : leftStepPrefab;
        Transform stepInsPos = isFirstStep ? rightStepInsPos : leftStepInsPos;
        
        // 발자국 프리팹 생성
        GameObject step = Instantiate(stepPrefab, stepInsPos.position, Quaternion.identity);
        
        // 발자국의 y값 고정
        Vector3 stepPosition = step.transform.position;
        stepPosition.y = floorHold;
        step.transform.position = stepPosition;
        
        // Noise 값 랜덤 추가
        stepPosition.x += Random.Range(-noiseRange, noiseRange);
        stepPosition.z += Random.Range(-noiseRange, noiseRange);

        step.transform.position = stepPosition;

        // 발자국의 회전값 조정 (플레이어를 향하도록)
        Vector3 directionToPlayer = playerTransform.position - step.transform.position;
        Quaternion rotation = Quaternion.LookRotation(directionToPlayer);
        rotation.x = 0; // x축 회전 제거
        rotation.z = 0; // z축 회전 제거
        // y축으로 180도 회전 추가
        rotation *= Quaternion.Euler(0, 180, 0);
        step.transform.rotation = rotation;

        // 왼발/오른발 번갈아 생성
        isFirstStep = !isFirstStep;
    }

    private void MoveColliderCloser()
    {
        // anoCollider를 플레이어에게 더 가까이 이동
        Vector3 directionToPlayer = (playerTransform.position - anoCollider.transform.position).normalized;
        anoCollider.transform.position += directionToPlayer * distRed;

        // y값 고정
        Vector3 newPosition = anoCollider.transform.position;
        newPosition.y = floorHold;
        anoCollider.transform.position = newPosition;

        // 현재 거리를 업데이트
        currentRange = Vector3.Distance(playerTransform.position, anoCollider.transform.position);
    }
}