using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoManual.ANO
{
    public class ANO6_ChristianCross : ANO_Component
    {
        [Header("설정")] 
        [SerializeField] private Animator christianCrossAnim;
        [SerializeField] private GameObject christianCross;
        [SerializeField] private AudioSource shakeSfx;
        [SerializeField] private Collider anoStart;
        [SerializeField] private Collider anoEnd;
        
        private readonly int _isShake = Animator.StringToHash("isShake");
        private float pushPower = 3f;
        
        private void ChristianCrossAnimator(int animId)
        {
            christianCrossAnim.SetTrigger(animId);
        }
        
        public override void ANO_TriggerCheck(Collider anoTriggerZone)
        {
            if (anoTriggerZone == anoStart)
            {
                /* anoStart 접촉 시,
                1. isShake 시작
                2. shakeSfx 시작
                3. anoStart 비활성화
                */
                ChristianCrossAnimator(_isShake);
                shakeSfx.Play();
                anoStart.enabled = false;
            }
            
            else if (anoTriggerZone == anoEnd)
            {
                /*
                anoEnd 접촉 시,
                1. cross가 중력의 영향을 받음
                2. 떨어질 시 dropSfx 재생 -> 이는 Child에서 수행
                3. anoEnd 비활성화
                */
                christianCrossAnim.enabled = false;
                
                Rigidbody rigid = christianCross.AddComponent<Rigidbody>();
                
                // 바닥 뚫기 방지
                rigid.collisionDetectionMode = CollisionDetectionMode.Continuous;
                shakeSfx.Stop();
                
                if (rigid == null)
                {
                    rigid = christianCross.AddComponent<Rigidbody>();
                }
                
                rigid.AddForce(Vector3.down * pushPower, ForceMode.Impulse);
                rigid.AddForce(Vector3.forward * pushPower, ForceMode.Impulse);
                anoEnd.enabled = false; 
            }
        }
    }
}

