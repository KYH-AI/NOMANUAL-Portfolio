using UnityEngine;
using UnityEngine.Events;

public class EndingDoorTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent triggerEvent;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            this.gameObject.GetComponent<Collider>().enabled = false;
            triggerEvent?.Invoke();
        }
    }
}
