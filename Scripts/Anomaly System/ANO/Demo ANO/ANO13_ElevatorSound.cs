using UnityEngine;

namespace NoManual.ANO
{
    public class ANO13_ElevatorSound : ANO_Component
    {
        public AudioSource womanCrySfx;
        public Collider anoStart;

        public override void ANO_TriggerCheck(Collider anoTriggerZone)
        {
            if (anoTriggerZone == anoStart)
            {
                womanCrySfx.Play();
                anoStart.enabled = false;
            }
        }

        protected override void ANO_ClearAction()
        {
            womanCrySfx.Stop();
        }
    }

}
