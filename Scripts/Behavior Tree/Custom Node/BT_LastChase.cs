using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class BT_LastChase : BT_SeekAndChase
{
    public SharedVector3 lastTargetPosition;
    
    public override void OnStart()
    {
        //lastTargetPosition = targetObject.Value.transform.position;
        SetDestination(lastTargetPosition.Value);
        //Debug.Log(" BT_LastChase " + lastTargetTransform);
    }
    
    public override TaskStatus OnUpdate()
    {
        // 1. 시야 확인
        returnedObject.Value = WithinSight(targetObject.Value, fieldOfViewAngle.Value, viewDistance.Value);
        if (returnedObject.Value != null)
        {
            Debug.Log("BT_LastChase에서 시야에 들어옴!");
            // 2. Seek And Chase 실행
            return TaskStatus.Failure;
        }

        // 3. 마지막 위치 도착 확인
        if (HasArrived())
        {
            return TaskStatus.Success;
        }
        
        // 4. 마지막 위치까지 이동
        SetDestination(Target());
        return TaskStatus.Running;
    }
    
    private Vector3 Target()
    {
        return lastTargetPosition.Value;
    }
}
