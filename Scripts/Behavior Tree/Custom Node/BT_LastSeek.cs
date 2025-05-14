using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class BT_LastSeek : Action
{
    public SharedGameObject targetObject;
    public SharedVector3 lastTargetPosition;
    
    [SerializeField] private float lastMinPositionTimer;
    [SerializeField] private float lastMaxPositionTimer;
    private float _lastPositionTimer = 0f;
    private float _lastPositionDuration;
    
    public override void OnStart()
    {
        lastTargetPosition.Value = targetObject.Value.transform.position;
        _lastPositionTimer = 0f;
        _lastPositionDuration = Random.Range(lastMinPositionTimer, lastMaxPositionTimer);
    }
    
    public override TaskStatus OnUpdate()
    {

        if (_lastPositionTimer >= _lastPositionDuration)
        {
            SetLastTargetPosition();
#if UNITY_EDITOR
             Debug.Log("BT_LastSeek 계산완료 : " + lastTargetPosition);
#endif
            return TaskStatus.Success;
        }

        _lastPositionTimer += Time.deltaTime;
        return TaskStatus.Running;
    }

    private void SetLastTargetPosition()
    {
        lastTargetPosition.Value = targetObject.Value.transform.position;
    }
}
