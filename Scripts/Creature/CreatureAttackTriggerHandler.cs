using System;
using System.Collections;
using HFPS.Player;
using UnityEngine;

namespace NoManual.Creature
{
    public class CreatureAttackTriggerHandler : MonoBehaviour
    {
        [Header("크리쳐 공격력")] 
        [SerializeField] private int creatureDamage;
        [Header("즉사 범위")]
        [SerializeField] private float deathAreaDistance;
        [Header("공격 범위")]
        [SerializeField] private float damageAreaDistance;
        [Header("공격 시간")]
        [SerializeField] private float attackDelay;

        private Coroutine _attackCoroutineProcess;
        private WaitForSeconds _attackProcessDelay;


        /// <summary>
        /// 공격 대상 지정
        /// </summary>
        public Transform Target { get; set; }
        
        /// <summary>
        /// 트리거 작업 완료
        /// </summary>
        public bool CompleteTriggerControl { get; private set; }

        /// <summary>
        /// 공격 트리거 초기화
        /// </summary>
        public void InitAttackTrigger(int dmg, float deathArea, float dmgArea, float attackDelay)
        {
            this.creatureDamage = dmg;
            this.deathAreaDistance = deathArea;
            this.damageAreaDistance = dmgArea;
            this.attackDelay = attackDelay;
        }
        
        
        /// <summary>
        /// 공격 트리거 토글
        /// </summary>
        public void ToggleAttackTrigger()
        {
            CompleteTriggerControl = false;
            /* 코루틴 버전
            if (_attackCoroutineProcess != null)
            {
                StopCoroutine(_attackCoroutineProcess);
                _attackCoroutineProcess = null;
                Target = null;
            }
            else
            {
                _attackCoroutineProcess = StartCoroutine(AttackTriggerProcess());
            }
            */
            if (_attackProcess)
            {
                _attackProcess = false;
                Target = null;
            }
            else
            {
                _attackProcess = true;
            }

            _attackTimer = 0;
            CompleteTriggerControl = true;
        }

        private float _attackTimer  = 0f;
        private bool _attackProcess = false;

        private void Update()
        {
            if (!Target || !_attackProcess) return;

            float distanceToTarget = Vector3.Distance(transform.position, Target.position);

            // 즉사 거리 체크
            if (distanceToTarget <= deathAreaDistance)
            {
                // 즉사 처리
                Debug.Log("즉사");
                PlayerController.Instance.SetMentality(0);
                return;  // 즉사 처리 후 더 이상 검사하지 않음
            }

            // 데미지 거리 내에 있을 때만 공격 타이머 증가
            if (distanceToTarget <= damageAreaDistance)
            {
                _attackTimer += Time.deltaTime;

                // 타이머가 공격 지연 시간 이상일 때 공격
                if (_attackTimer >= attackDelay)
                {
                    // 데미지 처리
                    PlayerController.Instance.DecreaseMentality(creatureDamage);
            
                    // 타이머 초기화
                    _attackTimer = 0f;
                }
            }
            else
            {
                // 타겟이 데미지 거리 바깥에 있을 때 타이머 초기화
                _attackTimer = 0f;
            }
        }


        private IEnumerator AttackTriggerProcess()
        {
            if (Target == null) yield break;
            
            _attackProcessDelay = new WaitForSeconds(attackDelay);
            while (true)
            {
                yield return _attackProcessDelay;
                float distanceToTarget = Vector3.Distance(transform.position, Target.position);
                if (deathAreaDistance >= distanceToTarget)
                {
                    // 즉사 
                    Debug.Log("즉사");
                    PlayerController.Instance.SetMentality(0);
                }
                else if (damageAreaDistance >= distanceToTarget)
                {
                    // 데미지 범위 밖에 있을 때 처리
                    PlayerController.Instance.DecreaseMentality(creatureDamage);
                }
            }
        }
    }
}


