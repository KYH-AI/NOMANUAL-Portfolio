using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoManual.ANO
{
    public class ANO4_DropPainting : ANO_Component
    {
        [Header("그림판들")]
        [SerializeField] private GameObject[] anoPictures;
        [SerializeField] private Collider anoStart;
        
        [Header("떨어지기 직전 재생되는 사운드")]
        public AudioSource picUntieSfx;
        
        
        public override void ANO_TriggerCheck(Collider anoTriggerZone)
        {
            if (anoStart == anoTriggerZone)
            {
                anoStart.enabled = false;
                AddRigidbodyPainting();
                picUntieSfx.Play();
            }
        }
        
        /// <summary>
        /// 그림 오브젝트에 리지드바디 부여
        /// </summary>
        private void AddRigidbodyPainting()
        {
            foreach (var anoPicture in anoPictures)
            {
                // picture GameObject에 Rigidbody가 없는 경우에만 Rigidbody를 추가
                if (!anoPicture.GetComponent<Rigidbody>())
                {
                    anoPicture.AddComponent<Rigidbody>();
                }
            }
        }

        public MeshRenderer[] GetANO_PaintingMeshRenderer()
        {
            MeshRenderer[] anoMesh = new MeshRenderer[anoPictures.Length];
            for (int i = 0; i < anoPictures.Length; i++)
            {
                anoMesh[i] = anoPictures[i].GetComponent<MeshRenderer>();
            }

            return anoMesh;
        }
    }
}


