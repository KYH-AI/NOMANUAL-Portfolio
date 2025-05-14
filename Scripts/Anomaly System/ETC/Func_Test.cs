using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoManual.ANO;
using NoManual.StateMachine;

public class Func_Test : ANO_Component
{
    [Header("정신력 데미지 구역")] [SerializeField]
    private Collider mentDmg;

    [Header("눈 감는 구역, 안감으면 데미지 들어감")] [SerializeField]
    private Collider blinkEyeDmg;

    [Header("숨 참는 구역, 안감으면 데미지 들어감")] [SerializeField]
    private Collider holdBreathDmg;

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == mentDmg)
        {
            Debug.Log("여기는 데미지 구역");
        }

        if (anoTriggerZone == blinkEyeDmg)
        {
            Debug.Log("여기는 눈감기 구역. 안감으면 데미지");
        }

        if (anoTriggerZone == holdBreathDmg)
        {
            Debug.Log("여기는 숨참기 구역. 안참으면 데미지");
        }
    }

}
