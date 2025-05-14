using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoManual.ANO
{
    public class ANO3_BleedingRoad : ANO_Component
    {
        [SerializeField] private Collider anoEnd;
        
        public override void ANO_TriggerCheck(Collider anoTriggerZone)
        {
            if (anoEnd == anoTriggerZone)
            {
                anoEnd.enabled = false;
                ANO_Clear(true);
            }
        }
    }

}

