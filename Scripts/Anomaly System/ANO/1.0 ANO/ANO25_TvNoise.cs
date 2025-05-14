using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoManual.ANO;

public class ANO25_TvNoise : ANO_Component
{
    [Header("ANO 설정")] 
    [SerializeField] private Collider anoStart; 
    [SerializeField] private GameObject tvNoise;
    [SerializeField] private AudioSource tvNoiseSfx;

    private int enterCount = 0;  

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            enterCount++; 

            Debug.Log("현재 진입 횟수: " + enterCount);

            // 두 번째 진입이면 tvNoise 활성화
            if (enterCount == 2)
            {
                ActivateTvNoise();
            }
        }
    }

    private void ActivateTvNoise()
    {
        if(tvNoise != null)
        {
            tvNoise.SetActive(true);  // tvNoise GameObject 활성화
        }

        if (tvNoiseSfx != null)
        {
            tvNoiseSfx.Play();
        }

        anoStart.enabled = false;

    }
}