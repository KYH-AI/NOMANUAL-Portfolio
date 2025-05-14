using System.Collections;
using System.Collections.Generic;
using NoManual.UI;
using UnityEngine;

public class TextDataBase : MonoBehaviour
{
    [SerializeField] private TextScriptableDataBase textDataBase;
    
    public string GetTextDataValue(TextType textType, TextTip textTip)
    {
        return textDataBase.GetTextDataValue(textType, textTip);
    }
}



