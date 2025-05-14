using NoManual.ANO;
using UnityEngine;

public class ANO2_Prayer : ANO_Component
{
    [SerializeField] private Collider anoEnd;
    
    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoEnd)
        {
            anoEnd.enabled = false;
            ANO_Clear(true);
        }
    }
}
