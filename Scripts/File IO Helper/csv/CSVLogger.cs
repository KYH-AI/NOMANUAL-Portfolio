using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using NoManual.Managers;

/// <summary>
/// ANO CSV 파일 로고 생성
/// </summary>
public class CSVLogger 
{
    private string filePath;

    // 파일 이름 중복 체크 및 새로운 파일 이름 생성
    private string GetUniqueFilePath(string directoryPath, string baseFileName)
    {
        int count = 1; // 첫 파일은 "ANO Log 1.csv"
        string filePath = Path.Combine(directoryPath, $"{baseFileName} {count}.csv");

        // 파일이 존재할 경우, 중복 방지를 위해 파일명 뒤에 숫자를 증가시킴
        while (File.Exists(filePath))
        {
            count++;
            filePath = Path.Combine(directoryPath, $"{baseFileName} {count}.csv");
        }

        return filePath;
    }

    void CreateCsvFile()
    {
        // 사용자 바탕화면 경로 가져오기
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string directoryPath = Path.Combine(desktopPath, "ano_log csv");

        // 폴더가 존재하지 않으면 생성
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // 파일 이름이 중복되는 경우 새로운 이름으로 파일 생성
        filePath = GetUniqueFilePath(directoryPath, "ANO Log");

        // CSV 파일에 헤더 추가
        using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            writer.WriteLine("ANO ID,Day,Round,ANO Link ID"); // CSV 헤더
        }
        
        Debug.Log(filePath + " ANO csv log 파일 생성");
    }

    // 로그 기록 메서드
    public void LogAnoData(List<ANO_Manager.ANO_LOG_DATA> logList)
    {
        CreateCsvFile();
        
        // CSV 파일에 데이터 추가
        using (StreamWriter writer = new StreamWriter(filePath, true, Encoding.UTF8))
        {
            foreach (var log in logList)
            {
                // 각 로그 데이터를 CSV 문자열로 변환
                string logEntry = $"{log.id},{log.day},{log.round},{log.link_id}";
                writer.WriteLine(logEntry);  // CSV 파일에 기록
            }
        }
    }
}