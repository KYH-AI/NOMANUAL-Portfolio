using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NoManual.UI
{
    public class UI_RS_Popup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI popUpTitle;
        [SerializeField] private TextMeshProUGUI popUpButtonTitle;
        [SerializeField] private UnityEvent popUpCartButtonEvent;
        [SerializeField] private Button popUpMainButton;
        

        public void SetText(bool cartPopup, string popUpTitle, string buttonTitle)
        {
            this.popUpTitle.text = popUpTitle;
            this.popUpButtonTitle.text = buttonTitle;

            popUpMainButton.onClick.RemoveAllListeners();
            if (cartPopup)
            {
                popUpMainButton.onClick.AddListener(() => popUpCartButtonEvent?.Invoke());
            }
            else
            {
                popUpMainButton.onClick.AddListener(() => this.gameObject.SetActive(false));
            }
            
            this.gameObject.SetActive(true);
        }
    }
}


