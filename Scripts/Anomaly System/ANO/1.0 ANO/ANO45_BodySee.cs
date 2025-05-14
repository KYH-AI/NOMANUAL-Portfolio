using System;
using System.Collections;
using HFPS.Player;
using NoManual.ANO;
using NoManual.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

public class ANO45_BodySee : ANO_Component
{
    [Header("ANO 설정")] 
    [SerializeField] private Collider anoStart;
    [SerializeField] private GameObject[] anoObjs;

    private void Start()
    {
        for (int i = 0; i < anoObjs.Length; i++)
            anoObjs[i].SetActive(false);
    }

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            ActivateANOObjects();
            StartCoroutine(JumpScareAndDamageCoroutine());
            anoStart.enabled = false;
        }
    }

    private void ActivateANOObjects()
    {
        foreach (var anoObj in anoObjs)
        {
            anoObj.SetActive(true);
            
            // 랜덤한 방향으로 조금 비틀기 로직
            float randomX = Random.Range(-10f, 10f); // -10도에서 10도 사이의 랜덤 값
            float randomY = Random.Range(-10f, 10f); // -10도에서 10도 사이의 랜덤 값
            float randomZ = Random.Range(-10f, 10f); // -10도에서 10도 사이의 랜덤 값

            // 현재 회전에 랜덤 값을 더하여 비틀기
            Quaternion randomRotation = Quaternion.Euler(randomX, randomY, randomZ);
            anoObj.transform.rotation = anoObj.transform.rotation * randomRotation;
        }
    }

    private IEnumerator JumpScareAndDamageCoroutine()
    {
        
        yield return new WaitForSeconds(0.5f); 
        NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(3, 0.1f);
        
        PlayerController.Instance.DecreaseMentality(10);
    }
}