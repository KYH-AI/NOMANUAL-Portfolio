using DG.Tweening;
using NoManual.UI;
using UnityEngine;

/// <summary>
/// Mein Menu 씬 스크립트
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("게임 시작 참조")] 
    [SerializeField] private PlayGame playGameHandler;

    [Header("메인 로고 UI 참조")] 
    [SerializeField] private CanvasGroup mainLogoCanvasGroup;
    
    [Header("옵션 UI 참조")]
    [SerializeField] private UI_Option uiOption;

    [Header("세이브 슬롯 UI 참조")] 
    [SerializeField] private GameObject saveSlotLoadFilePanel;
    [SerializeField] private SaveSlotHandler saveSlotHandler;

    [Header("Sfx 오디오")]
    [SerializeField] private AudioSource buttonClickSfxAudio;
    [SerializeField] private AudioClip buttonClickClip;
    
    
    public void Start()
    {
        // 옵션 UI 초기화
        uiOption.InitializeOptionUI();
        
        saveSlotLoadFilePanel.SetActive(false);
        
        // 세이브 데이터 불러오기
        saveSlotHandler.InitSaveSlot(GameManager.Instance.SaveGameManager.GetAllSaveFileData());
        
        saveSlotHandler.DisableSavePanel();

        saveSlotHandler.ButtonClickSfx -= PlayButtonClickSfx;
        saveSlotHandler.ButtonClickSfx += PlayButtonClickSfx;

        if (GameManager.Instance.FailLoadToSaveFileSignal)
        {
            saveSlotLoadFilePanel.SetActive(true);
            GameManager.Instance.FailLoadToSaveFileSignal = false;
        }
    }

    /// <summary>
    /// UI 비활성화
    /// </summary>
    public void DisableUI(GameObject target)
    {
        target.SetActive(false);
    }

    /// <summary>
    /// UI 활성화
    /// </summary>
    public void EnableUI(GameObject target)
    {
        target.SetActive(true);
    }

    public void PlayButtonClickSfx()
    {
        buttonClickSfxAudio.PlayOneShot(buttonClickClip);
    }

    public void PlayStartGame(GameManager.SceneName nextScene)
    {
        playGameHandler.SceneToGo = nextScene;
        mainLogoCanvasGroup.gameObject.SetActive(true);
        mainLogoCanvasGroup.DOFade(1f, 0.5f).From(0f).OnComplete(() => playGameHandler.PlayGameButton());
    }

}
