using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class BT_AttackTrigger : Action
{
    [SerializeField] private SharedGameObject targetObject;
    [SerializeField] private SharedCreatureAttackTrigger attackTrigger;
    
    public override void OnStart()
    {
        // 지정된 타켓이 없으면 할당
        if (!attackTrigger.Value.Target)
        {
            attackTrigger.Value.Target = targetObject.Value.transform;
        }
        attackTrigger.Value.ToggleAttackTrigger();
    }

    public override TaskStatus OnUpdate()
    {
        return attackTrigger.Value.CompleteTriggerControl ? TaskStatus.Success : TaskStatus.Running;
    }
}
