using UnityEngine;

namespace NoManual.ANO
{
    public class ANO16_104SemiOpen : ANO_Component
    {

        // 104호 문 애니메이션, 효과음, anoStart
        [SerializeField] private Animator room104Anim;
        [SerializeField] private AudioSource roomSemiOpenSfx;
        [SerializeField] private Collider anoStart;

        private readonly int semiOpenStart = Animator.StringToHash("Semi Open");
        
        
        // anoStart와 접촉 시, Semi Open 트리거 실행
        public override void ANO_TriggerCheck(Collider anoTriggerZone)
        {
            if (anoTriggerZone == anoStart)
            {
                anoStart.enabled = false;
                Room104Anim(semiOpenStart);
                roomSemiOpenSfx.Play();
            }
        }
        
        // 애니메이터 트리거
        private void Room104Anim(int animId)
        {
            room104Anim.SetTrigger(animId);
        }
    }

}

