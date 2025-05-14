using System;
using System.Collections;
using System.Collections.Generic;
using NoManual.Managers;
using UnityEngine;
using TMPro;
using UnityEngine.Events; // TextMeshPro ���� �ʿ�

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
        creditsTransform.gameObject.SetActive(false); // ó������ ��Ȱ��ȭ
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
        // �߾� �ؽ�Ʈ�� ���������� ǥ��
        for (int i = 0; i < centralTexts.Length; i++)
        {
            TextMeshProUGUI text = centralTexts[i];
            text.gameObject.SetActive(true);
            yield return StartCoroutine(FadeText(text, 0f, 1f)); 
            
            // ù ��° �ؽ�Ʈ�� ��� ����
            float displayDuration = (i == 0) ? firstTextDisplayTime : textDisplayTime;
            yield return new WaitForSeconds(displayDuration); 
            
            yield return StartCoroutine(FadeText(text, 1f, 0f)); 
            text.gameObject.SetActive(false);
        }
        
        yield return new WaitForSeconds(scrollStartDelay);
        
        creditsTransform.gameObject.SetActive(true); // Credits Transform Ȱ��ȭ
        yield return StartCoroutine(ScrollCredits());
        
        // ��� �ؽ�Ʈ�� ũ���� ��ũ���� ����Ǹ� GoMainScene ȣ��
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
        // ���� ��ġ�� ������
        Vector2 startPosition = creditsTransform.anchoredPosition;
    
        // ��ũ���� ��ǥ ��ġ (y=0)
        Vector2 endPosition = new Vector2(startPosition.x, 0f);
    
        // ��ũ�� �ӵ�
        float elapsedTime = 0f;

        // Smoothly move to end position
        while (elapsedTime < scrollDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / scrollDuration; // 0���� 1 ������ ������ ��ȯ
            creditsTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t); // ���� ����
            yield return null; // ���� �����ӱ��� ���
        }
        
        SkipButton.SetDisableSkipUiButton();

        // ��ġ�� ��Ȯ�� 0���� ���� (���� ����)
        creditsTransform.anchoredPosition = endPosition;
        GoMainScene(3f);
    }

    /// <summary>
    /// ���θ޴� ������ �̵�
    /// </summary>
    private void GoMainScene(float duration)
    {
        DOTweenManager.FadeAudioSource(false, bgm, duration, 0f, new List<UnityAction>
        {
            ()=>GameManager.Instance.OpenScene(GameManager.SceneName.Main)
        });
    }

}
