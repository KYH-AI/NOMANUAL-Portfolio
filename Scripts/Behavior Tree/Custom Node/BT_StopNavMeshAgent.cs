using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class BT_StopNavMeshAgent : Action
{
    [SerializeField] private SharedNavMeshAgent navMeshAgent;

    public override void OnStart()
    {
        if (!navMeshAgent.Value.isStopped) navMeshAgent.Value.isStopped = true;
    }

    public override TaskStatus OnUpdate()
    {
        navMeshAgent.Value.isStopped = true;
        return TaskStatus.Success;
    }
}
