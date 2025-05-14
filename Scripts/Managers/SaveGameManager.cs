using NoManual.Managers;
using NoManual.Utils;

public class SaveGameManager 
{
    public enum SaveFileSlot
    {
        None = -1,
        V3_SaveSlot_1 = 0,
        V3_SaveSlot_2 = 1,
        V3_SaveSlot_3 = 2,
    }

    private PlayerSaveData[] _playerSaveData = new PlayerSaveData[3];

    private readonly string _PLAYER_SAVE_DATA_KEY = "PlayerSave";


    
    public SaveFileSlot CurrentSaveFileSlot { get; set; } = SaveFileSlot.None;
    public PlayerSaveData CurrentPlayerSaveData { get; private set; } = null;

    /// <summary>
    /// 로컬에 있는 모든 세이브 파일 읽어오기
    /// </summary>
    public SaveGameManager()
    {
        /*
#if UNITY_EDITOR
        if (GameManager.Instance.DebugMode) return;
#else
        GameManager.Instance.DebugMode = false;
#endif        
        
        SavaFileDataWriter savaFileDataWriter = new SavaFileDataWriter(string.Empty);
        for (int i = 0; i < _playerSaveData.Length; i++)
        {
            // SaveFileSlot 열거형 값을 사용하여 파일을 로드
            SaveFileSlot slot = (SaveFileSlot)i;
            _playerSaveData[i] = savaFileDataWriter.LoadES3SaveFile<PlayerSaveData>(_PLAYER_SAVE_DATA_KEY, slot.ToString());
        }
        */
    }

    public PlayerSaveData[] GetAllSaveFileData()
    {
        SavaFileDataWriter savaFileDataWriter = new SavaFileDataWriter(string.Empty);
        for (int i = 0; i < _playerSaveData.Length; i++)
        {
            // SaveFileSlot 열거형 값을 사용하여 파일을 로드
            SaveFileSlot slot = (SaveFileSlot)i;
            _playerSaveData[i] = savaFileDataWriter.LoadES3SaveFile<PlayerSaveData>(_PLAYER_SAVE_DATA_KEY, slot.ToString());
        }
        return _playerSaveData;
    }
    
    public PlayerSaveData GetSaveFileDataToBaseSaveSlot(SaveFileSlot slot)
    {
        return _playerSaveData[(int)slot];
    }

    /// <summary>
    /// 세이브 파일 데이터 불러오기
    /// </summary>
    public void LoadFile()
    {
        CurrentPlayerSaveData = GetSaveFileDataToBaseSaveSlot(CurrentSaveFileSlot);
        if (CurrentPlayerSaveData == null)
        {
            ErrorCode.SendError(ErrorCode.ErrorCodeEnum.GetSaveFile);
        }
    }

    /// <summary>
    /// 세이브 파일 데이터 쓰기
    /// </summary>
    public void SaveFile()
    {
        if (CurrentSaveFileSlot == SaveFileSlot.None)
        {
            ErrorCode.SendError(ErrorCode.ErrorCodeEnum.GetSaveFileSlot);
            return;
        }
        
        SavaFileDataWriter savaFileDataWriter = new SavaFileDataWriter(string.Empty);
        savaFileDataWriter.SaveES3SaveFIle(CurrentPlayerSaveData, _PLAYER_SAVE_DATA_KEY, CurrentSaveFileSlot.ToString());
        // 현재 세이브 파일 데이터 캐싱
        _playerSaveData[(int)CurrentSaveFileSlot] = CurrentPlayerSaveData;
    }

    /// <summary>
    /// 세이브 파일 지우기
    /// </summary>
    public bool DeleteSaveFile()
    {
        bool isDelete = false;
        CurrentPlayerSaveData = GetSaveFileDataToBaseSaveSlot(CurrentSaveFileSlot);
        if (CurrentPlayerSaveData != null)
        {
            SavaFileDataWriter savaFileDataWriter = new SavaFileDataWriter(string.Empty);
            isDelete = savaFileDataWriter.DeleteES3SaveFile(CurrentSaveFileSlot.ToString());
        }

        if (isDelete)
        {
            _playerSaveData[(int)CurrentSaveFileSlot] = null;
            CurrentSaveFileSlot = SaveFileSlot.None;
            CurrentPlayerSaveData = null;
            return true;
        }
        ErrorCode.SendError(ErrorCode.ErrorCodeEnum.FailDeleteSaveFile);
        return false;
    }

    /// <summary>
    /// 새 게임 세이브 파일 데이터 쓰기
    /// </summary>
    public void CreateNewSaveFile(bool tutorialSkip)
    {
        CurrentPlayerSaveData = new PlayerSaveData();
        CurrentPlayerSaveData.DefaultSettingValue();
        if (tutorialSkip)
        {
            CurrentPlayerSaveData.InventoryItems = new InventorySaveData[]{new InventorySaveData(4028, 1, 0, InventorySlotType.BackPack)};
            CurrentPlayerSaveData.IsTutorialClear = true;
        }
        SaveFile();
    }
}
