using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccludeAudio : MonoBehaviour
{
        private Transform _playerTransform;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioLowPassFilter audioLowFilter;
        [SerializeField] private AudioReverbFilter audioReverbFilter;

        [SerializeField] private  float _DEFAULT_LOW_PASS_FREQUENCY = 10000f;
        private const float _MAX_CHECK_INTERVAL = 0.5f;
        private float _checkInterval;
        private bool _occluded = false;

        [Header("벽 및 장애물 Layer")] [SerializeField] private LayerMask defaultLayer;
        [Header("라인캐스터 시작 위치 (생략가능)")] [SerializeField] private Transform lineCastStartTransform;

        protected void Awake()
        {
            _playerTransform = HFPS.Player.PlayerController.Instance.transform;
        }

        private void Start()
        {
            _checkInterval = Random.Range(0f, 0.4f);
        }

        private void Update()
        {
            if (_occluded)
            {
                var cutoffFrequency = Mathf.Lerp(
                    audioLowFilter.cutoffFrequency,
                    Mathf.Clamp(
                        2500f / (Vector3.Distance(_playerTransform.position, base.transform.position) / (audioSource.maxDistance / 2f)),
                        900f,
                        4000f
                    ),
                    Time.deltaTime * 8f);
                audioLowFilter.cutoffFrequency = cutoffFrequency;
            }
            else
            {
                audioLowFilter.cutoffFrequency = Mathf.Lerp(audioLowFilter.cutoffFrequency, _DEFAULT_LOW_PASS_FREQUENCY, Time.deltaTime * 8f);
            }
            
            if (_checkInterval >= _MAX_CHECK_INTERVAL)
            {
                Transform startTransform = !lineCastStartTransform ? this.transform : lineCastStartTransform; 
                _checkInterval = 0f;

                if (Physics.Linecast(startTransform.position, _playerTransform.position, out _, defaultLayer, QueryTriggerInteraction.Ignore))
                {
                    _occluded = true;
                }
                else
                {
                    _occluded = false;
                }
            }
            else
            {
                _checkInterval += Time.deltaTime;
            }
        }

    
    

#if UNITY_EDITOR
        // Gizmos를 이용해 라인캐스트 시각적으로 표시
        private void OnDrawGizmos()
        {
            if (!_playerTransform) return; // 플레이어가 할당되지 않았다면 반환

            // 라인캐스트 시작 지점 설정
            Transform startTransform = lineCastStartTransform ? lineCastStartTransform : this.transform;

            // Gizmos 색상 설정
            Gizmos.color = _occluded ? Color.red : Color.green;

            // 라인캐스트 선을 시각적으로 표시
            Gizmos.DrawLine(startTransform.position, _playerTransform.position);
        }
#endif
}