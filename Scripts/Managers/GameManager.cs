using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : Singleton<GameManager>
{
    /// <summary>
    /// 씬 빌드 순서로 결정
    /// </summary>
    public enum SceneName
    {
        Intro = 0,
        Tutorial = 1,
        Handover = 2,
        Main = 3,
        Loading = 4,
        Hotel = 5,
        Monologue = 6,
        Adjustment = 7,
        
        Ending_Credit = 8,
        Ending_None = 9,
        Ending_A = 10,
        Ending_B = 11,
        Demo_Ending = 12,
    }
    
    
    /// <summary>
    /// 로딩씬 진입 전에 이 변수에 로딩씬에서 실행할 다음 씬 이름을 저장함
    /// </summary>
    public SceneName NextScene { get; set; }
    
    #region 옵션 관련

    public OptionHandler optionHandler { get; private set; }
    public LocalizationTextManager localizationTextManager;
    private readonly int _FRAME_RATE = 144;
    
    #endregion

    #region 세이브 데이터 관련

    public SaveGameManager SaveGameManager;
    // 세이브 파일 손상 확인
    public bool FailLoadToSaveFileSignal = false;

    #endregion

    #region 게임 버전

    public string GameVersion = string.Empty;
    [SerializeField] private TextMeshProUGUI versionText;
    
    #endregion
 
    
    public bool DebugMode = false;
    
    private void Awake()
    {
        SelfDestroyCheck();

        GameVersion = Application.version;
        versionText.text = "v" + GameVersion;
        
        // 옵션 데이터 읽기
        optionHandler = new OptionHandler();
        // 세이브 데이터 읽기
        SaveGameManager = new SaveGameManager();

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        // 프레임 설정
        Application.targetFrameRate = _FRAME_RATE;
    }

    public void OpenScene(SceneName sceneName)
    {
        SceneManager.LoadScene((int)sceneName);
    }
    
    public bool IsSceneCorrect(SceneName scene)
    {
        return SceneManager.GetActiveScene().buildIndex == (int)scene;
    }

}
