using System;
using System.IO;
using NoManual.Utils;
using UnityEngine;
using XEntity.InventoryItemSystem;

public class SavaFileDataWriter
{
    // 세이브 파일 Root
    private const string _SAVE_FILE_DIRECTORY_NAME = "/SaveFile";
    private string _fileDirectoryPath = "";
    private string _filePath = "";

    public SavaFileDataWriter(string fileName)
    {
        _fileDirectoryPath = Application.persistentDataPath + _SAVE_FILE_DIRECTORY_NAME;
        // 디렉토리가 없는 경우 생성
        if (!Directory.Exists(_fileDirectoryPath))
        {
            Directory.CreateDirectory(_fileDirectoryPath);
        }
        _filePath = Path.Combine(_fileDirectoryPath, fileName);
    }

    /// <summary>
    /// 세이브 폴더 존재 확인
    /// </summary>
    public void CheckSaveFilePath()
    {
        if (!Directory.Exists(_fileDirectoryPath))
        {
            // 세이브 폴더가 존재하지 않는 경우, 세이브 폴더를 만듬
            Directory.CreateDirectory(_fileDirectoryPath);
        }
    }
    
    /// <summary>
    /// 세이브 파일 존재 확인
    /// </summary>
    private bool CheckFileExists()
    {
        return File.Exists(_filePath);
    }

    public T LoadFile<T>() where T : SaveData, new()
    {
        T loadData;
        // 기존에 세이브 파일이 존재하는 경우 읽어옴
        if (CheckFileExists())
        {
            try
            {
                string json = File.ReadAllText(_filePath);
                loadData = JsonUtility.FromJson<T>(json);
            }
            catch (Exception e) // 세이브 파일이 손상됬다는 의미. 세이브 파일 기본 버전으로 새로 생성
            {
                loadData = new T();
                loadData.DefaultSettingValue();
                SaveFile(loadData);
            }
        }
        // 기존에 세이브 파일이 없는 경우 새로 생성 후 바로 저장
        else
        {
            CheckSaveFilePath();
            loadData = new T();
            loadData.DefaultSettingValue();
            SaveFile(loadData);
        }
        return loadData;
    }

    public void SaveFile(SaveData saveData)
    {
        string saveFile = JsonUtility.ToJson(saveData, true);
        // 기존에 세이브 파일이 있으면 덮어씀
        File.WriteAllText(_filePath, saveFile);
       
    }


    public void SaveES3SaveFIle<T>(T type, string key, string fileName) where T : SaveData
    {
        _filePath = Path.Combine(_fileDirectoryPath, fileName);
        // 덮어쓰기 및 새로쓰기
        ES3.Save(key, type, _filePath);
    }
    
    public T LoadES3SaveFile<T>(string key, string fileName) where T : SaveData, new()
    {
        T loadData = null;
        _filePath = Path.Combine(_fileDirectoryPath, fileName);
        Debug.Log($"{_filePath} Finding Save File");
        if (CheckFileExists())
        {
            try
            {
                if (ES3.KeyExists(key, _filePath))
                {
                    loadData = (T)ES3.Load(key, _filePath);
                }
            }
            catch
            {
                Debug.LogError($"[{fileName} 세이브 파일 손상 예상]");

                // 손상된 파일 이름 변경
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string corruptFileName = $"{fileName}_corrupt_{timestamp}";
                string corruptFilePath = Path.Combine(_fileDirectoryPath, corruptFileName);

                try
                {
                    File.Move(_filePath, corruptFilePath);
                    Debug.LogWarning($"손상된 파일이 '{corruptFileName}'으로 이름 변경됨.");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"손상된 파일 이름 변경에 실패했습니다. [오류: {ex.Message}]");
                }
                
                GameManager.Instance.FailLoadToSaveFileSignal = true;
                loadData = null;
            }
        }
        return loadData;
    }

    public bool DeleteES3SaveFile(string fileName)
    {
        _filePath = Path.Combine(_fileDirectoryPath, fileName);
        ES3.DeleteFile(_filePath);
        return !CheckFileExists();
    }
}