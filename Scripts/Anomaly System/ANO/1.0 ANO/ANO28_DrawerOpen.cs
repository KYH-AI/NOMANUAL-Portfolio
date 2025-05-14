using System.ComponentModel;
using UnityEngine;
using NoManual.ANO;
using NoManual.Managers;
using NoManual.Utils;

public class ANO28_DrawerOpen : ANO_Component
{
    [Header("ANO 설정")] 
    [SerializeField] private Collider[] anoStarts;   // 여러 개의 anoStart 트리거
    [SerializeField] private GameObject[] anoObjs;   // 여러 개의 anoObj
    [SerializeField] private AudioSource anoSfx;     // 오디오 소스
    [Description("ANO 발동 확률. 0~1 사이")] [SerializeField] private float activePer = 0.35f;       // ano 발동확률
    
    private bool isAnySuccess = false;               // 하나라도 성공했는지 확인
    private int remainingTriggers;                   // 남은 트리거 수
    private static readonly int Trigger1028 = Animator.StringToHash("Trigger1028");

    void Start()
    {
        // 남은 트리거 수 초기화
        remainingTriggers = anoStarts.Length;
    }

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        for (int i = 0; i < anoStarts.Length; i++)
        {
            // 트리거가 현재 anoStarts[i]와 일치할 때
            if (anoTriggerZone == anoStarts[i])
            {
                remainingTriggers--; // 트리거가 발동될 때마다 감소

                if (remainingTriggers == 0 && !isAnySuccess)
                {
                    // 마지막 트리거는 반드시 성공시킴
                    ForceSuccess(i);
                }
                else
                {
                    // 50% 확률로 성공 여부 판단
                    if (Random.value <= activePer)
                    {
                        TriggerSuccess(i);
                    }
                }
                if (isAnySuccess)
                {
                    DisableANOStarts();
                }
                break;
            }
        }
    }
    
    // 마지막 트리거는 반드시 성공
    private void ForceSuccess(int index)
    {
        TriggerSuccess(index);
    }

    
    private void TriggerSuccess(int index)
    {
        isAnySuccess = true; // 성공 기록

        anoSfx.Play();
        NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(2, 0f);        
        // 각 오브젝트에 있는 애니메이터 가져오기
        Animator objAnimator = anoObjs[index].GetComponent<Animator>();
        if (objAnimator != null)
        {
            objAnimator.SetTrigger(Trigger1028);
        }
        anoObjs[index].layer = (int)Layer.LayerIndex.Interact;
    }
    
    // 모든 anoStart 콜라이더를 비활성화
    private void DisableANOStarts()
    {
        foreach (var collider in anoStarts)
        {
            Destroy(collider);
        }
    }
}
