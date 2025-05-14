using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

/// <summary>
/// 언어 설정
/// </summary>
public class LocalizationTextManager : MonoBehaviour
{
    /// <summary>
    /// 지역별 언어 열거형
    /// </summary>
    public enum LanguageLocalization
    {
        KR = 0,
        EN = 1,
        JP = 2,
        CH = 3,
    }

    // 현재 설정된 언어
    public LanguageLocalization currentLanguage { get; private set; } = LanguageLocalization.KR;
    // 현재 설정된 언어
    private Locale _currentLanguageLocalization;
    
    /// <summary>
    /// 언어 변경
    /// </summary>
    public void ChangeLocale(LanguageLocalization languageLocalization)
    {
        StartCoroutine(ChangeProcess(languageLocalization));
    }

    /// <summary>
    /// 언어 변경 코루틴
    /// </summary>
    private IEnumerator ChangeProcess(LanguageLocalization languageLocalization)
    {
        yield return LocalizationSettings.InitializationOperation;
        _currentLanguageLocalization = LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[(int)languageLocalization];
        currentLanguage = languageLocalization;
    }

    /// <summary>
    /// 런타임 중 String 텍스트 가져오기
    /// </summary>
    public string GetText<TTextKey>(LocalizationTable.TextTable textTable, TTextKey textKey) where  TTextKey : System.Enum
    {
        string text = LocalizationSettings.StringDatabase.GetLocalizedString(textTable.ToString(), 
                                                textKey.ToString(), 
                                                                             _currentLanguageLocalization);
        return text;
    }
    
    /// <summary>
    /// 런타임 중 String 텍스트 가져오기
    /// </summary>
    public string GetText(LocalizationTable.TextTable textTable, string textKey)
    {
        string text = LocalizationSettings.StringDatabase.GetLocalizedString(textTable.ToString(), 
            textKey, 
            _currentLanguageLocalization);
        return text;
    }
}
