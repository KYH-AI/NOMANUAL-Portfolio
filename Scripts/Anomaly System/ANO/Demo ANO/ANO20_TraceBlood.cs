using UnityEngine;
using UnityEngine.Events;

namespace NoManual.ANO
{
    public class ANO20_TraceBlood : ANO_Component
    {
        [SerializeField] private AudioSource traceBloodAudio;
        [SerializeField] private Collider anoStart;

        public override void ANO_TriggerCheck(Collider anoTriggerZone)
        {
            if (anoStart == anoTriggerZone)
            {
                traceBloodAudio.Play();
                anoStart.enabled = false;
            }
        }
    }
    
}

