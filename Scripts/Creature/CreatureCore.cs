using System.Linq;
using BehaviorDesigner.Runtime;
using NoManual.Utils;
using UnityEngine;

namespace NoManual.Creature
{
    public class CreatureCore : MonoBehaviour
    {
        [SerializeField] private CreatureSO creatureSo;
        [SerializeField] private BehaviorTree creatureBT;

        [SerializeField] private bool showFieldOfViewGizmo;
        [SerializeField] private Color fieldOfViewColor;
        [SerializeField] private Color viewDistanceColor;

        public int GetCreatureID => creatureSo.CreatureId;
        
        private void Start()
        {
            Initialization();
        }

        /// <summary>
        /// 크리처 초기화
        /// </summary>
        public void Initialization()
        {
            // 추적 대상 설정
            SetCreatureTarget(HFPS.Player.PlayerController.Instance.gameObject);
            
            // BT
            BehaviourTreeInitVariableValue();
            
            // 애니메이터
            GetComponentInChildren<CreatureAnimatorHandler>().InitAnimator(creatureSo.CreatureAnimatorStateSo);
            
            // 공격
            GetComponentInChildren<CreatureAttackTriggerHandler>().InitAttackTrigger(creatureSo.DefaultAttackValue,
                                                                                    creatureSo.KillAttackRange,
                                                                                    creatureSo.DefaultAttackRange,
                                                                                    creatureSo.attackDelay);
            // 회전속도
            GetComponentInChildren<CreatureRotationHandler>().InitRotationSpeed(creatureSo.RotationSpeed);

            // BT 순찰 노드 
            BehaviourTreeInitPatrolNode(Managers.HotelManager.Instance.ANO.GetCreaturePatrolNode(1));
            
            // BT 시작
            creatureBT.EnableBehavior();
        }

        /// <summary>
        /// BT 기초 변수 초기화
        /// </summary>
        private void BehaviourTreeInitVariableValue()
        {
           creatureBT.SetVariableValue("Default View Distance Value", creatureSo.DefaultSearchDistanceRange);
           creatureBT.SetVariableValue("Default Field Of View Angle Value", creatureSo.DefaultSearchAngleRange);
           creatureBT.SetVariableValue("Chase View Distance Value", creatureSo.ChaseSearchDistanceRange);
           creatureBT.SetVariableValue("Chase Field Of View Angle Value", creatureSo.ChaseSearchAngleRange);
           creatureBT.SetVariableValue("Patrol Speed", creatureSo.PatrolSpeed);
           creatureBT.SetVariableValue("Chase Speed", creatureSo.ChaseSpeed);
           creatureBT.SetVariableValue("Door Open Delay", creatureSo.DoorOpenDelay);
        }

        /// <summary>
        /// BT 순찰 노드 초기화
        /// </summary>
        private void BehaviourTreeInitPatrolNode(Transform[] patrolNode)
        {
            if (patrolNode == null || patrolNode.Length == 0)
            {
               NoManualUtilsHelper.EditorDebugLog(NoManualUtilsHelper.LogTextColor.red, "크리처 순찰 노드가 비웠습니다!");
               return;
            }
            creatureBT.SetVariableValue("Patrol Node List", patrolNode.ToList());
        }

        /// <summary>
        /// 크리처 추적 대상 할당
        /// </summary>
        private void SetCreatureTarget(GameObject target)
        {
            creatureBT.SetVariableValue("Target", target);
        }


        /// <summary>
        /// 크리처 소멸 
        /// </summary>
        public void SendCreatureDestroyEvent()
        {
            /*
               공통 : 모든 크리처는 라운드넘기실 소멸
               1. Angel : 라운드를 넘기지 않을 시 소멸 X
               2. Cult Leader : 경비실, 엘베 진입 시 소멸
             */
            
            creatureBT.SendEvent("Destroy");
        }

#if UNITY_EDITOR
        public  void OnDrawGizmos()
        {
            if (showFieldOfViewGizmo && creatureSo)
            {
                
                // 시야각
                var oldColor1 = UnityEditor.Handles.color;
                var color1 = fieldOfViewColor;
                color1.a = 0.1f;
                UnityEditor.Handles.color = color1;
                
                var halfFOV1 = creatureSo.DefaultSearchAngleRange * 0.5f;
                var beginDirection1 = Quaternion.AngleAxis(-halfFOV1, Vector3.up) * transform.forward;
                UnityEditor.Handles.DrawSolidArc(transform.position, transform.up, beginDirection1, creatureSo.DefaultSearchAngleRange, creatureSo.DefaultSearchDistanceRange);
                UnityEditor.Handles.color = oldColor1;
                
                // 길이
                var oldColor2 = UnityEditor.Handles.color;
                var color2 = viewDistanceColor;
                color2.a = 0.1f;
                UnityEditor.Handles.color = color2;
                
                var halfFOV2 = creatureSo.ChaseSearchAngleRange * 0.5f;
                var beginDirection2 = Quaternion.AngleAxis(-halfFOV2, Vector3.up) * transform.forward;
                UnityEditor.Handles.DrawSolidArc(transform.position, transform.up, beginDirection2, creatureSo.ChaseSearchAngleRange, creatureSo.ChaseSearchDistanceRange);
                UnityEditor.Handles.color = oldColor2;
                
                // 일반 공격 범위
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, creatureSo.DefaultAttackRange);

                // 즉사 공격 범위
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, creatureSo.KillAttackRange);
                

            }
        }
#endif
    }
}


