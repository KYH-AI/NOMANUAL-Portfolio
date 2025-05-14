using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class BT_AnimationHandler : Action
{
    [SerializeField] private SharedCreatureAnimatorHandler animatorHandler;
    [SerializeField] private string targetAnimClipName;
    [SerializeField] [Range(0f, 1f)] private float crossFadeSpeed = 0.2f;
    [SerializeField] private bool isLoopAnimation = false;

    public override void OnStart()
    {
        animatorHandler.Value.PlayAnimation(targetAnimClipName, isLoopAnimation, crossFadeSpeed);
    }
    
    public override TaskStatus OnUpdate()
    {
        return animatorHandler.Value.IsEndAnimation ? TaskStatus.Success : TaskStatus.Running;
    }
}
