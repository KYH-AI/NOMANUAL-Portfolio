using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoManual.ANO
{
    public class ANO7_AlarmRepeat : ANO_Component
    {
        [Header("알람벨")] [SerializeField] private GameObject alarmBellLight;
        [Header("알람벨 사운드")] [SerializeField] private AudioSource alarmBellSfx;
        [Header("anoStart")] [SerializeField] private Collider anoStart;

        public override void ANO_TriggerCheck(Collider anoTriggerZone)
        {
            if (anoTriggerZone == anoStart)
            {
                alarmBellLight.SetActive(true);
                alarmBellSfx.Play();
                anoStart.enabled = false;
            }
            
            
            // 상호작용 시 alarmBellSfx.Stop();
            //           alarmBellLight.SetActive(false);
        }
    }
}
