using System;
using System.Collections;
using System.Collections.Generic;
using NoManual.Managers;
using UnityEngine;

namespace NoManual.ANO
{
    public class ANO15_SamePainting : ANO_Component
    {
        [SerializeField] private Material[] anoEyeMaterials;
        private ANO_Manager _anoManager;
        private bool _swapPainting = false;

        private void Start()
        {
            _anoManager = HotelManager.Instance.ANO;
        }

        private void Update()
        {
            // ANO4 배치 확인
            if (!_swapPainting && _anoManager.IsSucessfullANO_Replace)
            {
                bool ano4Enable = false;
                List<ANO_Component> currentANOList = _anoManager.GetCurrentANO_List;
            
                foreach (var ano in currentANOList)
                {
                    if (ano is ANO4_DropPainting ano4)
                    {
                        MeshRenderer[] anoPaints = ano4.GetANO_PaintingMeshRenderer();

                        for (int i = 0; i < anoPaints.Length; i++)
                        {
                            // 짝수 인덱스일 경우 anoEyeMaterials[0] 할당
                            if (i % 2 == 0)
                            {
                                anoPaints[i].material = anoEyeMaterials[0];
                            }
                            // 홀수 인덱스일 경우 anoEyeMaterials[1] 할당
                            else
                            {
                                anoPaints[i].material = anoEyeMaterials[1];
                            }
                        }

                        ano4Enable = true;
                        this.gameObject.SetActive(false);
                        break;
                    }
                }

                if (!ano4Enable)
                {
                    anoIdleObjectId = 4;
                    Disable_ANO_IdleObject();
                }

                _swapPainting = true;
            }
        }

        public override void ANO_TriggerCheck(Collider anoTriggerZone)
        {
          
        }
    }

}

