using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NoManual.UI
{
    public class UI_ButtonClickAction : MonoBehaviour
    {
        [SerializeField] private Button targetButton;

        public void ButtonClickEvent()
        {
            targetButton.onClick.Invoke();
        }
    }
}


