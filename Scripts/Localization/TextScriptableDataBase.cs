using System;
using NoManual.UI;
using NoManual.Utils;
using UnityEngine;

namespace NoManual.UI
{
    [Tooltip("사용되는 구간")]
    public enum TextType
    {
        None = -1,
        Interaction = 0,
        Hint = 1,
    }

    [Tooltip("사용처")]
    public enum TextTip
    {
        None = -1,
        ItemPickUp = 0,
        ANO_Attach_Sticker = 1,
        Reporting_DeskTop_PC = 2,
        DoorOpen = 3,
        DoorClose = 4,
        DoorLock = 5,
        DoorUnLock = 6,
        SecurityDoorNeedLock = 7,
        ReadBook = 8,
    }


    /// <summary>
    /// UI 텍스트 DB
    /// </summary>
    [CreateAssetMenu(menuName = "Text/TextDataBase")]
    public class TextScriptableDataBase : ScriptableObject
    {
        public EnumDictionary textDataBase;

        public string GetTextDataValue(TextType textType, TextTip textTip)
        {
            if (textDataBase.ContainsKey(textType))
            {
                if(textDataBase[textType].ContainsKey(textTip))
                {
                    return textDataBase[textType][textTip];
                }
            }

            ErrorCode.SendError(ErrorCode.ErrorCodeEnum.GetTextData);
            return string.Empty;
        }
    }

    [Serializable]
    public class TextData
    {
        public TextType key;
        public string value;
    }
}

[Serializable]
public class EnumDictionary : SerializableDictionary<TextType, EnumTextEnumDictionary>
{
    
}

[Serializable]
public class EnumTextEnumDictionary : SerializableDictionary<TextTip, string>
{
    
}

