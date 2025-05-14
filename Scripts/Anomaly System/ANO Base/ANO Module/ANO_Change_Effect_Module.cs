using System.Collections;
using System.Collections.Generic;
using NoManual.ANO;
using UnityEngine;

public class ANO_Change_Effect_Module : ANO_BaseModule
{
    [Header("Change_Effect_Module 설정")]
    [SerializeField] private GameObject beforeObj; // 초기 오브젝트
    [SerializeField] private GameObject afterObj; // 변환할 오브젝트

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
        afterObj.SetActive(true); // afterObj를 활성화

        Stop(false);
    }
}
