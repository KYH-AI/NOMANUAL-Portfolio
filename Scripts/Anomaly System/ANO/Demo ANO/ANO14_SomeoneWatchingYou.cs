using System;
using System.Collections;
using NoManual.ANO;
using UnityEngine;

namespace NoManual.ANO
{

    public class ANO14_SomeoneWatchingYou : ANO_Component
    {
        public AudioSource windowKnockSfx;
        public Collider anoStart;
        

        public override void ANO_TriggerCheck(Collider anoTriggerZone)
        {
            if (anoTriggerZone == anoStart)
            {
                windowKnockSfx.Play();
            }
        }
        

        protected override void ANO_ClearAction()
        {
            windowKnockSfx.Stop();
        }
    }
}
