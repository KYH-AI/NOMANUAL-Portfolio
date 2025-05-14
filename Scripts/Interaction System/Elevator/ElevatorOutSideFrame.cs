using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NoManual.Interaction
{
    public class ElevatorOutSideFrame : MonoBehaviour
    {
        [Header("엘베 내부 컴포넌트")] 
        [SerializeField] private ElevatorComponent elevatorComponent;
        [Header("엘베 외부 컴포넌트 현재 층")] 
        [SerializeField] private int currentFloor;

        public int GetCurrentFloor => currentFloor;
        
        [Header("엘베 외부 전광판 MeshRenderer")] 
        [SerializeField] private MeshRenderer elevatorLedPanel;
        [Header("엘베 외부 호출 버튼")]
        [SerializeField] private ElevatorRecallButton recallButton;
        public ElevatorRecallButton GetRecallButton => recallButton;
        [Header("엘베 외부 문 애니메이터")]
        [SerializeField] private Animator doorAnimator;
        
        private void Awake()
        {
            recallButton.InitElevatorButton(elevatorComponent, elevatorComponent.GetBtnHighLightMaterial);
        }
        
        /// <summary>
        /// 엘베 외부 문 애니메이션 실행
        /// </summary>
        public void PlayDoorAnimation(int animHashKey)
        {
            doorAnimator.SetTrigger(animHashKey);
        }
        
        public void UpdateLedFloorLED(Texture ledTexture)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            elevatorLedPanel.GetPropertyBlock(mpb);
            mpb.SetTexture("_EmissionMap", ledTexture);
            elevatorLedPanel.SetPropertyBlock(mpb);
        }
    }
}


