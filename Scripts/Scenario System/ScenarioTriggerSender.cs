using UnityEngine;

public class ScenarioTriggerSender : MonoBehaviour
{
    [SerializeField] private ScenarioReceiverBase receiver;

    
    // 플레이어가 Trigger 영역에 진입했을 때
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && receiver != null)
        {
            // Receiver의 이벤트 실행
            receiver.OnScenarioTrigger(this);
            gameObject.SetActive(false);
        }
    }
    
    // 플레이어가 Trigger 영역에서 벗어났을 때 
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && receiver != null)
        {
            // Receiver의 퇴출 이벤트 실행
            receiver.OnScenarioExit(this);
        }
    }
}
