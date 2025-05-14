using UnityEngine;

namespace NoManual.ANO
{
    /// <summary>
    /// 플레이어 캐릭터가 이상현상 트리거 감지하는 기능
    /// </summary>
    public class Player_ANO_Collider : MonoBehaviour
    {
        // 콜라이더 감지 태그
        private const string _ANO_COLLIDER_TAG = "Anomaly";

        private void OnTriggerEnter(Collider anoTriggerZone)
        {
            
            if(anoTriggerZone.CompareTag(_ANO_COLLIDER_TAG))
            {
                ANO_Component anoComponent = anoTriggerZone.GetComponentInParent<ANO_Component>();
                if (anoComponent != null)
                {
                    anoComponent.ANO_TriggerCheck(anoTriggerZone);
                }
            }
        }
    }
}


