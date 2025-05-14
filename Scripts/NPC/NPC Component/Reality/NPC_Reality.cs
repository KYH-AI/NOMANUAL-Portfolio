using System.Collections;
using System.Collections.Generic;
using NoManual.Managers;
using NoManual.NPC;
using UnityEngine;

namespace NoManual.NPC
{
    /// <summary>
    /// 3D NPC 
    /// </summary>
    public class NPC_Reality : NPC_Component
    {
        protected Animator npcAnimator;
        protected RigMaster RigMaster;
        
        public override void InitializeNPC(NPC_CloneData npcCloneData)
        {
            base.InitializeNPC(npcCloneData);
            npcAnimator = transform.GetComponent<Animator>();
            RigMaster = GetComponentInChildren<RigMaster>();
        }

        /// <summary>
        /// NPC 애니메이션 실행
        /// </summary>
        /// <param name="animTrigger"></param>
        public void PlayAnimation(string animTrigger)
        {
            npcAnimator.SetTrigger(animTrigger);
        }

        /// <summary>
        /// NPC가 특정대상 Look At
        /// </summary>
        public void LookAtAim(Vector3 lookAtPosition)
        {
           RigMaster.headAndChestIk.SetAimTarget(lookAtPosition);
        }
        
        /// <summary>
        /// NPC가 특정대상 Look At
        /// </summary>
        public void LookAtAim(Transform lookAtTransform)
        {
            RigMaster.headAndChestIk.SetAimTarget(lookAtTransform);
        }
        

        /// <summary>
        /// Rig 가중치 설정
        /// </summary>
        public void SetRigWeight(float weight)
        {
            RigMaster.SetRigWeight(weight);
        }

        public virtual void AnimationEvent(){}
    }
}


