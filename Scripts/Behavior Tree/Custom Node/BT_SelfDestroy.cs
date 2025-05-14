using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class BT_SelfDestroy : Action
{
    [SerializeField] private float timer = 0f;

    public override TaskStatus OnUpdate()
    {
        if (timer == 0) GameObject.Destroy(this.gameObject);
        else GameObject.Destroy(this.gameObject, timer);
        return TaskStatus.Success;
    }
}
