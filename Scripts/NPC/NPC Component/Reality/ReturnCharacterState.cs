using System.Collections;
using System.Collections.Generic;
using NoManual.NPC;
using UnityEngine;

/// <summary>
/// 애니메이션 회전 상태
/// </summary>
public class ReturnCharacterState : StateMachineBehaviour
{
    private NPC_Nurse _npcAnimator;
    
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_npcAnimator)
        {
            _npcAnimator = animator.transform.GetComponent<NPC_Nurse>();
        }
        _npcAnimator.IsTurn = false;
        _npcAnimator.CanMove = false;
    }

    
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _npcAnimator.CanMove = true;
    }


}
