using System.Collections;
using HFPS.Player;
using UnityEngine;
using NoManual.ANO;
using NoManual.Managers;

public class ANO49_InTheGap : ANO_Component
{
    [Header("ANO 설정")] [SerializeField] private Collider anoStart; 
    [SerializeField] private Collider anoDamage; 
    [SerializeField] private Animator doorAnim; 
    [SerializeField] private GameObject anoNpc; 
    [SerializeField] private Animator anoNpcAnim; 
    [SerializeField] private AudioSource anoSfx0; 
    [SerializeField] private AudioSource anoSfx1; 


    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        // anoStart와 충돌 시
        if (anoTriggerZone == anoStart)
        {
            doorAnim.SetTrigger("Semi Open"); 
            anoStart.enabled = false; 
            anoSfx0.Play();
        }
        
        if (anoTriggerZone == anoDamage)
        {
            anoTriggerZone.enabled = false;
            // NPC 활성화 및 애니메이션 발동
            anoNpc.SetActive(true);
            anoNpcAnim.SetTrigger("Trigger1049");

            // 효과음 재생
            // anoSfx1.Play();

            // 멘탈 데미지 적용
            Invoke("MentalDamage", 0.4f);
    
            Invoke("AnoNpcFalse", 1.5f);
            
            
            // 점프 스케어 실행
            NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(3, 0.25f);
            
        }
    }
    
    // 멘탈 데미지 적용 메서드
    private void MentalDamage()
    {
        PlayerController.Instance.DecreaseMentality(10);
    }

    private void AnoNpcFalse()
    {
        anoNpc.SetActive(false);
    }
}
