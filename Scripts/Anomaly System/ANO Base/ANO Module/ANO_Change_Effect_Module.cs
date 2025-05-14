using System.Collections;
using System.Collections.Generic;
using NoManual.ANO;
using UnityEngine;

public class ANO_Change_Effect_Module : ANO_BaseModule
{
    [Header("Change_Effect_Module ����")]
    [SerializeField] private GameObject beforeObj; // �ʱ� ������Ʈ
    [SerializeField] private GameObject afterObj; // ��ȯ�� ������Ʈ

    public override void Init()
    {
        base.Init();
    }

    public override void Run(Collider anoTriggerZone)
    {
        if (anoStartCollider != null)
        {
            if (anoTriggerZone == anoStartCollider)
            {
                ChangeObject();
                anoStartCollider.enabled = false;
            }
        }
    }
    
    
    private void ChangeObject()
    {
        beforeObj.SetActive(false);
        afterObj.SetActive(true); // afterObj�� Ȱ��ȭ

        Stop(false);
    }
}
