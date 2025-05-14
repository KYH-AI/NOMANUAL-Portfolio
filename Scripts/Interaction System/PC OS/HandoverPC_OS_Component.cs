using System;
using System.Collections;
using System.Collections.Generic;
using NoManual.Interaction;
using NoManual.Managers;
using UnityEngine;

namespace NoManual.Tutorial
{
    public class HandoverPC_OS_Component : PC_OS_Component
    {
        [SerializeField] private Michsky.DreamOS.NotificationCreator notificationCreator;
        public event Action CompleteReportEvent;
        
        protected override void ExitEndOS()
        {
            NoManualHotelManager.Instance.CutSceneManager.CutSceneRunning = false;
            CompleteReportEvent?.Invoke();
            NoManualHotelManager.Instance.EnablePlayer();
        }

        /// <summary>
        /// ÆË¾÷ UI »ý¼º
        /// </summary>
        public void CreateNotification(bool handoverReserve)
        {
            string title;
            string description;
            if (handoverReserve)
            {
                title = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.Alarm_Item_Table, LocalizationTable.Alarm_Item_TableTextKey.HandoverReservePopUp);
                description =  GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.Alarm_Item_Table, LocalizationTable.Alarm_Item_TableTextKey.HandoverReserveDescription);
            }
            else
            {
                title = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.Alarm_Item_Table, LocalizationTable.Alarm_Item_TableTextKey.HandoverReportPopUp);
                description =  GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.Alarm_Item_Table, LocalizationTable.Alarm_Item_TableTextKey.HandoverReportDescription);
            }

            notificationCreator.notificationTitle = title;
            notificationCreator.notificationDescription = description;
            notificationCreator.popupDescription = description;
            notificationCreator.CreateNotification();
        }

    }
}


