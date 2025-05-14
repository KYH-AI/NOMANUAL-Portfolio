using System;
using System.Collections;
using System.Collections.Generic;
using NoManual.Managers;
using UnityEngine;
using TMPro;
using UnityEngine.Events; // TextMeshPro 사용시 필요

public class CreditsManager : MonoBehaviour
{
    public UI_SkipButton SkipButton;
    public AudioSource bgm;
    public TextMeshProUGUI[] centralTexts;

    public float firstTextDisplayTime = 8f;
    public float textDisplayTime = 4f; 
    public float fadeDuration = 0.5f; 
    public RectTransform creditsTransform;
    public float scrollSpeed = 50f;
    public float scrollStartDelay = 2f;
    public float scrollDuration = 1f;

    private void Start()
    {
        SkipButton.SetActiveSkipUiButton();
        creditsTransform.gameObject.SetActive(false); // 처음에는 비활성화
        StartCoroutine(DisplayCentralTexts());
    }

    private void Update()
    {
        if (SkipButton.UpdateSkipUiButton())
        {
            StopAllCoroutines();
            SkipButton.SetDisableSkipUiButton();
            GoMainScene(0.5f);
        }
    }

    private IEnumerator DisplayCentralTexts()
    {
        // 중앙 텍스트를 순차적으로 표시
        for (int i = 0; i < centralTexts.Length; i++)
        {
            TextMeshProUGUI text = centralTexts[i];
            text.gameObject.SetActive(true);
            yield return StartCoroutine(FadeText(text, 0f, 1f)); 
            
            // 첫 번째 텍스트만 길게 노출
            float displayDuration = (i == 0) ? firstTextDisplayTime : textDisplayTime;
            yield return new WaitForSeconds(displayDuration); 
            
            yield return StartCoroutine(FadeText(text, 1f, 0f)); 
            text.gameObject.SetActive(false);
        }
        
        yield return new WaitForSeconds(scrollStartDelay);
        
        creditsTransform.gameObject.SetActive(true); // Credits Transform 활성화
        yield return StartCoroutine(ScrollCredits());
        
        // 모든 텍스트와 크레딧 스크롤이 종료되면 GoMainScene 호출
        GoMainScene(3f);
    }

    private IEnumerator FadeText(TextMeshProUGUI text, float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color originalColor = text.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }
    
    private IEnumerator ScrollCredits()
    {
        // 현재 위치를 가져옴
        Vector2 startPosition = creditsTransform.anchoredPosition;
    
        // 스크롤할 목표 위치 (y=0)
        Vector2 endPosition = new Vector2(startPosition.x, 0f);
    
        // 스크롤 속도
        float elapsedTime = 0f;

        // Smoothly move to end position
        while (elapsedTime < scrollDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / scrollDuration; // 0에서 1 사이의 값으로 변환
            creditsTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t); // 선형 보간
            yield return null; // 다음 프레임까지 대기
        }
        
        SkipButton.SetDisableSkipUiButton();

        // 위치를 정확히 0으로 설정 (오차 방지)
        creditsTransform.anchoredPosition = endPosition;
        GoMainScene(3f);
    }

    /// <summary>
    /// 메인메뉴 씬으로 이동
    /// </summary>
    private void GoMainScene(float duration)
    {
        DOTweenManager.FadeAudioSource(false, bgm, duration, 0f, new List<UnityAction>
        {
            ()=>GameManager.Instance.OpenScene(GameManager.SceneName.Main)
        });
    }

}
