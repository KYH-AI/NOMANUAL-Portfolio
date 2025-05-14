using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Tutorials;
using UnityEngine;

public class BT_SeekAndChase : CanSeeObject
{

    [BehaviorDesigner.Runtime.Tasks.Tooltip("The speed of the agent")]
    public SharedFloat speed = 10;
    // Component references
    protected UnityEngine.AI.NavMeshAgent navMeshAgent;
    
    public override void OnAwake()
    {
        base.OnAwake();
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }
    
    public override void OnStart()
    {
        base.OnStart();
        navMeshAgent.speed = speed.Value;
        navMeshAgent.isStopped = false;
        
        // 1. 시야 확인
        returnedObject.Value = WithinSight(targetObject.Value, fieldOfViewAngle.Value, viewDistance.Value);
        if (returnedObject.Value != null)
        {
            // 2. 추적
            SetDestination(Target());
        }
    }
    
    public override TaskStatus OnUpdate()
    {
        // 1. 시야 확인
        returnedObject.Value = WithinSight(targetObject.Value, fieldOfViewAngle.Value, viewDistance.Value);
        if (returnedObject.Value != null)
        {
            // 2. 추적
            SetDestination(Target());
            if (navMeshAgent.isOnOffMeshLink)
            {
#if UNITY_EDITOR
                Debug.Log("BT_SeekAndChase : 문 Off Mesh Link");
#endif                
              
            }
            return TaskStatus.Running;
        }
        // 3. 추적 실패 마지막 위치 기억
        return TaskStatus.Failure;
    }
    
    private Vector3 Target()
    {
        return targetObject.Value.transform.position;
    }
    
    /// <summary>
    /// Set a new pathfinding destination.
    /// </summary>
    /// <param name="destination">The destination to set.</param>
    /// <returns>True if the destination is valid.</returns>
    protected bool SetDestination(Vector3 destination)
    {
        navMeshAgent.isStopped = false;
        return navMeshAgent.SetDestination(destination);
    }
    
    protected bool HasArrived()
    {
        if (navMeshAgent.pathPending) return false;
        return  navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;
    }

    
    
}
