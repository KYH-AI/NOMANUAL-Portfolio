using UnityEngine;

namespace NoManual.ANO
{

    public class ANO4_SemiOepn_WithBody : ANO_Component
    {
        [Header("ANO 설정")] 
        [SerializeField] private Collider anoStart;
        [SerializeField] private AudioSource anoSfx;

        [Header("문 애니메이터")] 
        [SerializeField] private Animator doorAnimator;

        private readonly int animTrigger = Animator.StringToHash("Trigger1004");

        public override void ANO_TriggerCheck(Collider anoTriggerZone)
        {
            if (anoTriggerZone == anoStart)
            {
                AnimTrigger(animTrigger);
                anoSfx.Play();
                anoStart.enabled = false;
            }
        }
        
        // 애니메이터 트리거
        private void AnimTrigger(int animId)
        {
            doorAnimator.SetTrigger(animId);
        }
    }

}
