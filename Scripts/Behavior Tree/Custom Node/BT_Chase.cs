using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using HFPS.Player;
using UnityEngine;

public class BT_Chase : Action
{
    [SerializeField] private SharedNavMeshAgent navMeshAgent;
    [SerializeField] private SharedGameObject target;
    [SerializeField] private float chaseSpeed;
    private HealthManager _player;

    public override void OnAwake()
    {
        Debug.Log("BT_Chase : Awake");
        _player = target.Value.GetComponent<HealthManager>();
    }

    public override void OnStart()
    {
        navMeshAgent.Value.speed = chaseSpeed;
        navMeshAgent.Value.isStopped = false;
    }

    public override TaskStatus OnUpdate()
    {
        if (!_player || _player.isDead)
        {
            return TaskStatus.Success;
        }
        navMeshAgent.Value.SetDestination(target.Value.transform.position);
        return TaskStatus.Running;
    }
}
