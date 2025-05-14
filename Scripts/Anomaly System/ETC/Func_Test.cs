using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoManual.ANO;
using NoManual.StateMachine;

public class Func_Test : ANO_Component
{
    [Header("���ŷ� ������ ����")] [SerializeField]
    private Collider mentDmg;

    [Header("�� ���� ����, �Ȱ����� ������ ��")] [SerializeField]
    private Collider blinkEyeDmg;

    [Header("�� ���� ����, �Ȱ����� ������ ��")] [SerializeField]
    private Collider holdBreathDmg;

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == mentDmg)
        {
            Debug.Log("����� ������ ����");
        }

        if (anoTriggerZone == blinkEyeDmg)
        {
            Debug.Log("����� ������ ����. �Ȱ����� ������");
        }

        if (anoTriggerZone == holdBreathDmg)
        {
            Debug.Log("����� ������ ����. �������� ������");
        }
    }

}
