using UnityEngine;
using TMPro;

namespace NoManual.UI
{

    public class UI_GuideText : MonoBehaviour
    {

        [SerializeField] private Transform guideTextTransform;
        [SerializeField] private CanvasGroup fadePanel;
        [SerializeField] private TextMeshProUGUI guideText;

        private const float _FADE_FIRST_PHASE_DELAY = 2f; // 2 페이지 시작 전 딜레이
        private bool _startFade = false; // fade 시작 여부
        private float _fadeTimer = 0f; // fade 타이머
        private bool _firstPhaseFade = false; // 1 페이지 종료 확인

        private const float _POS_SMOOTH = 100f; // 가이드 텍스트 이동 속도 
        private Vector3 _currPos;
        private Vector3 _fadeTo = new Vector3(0f, 0f, 0f);
        private Vector3 _velocity;
        

        /// <summary>
        /// 가이드 텍스트 Fade 시작
        /// </summary>
        [ContextMenu("가이드 텍스트 시작")]
        public void StartFade(string text)
        {
            guideText.text = text;
            _currPos = guideTextTransform.localPosition;
            _fadeTimer = 0f;
            fadePanel.alpha = 0f;
            _startFade = true;
        }

        private void Update()
        {
            if (_startFade)
            {
                _fadeTimer += Time.deltaTime;

                if (!_firstPhaseFade)
                {
                    // 0 -> 1 
                    fadePanel.alpha = Mathf.Lerp(0f, 1f, _fadeTimer / 2.5f);

                    // 알파값이 0.9 이상이면 1 페이즈 종료
                    if (fadePanel.alpha >= 0.95f)
                    {
                        fadePanel.alpha = 1f;
                        _firstPhaseFade = true;
                        _fadeTimer = 0f;
                    }
                }
                else if (_fadeTimer >= _FADE_FIRST_PHASE_DELAY) // 딜레이 후  2 페이즈 시작
                {
                    // 1 -> 0
                    fadePanel.alpha = Mathf.Lerp(1f, 0f, _fadeTimer / 2.5f);
                    if (fadePanel.alpha <= 0.01f)
                    {
                        _startFade = false;
                        fadePanel.alpha = 0f;
                        SelfDestroy();
                    }
                }

                // 텍스트 우측으로 이동
                _currPos = Vector3.SmoothDamp(_currPos, _fadeTo, ref _velocity, _POS_SMOOTH * Time.deltaTime);
                guideTextTransform.localPosition = _currPos;
            }
        }

        /// <summary>
        /// 가이드 텍스트 삭제
        /// </summary>
        private void SelfDestroy()
        {
            Destroy(this.gameObject);
        }
    }
}
