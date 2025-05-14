using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoManual.ANO;

public class ANO14_DropPainting : ANO_Component
{
    [Header("ANO 설정")] 
    [SerializeField] private GameObject[] anoObjs;
    [SerializeField] private Collider anoStart;

    [Header("떨어지기 직전 재생되는 사운드")] 
    public AudioSource anoSfx;

    [Header("힘의 크기 및 방향")] 
    [SerializeField] private Vector3 forceDirection = Vector3.forward; // 기본적으로 앞방향
    [SerializeField] private float forceMagnitude = 5f; // 힘의 크기

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoStart == anoTriggerZone)
        {
            anoStart.enabled = false;
            AddRigidbodyAndApplyForce();
            anoSfx.Play();
        }
    }

    /// <summary>
    /// 그림 오브젝트에 리지드바디를 부여하고 힘을 가함
    /// </summary>
    private void AddRigidbodyAndApplyForce()
    {
        foreach (var anoPicture in anoObjs)
        {
            Rigidbody rb = anoPicture.GetComponent<Rigidbody>();
            
            // Rigidbody가 없으면 추가
            if (rb == null)
            {
                rb = anoPicture.AddComponent<Rigidbody>();
            }

            // 앞방향(또는 원하는 방향)으로 힘을 가함
            rb.AddForce(forceDirection.normalized * forceMagnitude, ForceMode.Impulse);
        }
    }
}