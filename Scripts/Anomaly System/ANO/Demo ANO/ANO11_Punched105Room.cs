using UnityEngine;

namespace NoManual.ANO
{
    public class ANO11_Punched105Room : ANO_Component
    {

        public Animator doorAnimator;
        public AudioSource doorPunchedSfx;
        public Collider anoStart;

        private readonly int doorPunched = Animator.StringToHash("is Punched");


        public override void ANO_TriggerCheck(Collider anoTriggerZone)
        {
            if (anoTriggerZone == anoStart)
            {
                doorAnimator.SetTrigger(doorPunched);
                doorPunchedSfx.Play();
                ANO_DamageTriggerCheck();
                Managers.NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(0, 0.5f);
                anoStart.enabled = false;
            }
        }
        
        
        protected override void ANO_ClearAction()
        {
            doorAnimator.enabled = false;
            doorPunchedSfx.Stop();
        }
    }
}
