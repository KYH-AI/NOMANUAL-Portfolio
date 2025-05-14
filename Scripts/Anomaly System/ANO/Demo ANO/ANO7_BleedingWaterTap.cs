using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoManual.ANO
{
    public class ANO7_BleedingWaterTap : ANO_Component
    {
        [SerializeField] private Collider anoStart;
        [SerializeField] private Collider anoDamage;

        [Header("정수기 물통 머티리얼")]
        [SerializeField] private MeshRenderer waterPurifierBottle;

        [Header("변경할 물통 머티리얼")] 
        [SerializeField] private Material changeBottleMaterial;

        [Header("Blood Effect")] 
        [SerializeField] private GameObject bloodEffect;
        
        public override void ANO_TriggerCheck(Collider anoTriggerZone)
        {
            if (anoStart == anoTriggerZone)
            {
                anoStart.enabled = false;
                // 머티리얼 교체
                Material[] mat = waterPurifierBottle.sharedMaterials;
                mat[1] = changeBottleMaterial;
                waterPurifierBottle.materials = mat;
                // 피 효과 활성화
                bloodEffect.SetActive(true);

            }
            else if (anoDamage == anoTriggerZone)
            {
                anoDamage.enabled = false;
                // 플레이어게 정신력 피해 
                ANO_DamageTriggerCheck();
            }
        }
    }
}
