using System;
using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using HFPS.Player;
using HFPS.Systems;
using UnityEngine.Playables;

public class TaskTestCut : MonoBehaviour
{
    [SerializeField] private PlayableDirector timelineAsset;
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private TextMeshProUGUI popUpText;

    [SerializeField] private GameObject playerBlockWall;
    private string mainRichText =         "20??/08/22 목요일\n" +
                                          "\n"+
                                          "실험 기간 <color=red>13주차</color>\n" +
                                          "실험 코드 : <color=green>#13358</color>\n";

    private string popUpRichText1 = "실험 대상 : <b><color=yellow>98RM-JHJ</color></b>";
    private string popUpRichText = "\n"+"\n"+"\n"+"\n" + "실험 대상 : <b><color=yellow>98RM-JHJ</color></b>";
    
    
    
    private void Start()
    {
        HFPS_GameManager.Instance.PlayerActionControllerLock(PlayerController.ControllerFeatures.ControllerLock.Move, false);
        
        timelineAsset.stopped -= PlayerBlockWall;
        timelineAsset.stopped += PlayerBlockWall;
        
        playerBlockWall.SetActive(true);
        mainText.text = mainRichText + popUpRichText1;
        
        mainText.DOFade(0, 0);
        DOTweenTMPAnimator tmpAnimator = new DOTweenTMPAnimator(mainText);

        for (int i = 0; i < tmpAnimator.textInfo.characterCount; i++)
        {
            DOTween.Sequence()
                .Append(tmpAnimator.DOFadeChar(i, 1, 0.25f))
                .SetDelay(i * 0.1f);
        }
        
        Invoke(nameof(PopUpText), 7f);
        
    }

    private void PlayerBlockWall(PlayableDirector director)
    {
        playerBlockWall.SetActive(false);
        HFPS_GameManager.Instance.PlayerActionControllerLock(PlayerController.ControllerFeatures.ControllerLock.Move, true);
    }

    private void PopUpText()
    {
        mainText.DOFade(1, 0);
        popUpText.text = popUpRichText;
        mainText.text = mainRichText + "\n";

        DOTweenTMPAnimator tmproAnimator = new DOTweenTMPAnimator(mainText);
        /*
        for (int i = 0; i < tmproAnimator.textInfo.characterCount; i++)
        {
        
            DOTween.Sequence()
                .Append(tmproAnimator.DOFadeChar(i, 0, 0.1f))
                .SetDelay(i * 0.1f);
        }
        */
        
        // Start the character fade coroutine
        StartCoroutine(FadeInCharacters(tmproAnimator, popuptextEffect));
        
    }

 private IEnumerator FadeInCharacters(DOTweenTMPAnimator tmproAnimator, Action onComplete)
    {
        // Sequence 생성
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < tmproAnimator.textInfo.characterCount; i++)
        {
            int index = i; // Local copy to avoid closure issues
            sequence.Append(tmproAnimator.DOFadeChar(index, 0, 0.05f));
        }

        // Sequence의 완료 콜백 설정
        sequence.OnComplete(() => onComplete?.Invoke());

        yield return sequence.WaitForCompletion(); // 애니메이션이 완료될 때까지 대기
    }

    private void popuptextEffect()
    {
        timelineAsset.Play();
        
        // Create a sequence
        Sequence sequence = DOTween.Sequence();

        sequence.Append(popUpText.DOColor(Color.red, 2f));
        sequence.Join(popUpText.DOScale(1.5f, 2f));
        // Add a callback to fade out the text after the sequence is complete
        sequence.OnComplete(() => 
        {
            popUpText.DOFade(0, 0.5f); // Fade out over 0.5 seconds
        });
    }

  
}
