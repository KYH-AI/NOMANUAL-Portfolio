using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// UI 스킵 버튼 컴포넌트
/// </summary>
public class UI_SkipButton : MonoBehaviour, ISkipUiButtonController
{
    [SerializeField] private GameObject skipUiPanel;
    [SerializeField] private Image tabImage;
    private bool _isSkipping = false;
    
    public bool UpdateSkipUiButton()
    {
        if (!skipUiPanel.gameObject.activeSelf)
        {
            if (Input.anyKeyDown)
            {
                skipUiPanel.gameObject.SetActive(true);
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Tab))
            {
                // 점점 채우기
                tabImage.fillAmount = Mathf.Clamp(tabImage.fillAmount + Time.deltaTime / 2f, 0, 1);
                if (Mathf.Approximately(tabImage.fillAmount, 1))
                {
                    _isSkipping = true;
                }
            }
            else
            {
                // 게이지 점점 줄어듬
                tabImage.fillAmount = Mathf.Clamp(tabImage.fillAmount - Time.deltaTime, 0, 1);
            }
        }

        return _isSkipping;
    }

    public void SetActiveSkipUiButton()
    {
        Init();
        this.gameObject.SetActive(true);
    }

    public void SetDisableSkipUiButton()
    {
        Init();
        this.gameObject.SetActive(false);
    }

    private void Init()
    {
        tabImage.fillAmount = 0f;
        skipUiPanel.gameObject.SetActive(false);
        _isSkipping = false;
    }
}
