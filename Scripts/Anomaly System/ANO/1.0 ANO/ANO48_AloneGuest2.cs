using System.Collections;
using System.Collections.Generic;
using HFPS.Player;
using NoManual.ANO;
using NoManual.Managers;
using UnityEngine;

public class ANO48_AloneGuest2 : ANO_Component
{
    [Header("ANO ¼³Á¤")] 
    [SerializeField] private Collider anoStart;
    [SerializeField] private Animator anoAnim;
    [SerializeField] private GameObject anoNpc;
    
    private readonly int isWheelSitAtk = Animator.StringToHash("isWheelSitAtk");
    private readonly int isSitAtk = Animator.StringToHash("isSitAtk");


    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            anoAnim.SetTrigger(isSitAtk);
            
            anoStart.enabled = false;
            
            NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(3, 0.2f);
            
            if(!PlayerAPI.GetHoldBreathState()) PlayerController.Instance.DecreaseMentality(10);
            
            anoStart.enabled = false;
            
            Invoke("anoNpcDestroy", 1f);
        }
    }

    void anoNpcDestroy()
    {
        anoNpc.SetActive(false);
    }
}
