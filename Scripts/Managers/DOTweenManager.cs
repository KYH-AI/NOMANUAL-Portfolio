using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;


namespace NoManual.Managers
{
    public static class DOTweenManager 
    {
        // 알파 값 0f -> 1f
        public static void FadeInImage(Image image, float duration, List<UnityAction> events)
        {
            image.DOFade(1f, duration).From(0f).OnComplete(() =>
            {
                if (events == null) return;
                
                foreach (var action in events)
                {
                    action?.Invoke();
                }
            });
        }

        // 알파 값 1f -> 0f
        public static void FadeOutImage(Image image, float duration, List<UnityAction> events)
        {
            image.DOFade(0f, duration).From(1f).OnComplete(() =>
            {
                if (events == null) return;
                
                foreach (var action in events)
                {
                    action?.Invoke();
                }
            });
        }
        
        // 알파 값 0f -> 1f
        public static void FadeInCanvasGroup(CanvasGroup canvasGroup, float duration, List<UnityAction> events)
        {
            canvasGroup.DOFade(1f, duration).From(0f).OnComplete(() =>
            {
                if (events == null) return;
                
                foreach (var action in events)
                {
                    action?.Invoke();
                }
            });
        }

        // 알파 값 1f -> 0f
        public static void FadeOutCanvasGroup(CanvasGroup canvasGroup, float duration, List<UnityAction> events)
        {
            canvasGroup.DOFade(0f, duration).From(1f).OnComplete(() =>
            {
                if (events == null) return;
                
                foreach (var action in events)
                {
                    action?.Invoke();
                }
            });
        }

        /// <summary>
        /// 사운드 Fade In, Out
        /// </summary>
        public static Tween FadeAudioSource(bool isFadeIn, AudioSource audioSource, float duration, float endValue, List<UnityAction> events)
        {
            float startValue;
            float targetValue = endValue;

            // 사운드 시작 및 목표 값 설정
            if (isFadeIn)
            {
                startValue = 0f;
            }
            else
            {
                startValue = 1f;
            }
            
            Tween audioFadeTween = audioSource.DOFade(targetValue, duration).From(startValue).OnComplete(() =>
            {
                if (events == null) return;
                
                foreach (var action in events)
                {
                    action?.Invoke();
                }
            });

            return audioFadeTween;
        }
        
        /// <summary>
        /// 사운드 Fade In, Out
        /// </summary>
        public static Tween FadeAudioSource(bool isFadeIn, AudioSource audioSource, float duration, float strValue, float endValue, List<UnityAction> events)
        {
            float startValue = strValue;
            float targetValue = endValue;
            
            Tween audioFadeTween = audioSource.DOFade(targetValue, duration).From(startValue).OnComplete(() =>
            {
                if (events == null) return;
                
                foreach (var action in events)
                {
                    action?.Invoke();
                }
            });

            return audioFadeTween;
        }
    }
}