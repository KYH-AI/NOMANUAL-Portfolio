using System.Collections.Generic;
using HFPS.Player;
using UnityEngine;
using NoManual.ANO;
using NoManual.Managers;

public class ANO_Animation_Effect : ANO_Component
{
    [Header("ANO 애니메이션")] [SerializeField] private List<Animator> anoAnims; // Animator 리스트로 설정
    [Header("ANO Start")] [SerializeField] private Collider anoStart;
    [Header("ANO End")] [SerializeField] private Collider anoEnd;
    [Header("ANO SFX")] [SerializeField] private AudioSource[] anoSfx; // AudioSource 배열로 설정

    [Header("ANO End 옵션")]
    [SerializeField] private bool isAnoEnd_AfterObjDestroy; // anoEnd 시 오브젝트 삭제 여부
    [SerializeField] private bool isAnoEnd_AnimTrigger; // anoEnd 시 Trigger2 실행 여부

    [Header("Jumpscare 옵션")] 
    [SerializeField] private bool isJumpscare;
    [SerializeField, Range(2, 4)] private int jumpscareLevel; // 점프스케어 ID값
    [SerializeField] private float jumpscareDelay; // 점프스케어 딜레이값

    [Header("Damage 옵션")] 
    [SerializeField] private bool isDamage;
    [SerializeField] private int damageValue = 0;
    
    private readonly int trigger1 = Animator.StringToHash("Trigger1"); // Trigger1 애니메이션 트리거
    private readonly int trigger2 = Animator.StringToHash("Trigger2"); // Trigger2 애니메이션 트리거

    private void Start()
    {
        if (anoEnd != null)
        {
            anoEnd.enabled = false; // 초기엔 비활성화
        }
    }

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            ActivateAnoStart();
        }
        else if (anoTriggerZone == anoEnd)
        {
            ActivateAnoEnd();
        }
    }

    private void ActivateAnoStart()
    {
        AnimTrigger(trigger1); // Trigger1 애니메이션 실행
        anoStart.enabled = false; // anoStart 비활성화
        PlayAllSfx(); // 모든 SFX 재생
        
        if (anoEnd != null)
        {
            anoEnd.enabled = true; // anoEnd 활성화
        }
        
        if (isJumpscare)
        {
            TriggerJumpscare(); // Jumpscare 즉시 실행
        }

        if (isDamage)
        {
            ApplyDamage(); // Damage 즉시 적용
        }
    }

    private void ActivateAnoEnd()
    {
        if (isAnoEnd_AnimTrigger)
        {
            AnimTrigger(trigger2); // Trigger2 애니메이션 실행
        }

        if (isAnoEnd_AfterObjDestroy)
        {
            DestroyObjects();
        }
        
        anoEnd.enabled = false; // anoEnd 비활성화
    }

    private void AnimTrigger(int animId)
    {
        foreach (var anim in anoAnims)
        {
            anim.SetTrigger(animId);
        }
    }

    // 모든 AudioSource 재생  --> 
    private void PlayAllSfx()
    {
        foreach (var sfx in anoSfx)
        {
            sfx.Play();
        }
    }

    // 모든 오브젝트 삭제
    private void DestroyObjects()
    {
        foreach (var obj in anoAnims)
        {
            Destroy(obj.gameObject);
        }
    }
    
    // 점프스케어 즉시 활성화
    private void TriggerJumpscare()
    {
        NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(jumpscareLevel, jumpscareDelay);
    }

    // 데미지 즉시 적용
    private void ApplyDamage()
    {
        PlayerController.Instance.DecreaseMentality(damageValue);
    }
}
