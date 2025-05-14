using System.Collections;
using System.Collections.Generic;
using NoManual.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MedicalServiceTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    
    [SerializeField] private bool hideMentalityText;
    [SerializeField] private MedicalServiceEnum tipTextKey;
    [SerializeField] private Adjustment tipManager;


    public void OnPointerEnter(PointerEventData eventData)
    {
        tipManager.ShowMedicalServiceTip(tipTextKey, hideMentalityText);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
       // tipManager.ShowMedicalServiceTip(tipTextKey);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tipManager.HideMedicalServiceTip();
    }
    

}
