using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoManual.ANO;

    public class ANO2_UnknownShadow : ANO_Component 
    {

        // mainHallCollider는 플레이어가 ano를 보고 후퇴했을 경우를 대비해 설치. 충돌 후 모든 anoObj를 비활성화 시킴
        [SerializeField] private Collider mainHallCollider;

        [Header("Gallery 쪽 동선 설정")] 
        [SerializeField] private Collider anoActiveGallery;
        [SerializeField] private GameObject anoObjGallery;
        [SerializeField] private Collider anoEndGallery;

        [Header("Sub Lobby 쪽 동선 설정")] 
        [SerializeField] private Collider anoActiveSub;
        [SerializeField] private GameObject anoObjSub;
        [SerializeField] private Collider anoEndSub;

        [Header("SFX")] 
        [SerializeField] private AudioSource anoSfx;
        
        public override void ANO_TriggerCheck(Collider anoTriggerZone)
        {
            if (anoTriggerZone == null) return;

            // Gallery 동선 처리
            if (anoTriggerZone == anoActiveGallery && anoObjGallery != null)
            {
                anoObjGallery.SetActive(true);
            }

            // Sub Lobby 동선 처리
            if (anoTriggerZone == anoActiveSub && anoObjSub != null)
            {
                anoObjSub.SetActive(true);
            }

            // Main Hall 처리
            if (anoTriggerZone == mainHallCollider)
            {
                if (anoObjGallery != null) anoObjGallery.SetActive(false);
                if (anoObjSub != null) anoObjSub.SetActive(false);
            }
            
            if (anoTriggerZone == anoEndGallery || anoTriggerZone == anoEndSub)
            {
                if (anoActiveGallery != null) anoActiveGallery.enabled = false;
                if (anoActiveSub != null) anoActiveSub.enabled = false;
                if (anoSfx != null) anoSfx.Play();
                
                if (anoObjGallery != null) Destroy(anoObjGallery);
                if (anoObjSub != null) Destroy(anoObjSub);
                Destroy(gameObject, 2f);
            }
        }
    }