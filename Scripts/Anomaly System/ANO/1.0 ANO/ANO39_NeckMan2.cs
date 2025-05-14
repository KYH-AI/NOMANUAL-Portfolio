using System;
using System.Collections;
using HFPS.Player;
using UnityEngine;
using NoManual.ANO;
using NoManual.Managers;
using NoManual.StateMachine;

public class ANO39_NeckMan2 : ANO_Component
{
    [Header("ANO 설정")] 
    [SerializeField] private GameObject self;
    [SerializeField] private GameObject[] anoObj;
    [SerializeField] private Collider[] anoStart;
    [SerializeField] private Collider[] anoEnd;
    [SerializeField] private Collider[] anoDamage;
    [SerializeField] private Animator[] anoNpcAnim;
    [SerializeField] private AudioSource[] anoSfx;
    private bool isInAnoDamageZone = false;
    private bool isBlinkEyeZone = false;
    private Coroutine damageCoroutine;

    private void Update()
    {
        if(isBlinkEyeZone && PlayerAPI.GetBlinkEyeState())
        { 
            Invoke("DestroyAno", 0.5f);
            isBlinkEyeZone = false;
        }
    }

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        // anoStart 배열을 체크
        for (int i = 0; i < anoStart.Length; i++)
        {
            if (anoTriggerZone == anoStart[i])
            {
                anoNpcAnim[i].SetTrigger("Trigger1039");
                anoSfx[i].Play();
                isBlinkEyeZone = true;
                
                // 시간 조정
                Invoke("DestroyAno", 10f);
                
            }
        }

        // anoDamage 배열을 체크
        for (int i = 0; i < anoDamage.Length; i++)
        {
            if (anoTriggerZone == anoDamage[i])
            {
                NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(3, 0f);
                PlayerController.Instance.DecreaseMentality(30); // 데미지 적용
                Invoke("DestroyAno", 0.5f);
            }
        }

        // anoEnd 배열을 체크
        for (int i = 0; i < anoEnd.Length; i++)
        {
            if (anoTriggerZone == anoEnd[i])
            {
                anoObj[i].SetActive(false);
            }
        }
    }
    
    private void DestroyAno()
    {
        // 모든 anoObj를 비활성화
        foreach (GameObject obj in anoObj)
        {
            obj.SetActive(false);
            Destroy(self);
        }
    }
}
