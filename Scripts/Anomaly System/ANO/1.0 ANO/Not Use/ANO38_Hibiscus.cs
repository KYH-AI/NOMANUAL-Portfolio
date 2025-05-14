using System.Collections;
using System.Collections.Generic;
using HFPS.Player;
using NoManual.ANO;
using NoManual.Managers;
using UnityEngine;

public class ANO38_Hibiscus : ANO_Component
{
    [Header("ANO 설정")]
    [SerializeField] private GameObject[] anoLights;
    [SerializeField] private Collider[] anoStarts;
    [SerializeField] private Collider[] anoDeters;
    [SerializeField] private Collider[] anoSolves;
    [SerializeField] private GameObject[] anoNpcs;

    [SerializeField] private Animator anoLightsAnim;
    [SerializeField] private Animator[] anoNpcsAnim; // 배열로 수정

    private bool isMoving;
    private bool isLighting;
    private Vector3 lastPlayerPosition;

    private Coroutine lightCoroutine;

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        // anoStart에 접촉 시 로직 시작
        for (int i = 0; i < anoStarts.Length; i++)
        {
            if (anoTriggerZone == anoStarts[i])
            {
                // 최초로 LightOn Trigger
                anoLightsAnim.SetTrigger("LightOn");
                // anoStart 비활성화
                anoStarts[i].enabled = false;

                // 라이트 점등 시작
                if (lightCoroutine != null)
                    StopCoroutine(lightCoroutine);
                lightCoroutine = StartCoroutine(ToggleLights());

                // 플레이어 초기 위치 저장
                lastPlayerPosition = PlayerController.Instance.transform.position;
            }
        }

        // 플레이어의 움직임 검사
        for (int i = 0; i < anoDeters.Length; i++)
        {
            if (anoTriggerZone == anoDeters[i])
            {
                if (isMoving && isLighting && Vector3.Distance(PlayerController.Instance.transform.position, lastPlayerPosition) > 0.1f)
                {
                    // 해당 NPC의 Animator만 Running Trigger 신호
                    if (i < anoNpcsAnim.Length) // 배열 범위 검사
                    {
                        anoNpcsAnim[i].SetTrigger("Running");
                        // 점프스케어 및 멘탈 데미지 처리
                        NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(3, 0);
                        PlayerController.Instance.DecreaseMentality(20);
                        Debug.Log($"플레이어가 {i}번째 deter에 있을 때 NPC가 반응했습니다. 점프스케어 발생 및 멘탈 데미지!");
                    }
                }
            }
        }

        // anoSolves 처리
        for (int i = 0; i < anoSolves.Length; i++)
        {
            if (anoTriggerZone == anoSolves[i])
            {
                // 해당하는 NPC 삭제
                if (i < anoNpcs.Length && anoNpcs[i] != null)
                {
                    Destroy(anoNpcs[i]);
                    anoNpcs[i] = null; // 참조를 null로 설정하여 관리
                    Debug.Log($"{i}번째 NPC가 제거되었습니다.");
                }
            }
        }
    }

    private IEnumerator ToggleLights()
    {
        while (true)
        {
            // 랜덤한 시간 대기
            float waitTime = Random.Range(5f, 10f);
            yield return new WaitForSeconds(waitTime);
            
            // 라이트 점등 및 소등
            if (isLighting)
            {
                anoLightsAnim.SetTrigger("LightOff");
                isLighting = false;
            }
            else
            {
                anoLightsAnim.SetTrigger("LightOn");
                isLighting = true;
            }
        }
    }

    void Update()
    {
        // 플레이어의 움직임 체크
        isMoving = PlayerController.Instance.transform.position != lastPlayerPosition;
        if (isMoving)
        {
            lastPlayerPosition = PlayerController.Instance.transform.position;
        }
    }
}
