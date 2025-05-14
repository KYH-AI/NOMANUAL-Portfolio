using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoManual.ANO;


public class ANO0_BrokenClock : ANO_Component
{
    [Header("ANO 설정")] 
    [SerializeField] private Collider anoStart;

    [Header("SFX")] 
    [SerializeField] private AudioSource anoSfx1;
    [SerializeField] private AudioSource anoSfx2;

    
    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            anoStart.enabled = false;
            anoSfx1.Play();
            anoSfx2.Play();
        }
    }

    protected override void ANO_ClearAction()
    {
        anoSfx1.Stop();
        anoSfx2.Stop();
    }
}


