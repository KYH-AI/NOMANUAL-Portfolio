using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoManual.ANO;

public class ANO11_SomeOneWatch : ANO_Component
{
    [Header("ANO 설정")] 
    [SerializeField] private Collider[] anoStart; // 여러 시작 트리거
    [SerializeField] private Collider anoEnd; // 종료 트리거
    [SerializeField] private AudioSource anoSfx0; // 첫 번째 사운드
    [SerializeField] private AudioSource anoSfx1; // 두 번째 사운드
    [SerializeField] private GameObject selfDestroy; // 파괴될 오브젝트
    [SerializeField] private GameObject megan; // Megan 오브젝트

    private bool anoStarted = false; // ANO 시작 여부를 확인
    private bool anoEnded = false; // ANO 종료 여부를 확인

    private void Start()
    {
        // 처음에는 Megan의 Rigidbody가 Kinematic 상태여야 한다.
        if (megan.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }
    }

    private void Update()
    {
        CheckRaycast();
    }

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        // 1. anoStart[] 배열 중 하나에 닿았을 때
        if (!anoStarted && IsAnoStartTriggered(anoTriggerZone))
        {
            Debug.Log("ANO 시작");
            // 모든 anoStart 비활성화
            DisableAllColliders(anoStart);
            
            // 2. anoEnd 활성화 및 첫 번째 사운드 재생
            anoEnd.enabled = true;
            anoSfx0.Play();
            
            anoStarted = true;
        }
        
        // 3. anoEnd와 접촉 시 두 번째 사운드 재생 및 selfDestroy 파괴
        if (anoStarted && !anoEnded && anoTriggerZone == anoEnd)
        {
            Debug.Log("ANO 종료");
            anoSfx1.Play();
            StartCoroutine(DestroyAfterSfx(anoSfx1, selfDestroy));
            
            anoEnded = true;
        }
        
        // 4. Raycast 계산
        CheckRaycast();
    }

    private bool IsAnoStartTriggered(Collider anoTriggerZone)
    {
        foreach (var start in anoStart)
        {
            if (anoTriggerZone == start)
            {
                return true;
            }
        }
        return false;
    }

    private void DisableAllColliders(Collider[] colliders)
    {
        foreach (var col in colliders)
        {
            col.enabled = false;
        }
    }

    private IEnumerator DestroyAfterSfx(AudioSource audioSource, GameObject objToDestroy)
    {
        // 오디오 클립이 끝날 때까지 대기
        yield return new WaitForSeconds(audioSource.clip.length);
        Destroy(objToDestroy);
    }

    private void CheckRaycast()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward); // 카메라에서 정면으로 Ray를 쏜다.
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider == anoEnd)
            {

                // Megan의 Rigidbody의 isKinematic을 false로 변경
                if (megan.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.isKinematic = false;
                }
            }
        }
    }
}
