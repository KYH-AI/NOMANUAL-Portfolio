using System.Collections;
using System.Collections.Generic;
using NoManual.ANO;
using UnityEngine;

namespace NoManual.ANO
{
    public class ANO5_RestRoom_BloodPool : ANO_Component
    {
        [Header("피웅덩이")] [SerializeField] private GameObject bloodPoolObj;
        [Header("피웅덩이 스크립트")] [SerializeField] private BloodPoolControl bloodPoolCtrl;
        [Header("피웅덩이 Sfx")] [SerializeField] private AudioSource anoSfx;
        [Header("anoStart")] [SerializeField] private Collider anoStart;


        public override void ANO_TriggerCheck(Collider anoTriggerZone)
        {
            if (anoTriggerZone == anoStart)
            {
                bloodPoolCtrl = bloodPoolObj.GetComponent<BloodPoolControl>();
                bloodPoolCtrl.spreadRate = 4.0f;
                anoStart.enabled = false;
                anoSfx.Play();
            }
            
        }
    }
}
