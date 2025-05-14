using UnityEngine;

namespace NoManual.Patrol
{
    
    /// <summary>
    /// 순찰 지역 오브젝트 자식 트리거
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class ChildTask : MonoBehaviour
    {
        [Header("트리거 접촉 시 해당 콜라이더 비활성화 여부")]
        [SerializeField] private bool onTriggerEnterDisable;
        private PatrolFieldComponent parentTask;

        private void Awake()
        {
            parentTask = GetComponentInParent<PatrolFieldComponent>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (!parentTask)
                {
                    Debug.LogError($"{transform.parent.gameObject.name} PatrolFieldComponent가 없음");
                    return;
                }
                parentTask.ReportTask();
                gameObject.SetActive(!onTriggerEnterDisable);
            }
        }
    }
        
}