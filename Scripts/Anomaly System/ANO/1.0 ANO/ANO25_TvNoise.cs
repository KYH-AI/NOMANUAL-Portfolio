using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoManual.ANO;

public class ANO25_TvNoise : ANO_Component
{
    [Header("ANO ����")] 
    [SerializeField] private Collider anoStart; 
    [SerializeField] private GameObject tvNoise;
    [SerializeField] private AudioSource tvNoiseSfx;

    private int enterCount = 0;  

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            enterCount++; 

            Debug.Log("���� ���� Ƚ��: " + enterCount);

            // �� ��° �����̸� tvNoise Ȱ��ȭ
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
            tvNoise.SetActive(true);  // tvNoise GameObject Ȱ��ȭ
        }

        if (tvNoiseSfx != null)
        {
            tvNoiseSfx.Play();
        }

        anoStart.enabled = false;

    }
}