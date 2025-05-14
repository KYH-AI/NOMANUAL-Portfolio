using System.IO;
using Newtonsoft.Json;
using NoManual.Utils;
using UnityEngine;

public class JsonFileHelper 
{
    /// <summary>
    /// JSON 파일 읽은 후 파싱
    /// </summary>
    public static T ReadJsonFile<T>(string jsonFilePath)
    {
        T jsonData = default;
        
        if (File.Exists(jsonFilePath))
        {
           var jsonFile = File.ReadAllText(Application.dataPath + jsonFilePath);
           jsonData = JsonUtility.FromJson<T>(jsonFile);
        }
        else
        {
            ErrorCode.SendError(jsonFilePath, ErrorCode.ErrorCodeEnum.GetJsonFile);
        }
        return jsonData;
    }
    
    /// <summary>
    /// JSON Text Asset 파일 읽은 후 파싱
    /// </summary>
    public static T ReadJsonAssetFile<T>(string jsonFilePath, JsonSerializerSettings jsonConvert = null)
    {
        T jsonParseData = default;
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFilePath);
        
        if (jsonFile)
        {
            jsonParseData = jsonConvert != null ? JsonConvert.DeserializeObject<T>(jsonFile.text, jsonConvert) : JsonConvert.DeserializeObject<T>(jsonFile.text);
        }
        else
        {
            ErrorCode.SendError(jsonFilePath, ErrorCode.ErrorCodeEnum.GetJsonFile);
        }
        return jsonParseData;
    }
    
    
}


