using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoManual.ANO
{
    public class ANO1_DisappearGL_Man : ANO_Component
    {
        [SerializeField] private GameObject lobbyGLMan;
        [SerializeField] private GameObject corridorGLMan;
        [SerializeField] private Collider anoStart;
        private Rigidbody _corridorGLManRigidbody;
        
        private void Awake()
        {
            _corridorGLManRigidbody = corridorGLMan.GetComponent<Rigidbody>();
            
            lobbyGLMan.SetActive(true);
            corridorGLMan.SetActive(false);
        }

        public override void ANO_TriggerCheck(Collider anoTriggerZone)
        {
            if (anoTriggerZone == anoStart)
            {
                // ANO 트리거 Zone 비활성화
                anoStart.enabled = false;
                lobbyGLMan.SetActive(false);
                corridorGLMan.SetActive(true);
                Invoke(nameof(EnableRigidbodyConstraints), 3f);
                Managers.NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(0, 1.25f);
            }
        }

        /// <summary>
        /// corridorGLMan 리지드바디 제약 걸기
        /// </summary>
        private void EnableRigidbodyConstraints()
        {
            _corridorGLManRigidbody.isKinematic = true;
        }
    } 
}



