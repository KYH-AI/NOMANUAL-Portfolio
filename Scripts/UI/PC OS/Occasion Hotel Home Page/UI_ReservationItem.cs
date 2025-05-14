using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace NoManual.UI
{
    public class UI_ReservationItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI reservationNameText;
        [SerializeField] private Button cancelButton;
        private string _customerName = string.Empty;
        private bool _isListenerAdded = false;
        
        /// <summary>
        /// 예약자 정보 할당
        /// </summary>
        public void SetReservationData(string customerName, UnityAction<string, bool> cancelBtnEvent)
        {
            this._customerName = customerName;
            reservationNameText.text = customerName;

            if (_isListenerAdded) return;
            if (this._customerName != string.Empty)
            {
                cancelButton.onClick.AddListener(() => cancelBtnEvent(this._customerName, false));
                _isListenerAdded = true;
            }
        }

        /// <summary>
        /// 예악자 정보 초기화
        /// </summary>
        public void ClearReservationData()
        {
            this._customerName = string.Empty;
            reservationNameText.text = string.Empty;
        }

        /// <summary>
        /// 예약자 이름 가져오기
        /// </summary>
        public string GetCustomerName()
        {
            return this._customerName;
        }
        

    }
}


