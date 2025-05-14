using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpScareTriggerTest : MonoBehaviour
{

   [SerializeField] private int jcId;
   [SerializeField] private float jcDelay;
   private void OnTriggerEnter(Collider other)
   {
      if (other.gameObject.CompareTag("Player"))
      {
         NoManual.Managers.NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(jcId, jcDelay);
      }
   }
}
