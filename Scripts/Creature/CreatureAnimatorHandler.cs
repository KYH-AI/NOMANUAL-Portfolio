using NoManual.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace NoManual.Creature
{
    [RequireComponent(typeof(Animator))]
    public class CreatureAnimatorHandler : MonoBehaviour
    {
        [SerializeField] private CreatureAnimatorStateSO creatureAnimatorStateSo;
        [SerializeField] private NavMeshAgent navMeshAgent;
        private Animator _creatureAnimator;
        
        private readonly int _MovementSpeed = Animator.StringToHash("Speed");
        private string _currentAnimStateName = string.Empty;
        private bool _isLoopAnim = false;
        public bool IsEndAnimation { get; private set; } = false;
        
        private void Awake()
        {
            _creatureAnimator = GetComponent<Animator>();
        }

        /// <summary>
        /// 애니메이터 초가화
        /// </summary>
        public void InitAnimator(CreatureAnimatorStateSO animatorStateSO)
        {
            if (!creatureAnimatorStateSo) this.creatureAnimatorStateSo = animatorStateSO;
        }
        
        private void Update()
        {
            // Movement Anim
            _creatureAnimator.SetFloat(_MovementSpeed, navMeshAgent.velocity.magnitude);

            // Etc Anim
            if (!IsEndAnimation)
            {
                IsEndAnimation = GetCurrentAnimationState();
            }
        }


        /// <summary>
        /// 애니메이션 실행
        /// </summary>
        public void PlayAnimation(string animStringValue, bool isLoop, float blendValue = 0.2f, int layer = 0, float normalizedTimeOffset = 0f)
        {
            if (_currentAnimStateName.Equals(animStringValue)) return;
            _currentAnimStateName = creatureAnimatorStateSo.GetAnimatorStateName(animStringValue);
            if(NoManualUtilsHelper.FindStringEmptyOrNull(_currentAnimStateName))
            {
                Debug.LogError($"{gameObject.name} 애니메이터는 {animStringValue} 매개변수를 가지고 있지 않습니다!");
                return;
            }
            _currentAnimStateName = animStringValue;
            _isLoopAnim = isLoop;
            _creatureAnimator.CrossFade(animStringValue, blendValue, layer, normalizedTimeOffset);
            IsEndAnimation = false;
        }
        
        /// <summary>
        /// 애니메이션 상태확인
        /// </summary>
        private bool GetCurrentAnimationState()
        {
            AnimatorStateInfo animStateInfo = _creatureAnimator.GetCurrentAnimatorStateInfo(0);
    
            // 아직 애니메이션이 전환 중이면 false
            if (!animStateInfo.IsName(_currentAnimStateName)) return false;

            // 루프 애니메이션일 경우, 이름만 일치하면 true 반환
            if (_isLoopAnim) return true;

            // 루프가 아닌 경우, 애니메이션이 끝났는지 여부 확인 (normalizedTime이 1 이상이면 끝)
            return animStateInfo.normalizedTime >= 1f;
        }
    }
}


