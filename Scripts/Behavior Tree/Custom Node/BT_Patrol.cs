using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class BT_Patrol : Action
{
    [SerializeField] private SharedNavMeshAgent navMeshAgent;
    [SerializeField] private SharedFloat patrolSpeed;
    [SerializeField] private float patrolSearchRange;
    [SerializeField] private SharedTransformList patrolNodeList;

    public override void OnStart()
    {
        // 경로를 이미 가지고 있는 경우 경로 계산 X
        if (navMeshAgent.Value.hasPath)
        {
            Debug.Log("BT_Patrol has Path");
            return;
        }
        
        navMeshAgent.Value.speed = patrolSpeed.Value;
        navMeshAgent.Value.SetDestination(GetRandomPosition());
    } 
    
    public override TaskStatus OnUpdate()
    {
        if (navMeshAgent.Value.isOnOffMeshLink)
        {
            Debug.Log("BT_Patrol Is OffMeshLink");
        }
        
        return HasArrived() ? TaskStatus.Success : TaskStatus.Running;
    } 
    
    private Vector3 GetRandomPosition()
    {
        /*
        Vector3 randomDirection = Random.insideUnitSphere * patrolSearchRange;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit  hit, patrolSearchRange, NavMesh.AllAreas))
        {
           return hit.position;
        }
        return transform.position;
        */
        
        
        if (patrolNodeList.Value.Count == 0)
        {
            return transform.position;
        }
        int randomIndex = Random.Range(0, patrolNodeList.Value.Count);
        return patrolNodeList.Value[randomIndex].position;
    }
    
    private bool HasArrived()
    {
        if (navMeshAgent.Value.pathPending) return false;
        return  navMeshAgent.Value.remainingDistance <= navMeshAgent.Value.stoppingDistance;
    }

    
    /// <summary>
    /// 순찰 경로 기즈모
    /// </summary>
#if UNITY_EDITOR
    public override void OnDrawGizmos()
    {
        if (navMeshAgent == null || navMeshAgent.Value == null || !navMeshAgent.Value.hasPath) return;

        // 경로의 모든 코너들을 가져옴
        NavMeshPath path = navMeshAgent.Value.path;
        Vector3[] corners = path.corners;

        // 경로가 2개 이상의 점을 가지고 있을 때만 그림
        if (corners.Length > 1)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < corners.Length - 1; i++)
            {
                // 두 점을 연결하는 선을 그림
                Gizmos.DrawLine(corners[i], corners[i + 1]);
            }
        }
    }
#endif
}

