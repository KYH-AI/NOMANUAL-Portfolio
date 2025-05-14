using System.Collections;
using System.Collections.Generic;
using HFPS.Player;
using NoManual.ANO;
using UnityEngine;

public class ANO44_ManequinnChase : ANO_Component
{
    [Header("ANO 설정")]
    [SerializeField] private Collider anoStart; // ANO 시작 지점
    [SerializeField] private GameObject anoObj; // 생성할 마네킹 프리팹
    [SerializeField] private float spawnOffset = 2f; // 플레이어 뒤에 마네킹을 생성할 오프셋 거리
    [SerializeField] private Collider anoEnd;
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private AudioSource anoSfx0;
    [SerializeField] private AudioSource anoSfx1;

    private Transform manequinnLookPoint;
    private GameObject currentManequin; // 현재 생성된 마네킹
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main; // 메인 카메라 참조
    }

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            // 1. player 하위로 gameobject anoSpawnPos 생성, 플레이어 기준 뒤여야 함
            Vector3 spawnPosition = PlayerController.Instance.transform.position - PlayerController.Instance.transform.forward * spawnOffset;

            // 플레이어를 바라보는 방향으로 회전하고 x축을 -90도로 설정
            Quaternion rotation = Quaternion.LookRotation(PlayerController.Instance.transform.forward);
            rotation *= Quaternion.Euler(-90f, 0f, 0f); // x축 -90도 회전

            // 마네킹 생성
            currentManequin = Instantiate(anoObj, spawnPosition, rotation);
            // 마네킹 LookPoint
            manequinnLookPoint = currentManequin.transform.GetChild(0);
            
            Debug.Log("마네킹이 생성되었습니다: " + currentManequin.name); // 마네킹 생성 로그

            // 2. 유저는 anoObj를 바라볼 경우 (이는 Raycast), 정신력 데미지
            StartCoroutine(CheckIfPlayerLooksAtManequin());

            anoStart.enabled = false;
        }
        
        else if (anoTriggerZone == anoEnd)
        {
            anoStart.enabled = true;
            StopAllCoroutines();
            if (currentManequin)
            {
                manequinnLookPoint = null;
                Destroy(currentManequin);
            }
        }
    }

    private IEnumerator CheckIfPlayerLooksAtManequin()
    {
        while (true)
        {
            Vector3 viewportPosition = mainCamera.WorldToViewportPoint(currentManequin.transform.position);
            if (viewportPosition.z < 0f || viewportPosition.x < 0f || viewportPosition.x > 1f || viewportPosition.y < 0f || viewportPosition.y > 1f)
            {
                Vector3 spawnPosition = PlayerController.Instance.transform.position - PlayerController.Instance.transform.forward * spawnOffset;
               
                // 효과음 재생
                // anoSfx0.Play();
                
                // 플레이어를 바라보는 방향으로 회전하고 x축을 -90도로 설정
                Quaternion rotation = Quaternion.LookRotation(PlayerController.Instance.transform.forward);
                rotation *= Quaternion.Euler(-90f, 0f, 0f); // x축 -90도 회전
                // 새로운 마네킹 생성 및 회전
                currentManequin.transform.position = spawnPosition;
                currentManequin.transform.rotation = rotation;
            }
            else
            {
                Vector3 camPosition = mainCamera.transform.position;

                // Raycast를 통해 플레이어가 마네킹을 보고 있는지 확인
                if (Physics.Linecast(camPosition, manequinnLookPoint.position, out RaycastHit hit, layerMask, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider.gameObject == currentManequin)
                    {
                        // 정신력 데미지 적용
                        PlayerController.Instance.DecreaseMentality(5);
                    }
                }
            }
            
            yield return new WaitForSeconds(3f); // 체크 주기
        }
    }
}
