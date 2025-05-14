using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExitMenuDoor : MonoBehaviour
{
   public event UnityAction ExitEvent;

   private Animator _exitDoorAnimator;

   private void Awake()
   {
      _exitDoorAnimator = GetComponent<Animator>();
   }

   public void PlayAnimExitDoorOn()
   {
      _exitDoorAnimator.CrossFade("Exit_Door_On", 0f);
   }

   /// <summary>
   /// 애니메이션 이벤트에서 호출
   /// </summary>
   public void ExitDoorEvent()
   {
      // 카메라 이동 
      ExitEvent?.Invoke();
      this.gameObject.SetActive(false);
     
   }
}
