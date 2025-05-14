using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using HFPS.Player;
using HFPS.Systems;
using UnityEngine;
using Random = UnityEngine.Random;


namespace NoManual.StateMachine
{
    /// <summary>
    /// 플레이어 정신력 관리 (PlayerController 싱글톤에서 접근 가능)
    /// </summary>
    public class Mentality : MonoBehaviour
    {
        [Header("정신력 흔들림 효과 시네머신")]
        [SerializeField] private CinemachineImpulseSource mentalityShake;
        // 정신력 카메라 Volume 연출
        private JumpscareEffects _jumpScareEffects;
        // 사망처리
        private HealthManager _deadManager;

        public enum MentalMode
        {
            None = -1,
            Level0 = 0, // 100% ~ 76% (Default) 
            Level1 = 1, // 75% ~ 36%
            Level2 = 2, // 35% ~ 1%
            Level3 = 3, // 0% 
            Length = 4,
        }
        
        [Serializable]
        public sealed class MentalityImpulseValues
        {
            [Serializable]
            public struct LevelImpulseValues
            {
                public Vector2 minImpulse;
                public Vector2 maxImpulse;
                public float impulseDuration;
            }

            [Header("Level 0 정신력 값 기준")] 
            [Range(0f, 1f)] public float level0MentalityValue;
                
            [Header("Level 1 정신력 값 기준")] 
            [Range(0f, 1f)] public float level1MentalityValue;
            
            [Header("Level 2 정신력 값 기준")] 
            [Range(0f, 1f)] public float level2MentalityValue;
            
            [Header("Level 3 정신력 값 기준")] 
            [Range(0f, 1f)] public float level3MentalityValue;
            
             // min x,y : -0.001, max x,y : 0.001
             [Header("레벨 1 정신력 연출 값")]
             public LevelImpulseValues level1Impulse; 
             // min x,y : -0.01, max x,y : 0.01
             [Header("레벨 2 정신력 연출 값")]
             public LevelImpulseValues level2Impulse;
             // min x,y : -0.1, max x,y : 0.1
             [Header("레벨 3 정신력 연출 값")]
             public LevelImpulseValues level3Impulse;
        }

        [Header("정신력 카메라 흔들림 연출 설정 값")]
        public MentalityImpulseValues mentalityImpulseSettings = new MentalityImpulseValues();

        // 현재 정신력
        private float _currentMentality;
        public float CurrentMentality => _currentMentality;

        // 정신력 최소치
        public const float MIN_MENTALITY = 0f;
        
        // 정신력 최대치
        public const float MAX_MENTALITY = 1f;

        // 정신력 수치 계산 코루틴
       // private Coroutine _mentalityValueCoroutine;
       private bool _updateMentality = false;

        // 정신력 카메라 흔들림 효과 코루틴
        private Coroutine _mentalityCamShakeCoroutine;

        // 정신력 카메라 이펙트 효과 코루틴
        private Coroutine _mentalityCamEffectCoroutine;

        // 현재 정신력 상태
        [Header("현재 정신력 상태")]
        [SerializeField] private MentalMode currentMentalMode = MentalMode.Level0;

        private Managers.UI_NoManualUIManager _uiManager;

        /// <summary>
        /// 정신력 초기화
        /// </summary>
        public void InitializationMentality(float currentMentality)
        {
            _uiManager = Managers.NoManualHotelManager.Instance.UiNoManualUIManager;
            _jumpScareEffects = ScriptManager.Instance.C<JumpscareEffects>();
            _deadManager = PlayerController.Instance.GetComponent<HealthManager>();

            var currentMentalValue = currentMentality;
            /*
            var initMental = CalculatorMentalLevel(currentMentalValue);
            _currentMentality = currentMentalValue;
            UpdateMentalityEffect(initMental);
            _uiManager.ShowPlayerMentalityState(initMental);
            */
         //   currentMentalMode = CalculatorMentalLevel(currentMentalValue);
            UpdateMentality(currentMentalValue);
        }

        private void Update()
        {
            if (_updateMentality)
            {
                Debug.Log(currentMentalMode);
                // 정신력 텍스트 표시
                // _uiManager.ShowPlayerMentalityState(_currentMentality); 
                // 정신력 아이콘 표시
                _uiManager.ShowPlayerMentalityState(_currentMentality);
                _updateMentality = false;
            }
        }

 

        /// <summary>
        /// 정신력 코루틴 관련 모두 종료
        /// </summary>
        public void AllStopCoroutine()
        {
            StopAllCoroutines();
            _mentalityCamShakeCoroutine = null;
            _mentalityCamEffectCoroutine = null;
        }
        

        /// <summary>
        /// 플레이어 사망 처리
        /// </summary>
        public void DeadCheck()
        {
            if (_currentMentality <= 0f && currentMentalMode == MentalMode.Level3)
            {
                // AllStopCoroutine();
                _deadManager.Death();
            }
        }

        /// <summary>
        /// 자살
        /// </summary>
        public void SelfDead()
        {
            SetMentality(0f);
        }

        /// <summary>
        ///  플레이어 정신력 증가
        /// </summary>
        public void IncreaseMentality(float value)
        {
            UpdateMentality(_currentMentality + value);
        }

        /// <summary>
        ///  플레이어 정신력 감소
        /// </summary>
        public void DecreaseMentality (float value)
        {
            UpdateMentality(_currentMentality - value);
        }

        /// <summary>
        /// 플레이어 정신력 강제로 설정
        /// </summary>
        public void SetMentality(float value)
        {
            UpdateMentality(value);
        }
        
        private void UpdateMentality(float targetValue)
        {
            // 기존 목표 값에 추가로 감소될 값을 누적
            float tempMentalityValue =  Mathf.Clamp(targetValue, MIN_MENTALITY, MAX_MENTALITY); // 최소값 0, 최대값 1로 보정
            MentalMode tempMentalLevel = currentMentalMode;
            
            MentalityValueProcess(tempMentalityValue);

            // 새로 계산된 정신력 등급이랑 현재 정신력 등급이 같은 경우는 이펙트는 무시
            if (tempMentalLevel == currentMentalMode) return;
            
            StopMentalityCoroutines();
            UpdateMentalityEffect(currentMentalMode);
        }

        private void UpdateMentalityEffect(MentalMode tempMentalLevel)
        {
            _mentalityCamShakeCoroutine = StartCoroutine(MentalityCamRandomImpulses(tempMentalLevel));
            _mentalityCamEffectCoroutine = StartCoroutine(MentalityCamEffectProcess(tempMentalLevel));
        }
        
        private void StopMentalityCoroutines()
        {
            StopCoroutineIfNotNull(ref _mentalityCamShakeCoroutine);
            StopCoroutineIfNotNull(ref _mentalityCamEffectCoroutine);
        }

        private void StopCoroutineIfNotNull(ref Coroutine coroutine)
        {
            if (coroutine == null) return;
            
            StopCoroutine(coroutine);
            coroutine = null;
        }
        
        private MentalityImpulseValues.LevelImpulseValues GetImpulse(MentalMode mentalLevel)
        {
            MentalityImpulseValues.LevelImpulseValues levelValues;
            
            switch (mentalLevel)
            {
                case MentalMode.Level1:
                    levelValues = mentalityImpulseSettings.level1Impulse;
                    break;
                
                case MentalMode.Level2:
                    levelValues = mentalityImpulseSettings.level2Impulse;
                    break;
                
                case MentalMode.Level3:
                    levelValues = mentalityImpulseSettings.level3Impulse;
                    break;
                
                default:
                    levelValues = new MentalityImpulseValues.LevelImpulseValues();
                    Debug.LogError($"GetImpulse ({mentalLevel})은 존재하지 않는 정신력 등급입니다.");
                    break;
            }
            
            return levelValues;
        }
        
        public void SetPlayerMaxMentality()
        {
            _currentMentality = MAX_MENTALITY;
            SetCurrentMentalLevel(_currentMentality);
        }
        
        private void SetCurrentMentalLevel(float mentalityValue)
        {
            currentMentalMode = CalculatorMentalLevel(mentalityValue);
        }

        private MentalMode CalculatorMentalLevel(float mentalityValue)
        {
            MentalMode newMentalMode = MentalMode.None;

            if (mentalityImpulseSettings.level0MentalityValue <= mentalityValue)
            {
                newMentalMode = MentalMode.Level0;
            }
            else if (mentalityImpulseSettings.level1MentalityValue <= mentalityValue)
            {
                newMentalMode = MentalMode.Level1;
            }
            else if (mentalityImpulseSettings.level2MentalityValue <= mentalityValue)
            {
                newMentalMode = MentalMode.Level2;
            }
            else if(mentalityImpulseSettings.level3MentalityValue <= mentalityValue)
            {
                newMentalMode = MentalMode.Level3;
            }
            
            return newMentalMode;
        }
        
        private IEnumerator MentalityCamRandomImpulses(MentalMode tempMentalLevel)
        {
            // Level 0 카메라 흔들림 효과 무시
            if (tempMentalLevel is MentalMode.Level0)
            {
                Debug.Log("Level 0 카메라 흔들림 효과 무시");
                yield break;
            }
            
            MentalityImpulseValues.LevelImpulseValues impulseValues = GetImpulse(tempMentalLevel);
        
            
            float timer = 0f;
            float currentImpulseDuration = impulseValues.impulseDuration;
            
            while (timer <= 60f)
            {
                float impulseTimer = 0f;
                
                while(impulseTimer <= currentImpulseDuration)
                {
                    // 랜덤한 속도 설정
                    Vector3 randomVelocity = new Vector3(Random.Range(impulseValues.minImpulse.x, impulseValues.maxImpulse.x), 
                        Random.Range(impulseValues.minImpulse.y, impulseValues.maxImpulse.y), 
                        0f);//GetRandomImpulse();

                    // ImpulseSource에 임펄스 적용
                    mentalityShake.m_ImpulseDefinition.m_ImpulseDuration = currentImpulseDuration;
                    mentalityShake.m_DefaultVelocity = randomVelocity;
                    mentalityShake.GenerateImpulse();

                    impulseTimer += Time.deltaTime;
                    timer += Time.deltaTime;
                    
                    yield return null;
                }
            }
            _mentalityCamShakeCoroutine = null;
        }
        
        private void MentalityValueProcess(float targetValue)
        {
            /*
            while (!Mathf.Approximately(_currentMentality, targetValue))
            {
                _currentMentality = Mathf.MoveTowards(_currentMentality, targetValue, Time.deltaTime * 2f);
                yield return null;
            }
            SetCurrentMentalLevel(_currentMentality);
            _mentalityValueCoroutine = null;
            DeadCheck();
            */
            
            // 현재 정신력 값을 목표 값으로 즉시 변경
            _currentMentality = targetValue;
    
            // 정신력 레벨 설정
            SetCurrentMentalLevel(_currentMentality);

            _updateMentality = true;
    
            // DeadCheck 호출
            DeadCheck();
        }
        
        private IEnumerator MentalityCamEffectProcess(MentalMode tempMentalLevel)
        {
            _jumpScareEffects.MentalityVolumeLevel = tempMentalLevel;

            yield return new WaitUntil(() => _jumpScareEffects.Init);
            
            while (!_jumpScareEffects.MentalityEffect())
            {
                yield return null;
            }

            _mentalityCamEffectCoroutine = null;
        }
    }
}

