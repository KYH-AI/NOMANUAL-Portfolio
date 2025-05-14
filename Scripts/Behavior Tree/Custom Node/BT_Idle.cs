using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class BT_Idle : Action
{
    [SerializeField] private float idleMaxTime;
    [SerializeField] private float idleMinTime;
    private float _idleDuration;
    private float _idleTimer = 0f;
    
    public override void OnStart()
    {
        _idleTimer = 0f;
        _idleDuration = Random.Range(idleMinTime, idleMaxTime);
    }

    public override TaskStatus OnUpdate()
    {
        if (_idleTimer >= _idleDuration)
        {
            return TaskStatus.Success;
        }
        _idleTimer += Time.deltaTime;
        return TaskStatus.Running;
    }
}
