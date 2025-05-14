using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoManual.ANO;
using NoManual.Managers;

public class ANO15_CryingWoman : ANO_Component
{
    /*
     * 엘리베이터 이상현상
     *
     * anoStart에 닿으면 여자 움. 
     * !! 이 이상현상 발생 때는 엘리베이터 이용 불가. 
     * 
     */

    private PlayerActionCheckerInCollider _actionChecker;

    [Header("설정")] [SerializeField] private Collider anoStart;
    [SerializeField] private AudioSource anoSfx;
 
    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {

        if (anoTriggerZone == anoStart)
        {
            anoSfx.Play();
            // 왼쪽 엘베 비활성화
            HotelManager.Instance.ANO.DisableElevatorInteraction(false);
        }

        /*
        if (_actionChecker.IsHoldingBreath)
        {
            Debug.Log("현재 이상현상 공간에서 숨을 참는 중");
        }

        if (_actionChecker.IsBlinking)
        {
            Debug.Log("현재 이상현상 트리거 안에서 눈 감는중");
        }
        */
    }
    
    private void OnDestroy()
    {
        // 왼쪽 엘베 활성화
        HotelManager.Instance.ANO.EnableElevatorInteraction(false);
    }
}
