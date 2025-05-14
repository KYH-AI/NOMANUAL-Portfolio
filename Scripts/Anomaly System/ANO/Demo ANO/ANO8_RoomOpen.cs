using UnityEngine;

namespace NoManual.ANO
{
    public class ANO8_RoomOpen : ANO_Component
    {
        public AudioSource doorOpenSfx;
        public Animator doorAnimator;
        public Collider anoStart;
        public Collider anoMiddle;
        public Transform teleportAnoSpace;
        
        
        private readonly int doorOpen = Animator.StringToHash("Open");
        private readonly int doorClose = Animator.StringToHash("Close");
        
        public override void ANO_TriggerCheck(Collider anoTriggerZone)
        {
            if (anoTriggerZone == anoStart)
            {
                doorOpenSfx.Play();
                doorAnimator.SetTrigger(doorOpen);
                anoStart.enabled = false;
                ANO_Clear(true);
            }

            if (anoTriggerZone == anoMiddle)
            {
                doorAnimator.SetTrigger(doorClose);
                HFPS.Player.PlayerController.Instance.gameObject.transform.position = teleportAnoSpace.transform.position;
                anoMiddle.enabled = false;
            }
        }
    }
}
