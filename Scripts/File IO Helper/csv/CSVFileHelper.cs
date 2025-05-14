using System;
using System.Collections.Generic;
using System.IO;
using NoManual.Utils;
using UnityEngine;

public class CSVFileHelper<T> where T : class
{
    private readonly Func<string[], T> _createInstanceFromValues;

    public CSVFileHelper(Func<string[], T> createInstanceFromValues)
    {
        _createInstanceFromValues = createInstanceFromValues;
    }
    
    private TextAsset ReadCsvFileResources(string csvFilePath)
    {
        return Resources.Load<TextAsset>(csvFilePath);
    }

    public List<T> ParseCSV(string csvFilePath)
    {
        var result = new List<T>();

        TextAsset csvFile = ReadCsvFileResources(csvFilePath);

        if (csvFile != null)
        {
            using (StringReader reader = new StringReader(csvFile.text))
            {
                bool isHeader = true;

                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null) break;

                    if (isHeader)
                    {
                        isHeader = false;
                        continue;
                    }

                    string[] values = line.Split(',');

                    T instance = _createInstanceFromValues(values);
                    result.Add(instance);
                }
            }
        }
        else
        {
            ErrorCode.SendError(ErrorCode.ErrorCodeEnum.GetCSVFile);
        }

        return result;
    }
}
