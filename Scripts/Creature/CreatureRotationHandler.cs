using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NoManual.Creature
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CreatureRotationHandler : MonoBehaviour
    {
        private NavMeshAgent _navMeshAgent;
        private Vector3 _dir;
        private Quaternion _targetAngel;
        [Header("회전속도")] 
        [SerializeField] private float rotationSpeed = 8f;
        // 최소 움직임 임계값
        private readonly float _MOVEMENT_THRESHOLD = 0.01f;
        
        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshAgent.updateRotation = false;
        }

        public void InitRotationSpeed(float speed)
        {
            rotationSpeed = speed;
        }

        private void Update()
        {
            if (_navMeshAgent.desiredVelocity.sqrMagnitude >= _MOVEMENT_THRESHOLD)
            {  
                // 이동방향
                _dir = _navMeshAgent.desiredVelocity;
                // 회전각도
                _targetAngel = Quaternion.LookRotation(_dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, _targetAngel, Time.deltaTime * rotationSpeed);
            }
        }
    }
}


