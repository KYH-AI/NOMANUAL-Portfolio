using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigMaster : MonoBehaviour
{
   private Rig _rig;
   private Coroutine _weightTransitionCoroutine; // rig 가중치 계산 코루틴
   private bool _init = false;
   public HeadAndChestIK headAndChestIk { get; private set; }
   

   private void Awake()
   {
      if(!_init) Init();
   }

   private void Init()
   {
      _rig = GetComponent<Rig>();
      headAndChestIk = GetComponentInChildren<HeadAndChestIK>();
      _rig.weight = 0f;
      _init = true;
   }

   /// <summary>
   /// Rig 가중치 설정
   /// </summary>
   public void SetRigWeight(float weight)
   {
      //_rig.weight = weight;
      
      if(_weightTransitionCoroutine != null) StopCoroutine(_weightTransitionCoroutine);
      _weightTransitionCoroutine = StartCoroutine(SmoothWeightTransition(weight, 0.7f));
   }

   /// <summary>
   /// Rig 가중치 코루틴
   /// </summary>
   private IEnumerator SmoothWeightTransition(float targetWeight, float duration)
   {
      if(!_init) Init();
      
      float startWeight = _rig.weight;
      float elapsedTime = 0f;

      while (elapsedTime < duration)
      {
         elapsedTime += Time.deltaTime;
         _rig.weight = Mathf.Lerp(startWeight, targetWeight, elapsedTime / duration);
         yield return new WaitForEndOfFrame(); // 프레임 끝에서 업데이트
      }
      _rig.weight = targetWeight;
      _weightTransitionCoroutine = null;
   }
}
