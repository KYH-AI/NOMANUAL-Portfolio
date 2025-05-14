using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoManual.ANO;
using HFPS.Player;

public class ANO8_NeckMan : ANO_Component
{
    [Header("ANO 설정")]
    [SerializeField] private HeadAndChestIK headAndChestIK;  // HeadAndChestIK 스크립트 참조
    [SerializeField] private Collider anoStart;  // anoStart Collider
    [SerializeField] private AudioSource anoSfx;  // AudioSource 추가

    private Transform player;  // 플레이어의 Transform
    private bool isPlayerInZone = false;

    /*
     * anoStart에 진입 시에, Head IK를 이용해 플레이어를 계속 바라본다.
     */

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        // anoStart에 플레이어가 진입했는지 확인
        if (anoTriggerZone == anoStart && !isPlayerInZone)
        {
            player = HFPS.Player.PlayerController.Instance.transform;  // HFPS에서 플레이어 Transform 가져오기
            isPlayerInZone = true;  // 플레이어 진입 상태

            // Head IK 가중치 증가를 시작
            StartCoroutine(ActivateHeadIK());

            // 플레이어를 바라보도록 타겟 설정
            headAndChestIK.SetAimTarget(player);
        }
    }

    private IEnumerator ActivateHeadIK()
    {
        anoSfx.Play();

        float weight = 0f;

        // 0.7초 동안 IK 가중치를 증가시킴
        while (weight < 1f)
        {
            weight += Time.deltaTime / 0.7f;
            headAndChestIK.SetHeadIKWeight(weight);
            yield return null;
        }

        headAndChestIK.SetHeadIKWeight(1f);  // IK 가중치를 1로 설정
    }
}