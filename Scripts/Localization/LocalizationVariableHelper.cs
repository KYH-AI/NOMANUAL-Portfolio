using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using System.Text.RegularExpressions;


public class LocalizationVariableHelper
{
    public enum VariableTableName
    {
        pc_os = 0,
    }

    public enum PC_OS_HotelHomePage_VariableKey
    {
        customer_stack,
        reservation_stack,
        complete_reservation_stack,
        empty_guestroom_stack,
        forbidden_guestroom_stack,
    }

    public enum PC_OS_Msg_VariableKey
    {
        CallerName,
    }

    /// <summary>
    /// Text 컴포넌트 로컬라이징 변수 업데이트
    /// </summary>
    public static void UpdateLocalizationVariableText<T>(LocalizeStringEvent localizeStringEvent, Enum variableKey, T newValue)
    {
        if (localizeStringEvent.StringReference.TryGetValue(variableKey.ToString(), out IVariable variable))
        {
            // 변수 값 업데이트
            switch (variable)
            {
                case IntVariable intVariable when newValue is int intResult:
                    intVariable.Value = intResult;
                    break;
                case FloatVariable floatVariable when newValue is float floatResult:
                    floatVariable.Value = floatResult;
                    break;
                case StringVariable stringVariable when newValue is string stringResult:
                    stringVariable.Value = stringResult;
                    break;
            }
            // Localize String Event 업데이트
            localizeStringEvent.RefreshString();
        }
    }

    public static string GetLocalizeSmartStringText(LocalizationTable.TextTable localizationTable, Enum tableKey, Dictionary<string, string> placeholders)
    {
        // 로컬라이징된 문자열 가져오기
        var localizedString = new LocalizedString
        {
            TableReference = localizationTable.ToString(), // String Table 이름
            TableEntryReference = tableKey.ToString() // Key 이름
        };

        // 로컬라이징된 문자열을 동기적으로 가져오기
        string messageTemplate = localizedString.GetLocalizedString();

        // 플레이스홀더 대체
        foreach (var placeholder in placeholders)
        {
            // 플레이스홀더가 존재하는지 확인 후 대체
            if (messageTemplate.Contains(placeholder.Key))
            {
                messageTemplate = messageTemplate.Replace(placeholder.Key, placeholder.Value);
            }
        }

        // 최종 문자열 반환
        return messageTemplate;
    }
    
    /// <summary>
    /// 플레이스홀더 포함
    /// </summary>
    public static string GetLocalizeSmartStringText(LocalizationTable.TextTable localizationTable, Enum tableKey)
    {
        // 로컬라이징된 문자열 가져오기
        var localizedString = new LocalizedString
        {
            TableReference = localizationTable.ToString(), // String Table 이름
            TableEntryReference = tableKey.ToString() // Key 이름
        };

        // 로컬라이징된 문자열을 동기적으로 가져오기
        string messageTemplate = localizedString.GetLocalizedString();
        
        // 최종 문자열 반환
        return messageTemplate;
    }

    /// <summary>
    /// 문자열에서 플레이스홀더를 추출하는 메서드
    /// </summary>
    public static string GetPlaceHolderString(string target)
    {
        string placeHolderString = string.Empty;
        string pattern = @"\{(.*?)\}"; 
        // 정규 표현식 등록
        var matches = Regex.Matches(target, pattern);
        foreach (Match match in matches)
        {
            placeHolderString = match.Groups[0].Value;
            break;
        }
        return placeHolderString;
    }
    
    /*
    public static T GetLocalizationVariable<T>(VariableTableName tableName, Enum key) where T : IConvertible
    {
        T value = default;
        // 허용된 타입 목록
        Type[] allowedTypes = { typeof(int), typeof(string), typeof(float) };
        if (!allowedTypes.Contains(typeof(T))) return value;
        var variableTable = LocalizationSettings.StringDatabase.SmartFormatter.GetSourceExtension<PersistentVariablesSource>();
        var variable = variableTable[tableName.ToString()][key.ToString()];

        // 타입 변환 시도
        if (variable is T typedVariable)
            value = typedVariable;

        return value;
    }

    public static void SetLocalizationVariable<T>(VariableTableName tableName, Enum key, T value) where T : IConvertible
    {
        Type[] allowedTypes = { typeof(int), typeof(string), typeof(float) };
        if (!allowedTypes.Contains(typeof(T))) return;
        var variableTable = LocalizationSettings.StringDatabase.SmartFormatter.GetSourceExtension<PersistentVariablesSource>();
        var variable = variableTable[tableName.ToString()][key.ToString()];
        
    }
    */
}
