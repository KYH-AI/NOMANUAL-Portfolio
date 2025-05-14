using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class BT_Log : Log
{
    private BehaviorTree bt;
    private SharedVector3 logText;
    
    public override void OnStart()
    {
        bt = GetComponent<BehaviorTree>();
        logText = (SharedVector3)bt.GetVariable("Last Target Vector3");
    }

    public override TaskStatus OnUpdate()
    {
        Debug.Log(logText.Value);
        return TaskStatus.Success;
    }
}
