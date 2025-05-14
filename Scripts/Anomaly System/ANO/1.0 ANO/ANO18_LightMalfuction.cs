using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoManual.ANO;

public class ANO18_LightMalfuction : ANO_Component
{
    [Header("ANO 설정")] 
    [SerializeField] private GameObject[] anoLights; // 조명 오브젝트들
    [SerializeField] private Collider anoStart; // 트리거 콜라이더
    [SerializeField] private AudioSource anoSfx; // 사운드 효과

    [Header("재질 설정")]
    [SerializeField] private Material newMaterial; // 변경할 재질

    private readonly int animTrigger = Animator.StringToHash("Trigger1018"); // 애니메이터 트리거 해시값
    
    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        // 플레이어가 anoStart에 접촉했을 때
        if (anoTriggerZone == anoStart)
        {
            // 모든 anoLights의 Animator에서 트리거를 작동시킴
            foreach (var light in anoLights)
            {
                Animator lightAnimator = light.GetComponent<Animator>();
                if (lightAnimator != null)
                {
                    lightAnimator.SetTrigger(animTrigger);
                }

                // Renderer에서 재질을 동적으로 변경
                MeshRenderer lightRenderer = light.GetComponent<MeshRenderer>();
                if (lightRenderer != null)
                {
                    // 재질을 변경
                    Material[] materials = lightRenderer.materials;

                    // 첫 번째 재질을 새 재질로 교체
                    materials[0] = newMaterial;
                    lightRenderer.materials = materials;
                }
            }

            // 사운드 효과 재생
            anoSfx.Play();

            // 트리거 비활성화
            anoStart.enabled = false;
        }
    }
}