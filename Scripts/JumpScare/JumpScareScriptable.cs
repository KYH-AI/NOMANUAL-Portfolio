using System;
using UnityEngine;

namespace NoManual.JumpScare
{
    /// <summary>
    /// 점프 스케어 모듈
    /// </summary>
    [CreateAssetMenu(fileName = "[?] New JumpScare", menuName = "JumpScare/JumpScareScriptable")]
    public class JumpScareScriptable : ScriptableObject
    {
        [Header("Jump Scare ID")] 
        public int jumpScareId;

        [Header("Jump Scare Title")] 
        public string jumpScareTitle;
        
        [Serializable]
        public sealed class ScarePositionInfluence
        {
            [Header("카메라 좌표 위치 영향력")] public Vector3 positionInfluence;
            [Header("카메라 좌표 위치 곱셉 값")] public float positionInfluenceMux;
            [Header("카메라 회전 위치 영향력")] public Vector3 rotationInfluence;
        }

        [Serializable]
        public sealed class ScareEffects
        {
            [Header("ChromaticAberration 영향력 계수")] public float chromaticAberrationAmount = 0.8f;
            [Header("Vignette 영향력 계수")] public float vignetteAmount = 0.3f;
            [Header("포스트 프로세싱 효과 지속시간")] public float effectsTime = 5f;

            public sealed class ScareEffectsSpeed
            {
                [Header(" chromatic 과 vignette 연출 속도 (Jump Scare 시작 시")]
                public float scareEffectSpeed = -1;

                [Header(" chromatic 회복 속도 (Jump Scare 끝날 때)")]
                public float chromaticOutSpeed = -1;

                [Header(" vignette 회복 속도 (Jump Scare 끝날 때)")]
                public float vignetteOutSpeed = -1;
            }

            [Header("포스트 프로세싱 효과 속도 조절")] public ScareEffectsSpeed scareEffectsSpeed = new ScareEffectsSpeed();
        }

        [Serializable]
        public sealed class ScareShake
        {
            [Header("magnitude(흔들림 강도 계수")] public float magnitude = 3f;

            [Header("roughness(거칠기(?) 크기 계수")] [Tooltip("낮은 값은 매끄럽고 높은 값은 더 거칠어짐")]
            public float roughness = 3f;

            [Header("흔들림이 사라지는 데 걸리는 시간(초)")] [Tooltip("한번 흔들림에 걸리는 시간을 의미함")]
            public float startTime = 0.1f;

            [Header("최종적으로 흔들림 지속 시간(초)")] public float durationTime = 3f;
        }

        [Serializable]
        public sealed class JumpScareSound
        {
            [Header("점프 스케어 효과음")] public AudioClip jumpScareSound;
            [Header("점프 스케어 당했을 시 플레이어 사운드")] public AudioClip scaredBreath;
            [Header("scaredBreath 사운드 볼륨")]
            [Range(0, 5)] public float scareVolume;
            [Header("플레이어가 공포에 지속될 시간(초)")]
            [Tooltip("공포 지속시간이 끝나면 스스로 심호흡 하는 사운드가 재생 됨")] public float scaredBreathTime;
        }

        [Header("Jump Scare 카메라 좌표 위치")]
        public ScarePositionInfluence scarePositionInfluence = new ScarePositionInfluence();

        [Header("Jump Scare 카메라 포스트 프로세싱 효과")] public ScareEffects scareEffects = new ScareEffects();
        [Header("Jump Scare 카메라 흔들림 효과")] public ScareShake scareShake = new ScareShake();
        [Header("Jump Scare 효과음")] public JumpScareSound jumpScareSound = new JumpScareSound();
    }
}
