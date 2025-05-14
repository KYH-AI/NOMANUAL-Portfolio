using BehaviorDesigner.Runtime;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class SharedNavMeshAgent : SharedVariable<NavMeshAgent>
    {
        public static implicit operator SharedNavMeshAgent(NavMeshAgent value) { return new SharedNavMeshAgent { mValue = value }; }
    }
}


