using UnityEngine;

namespace NoManual.Tutorial
{
    public class HandoverTriggerSender : MonoBehaviour
    {
        [SerializeField] private HandoverStep receiver;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                receiver.OnTriggerEvent(GetComponent<Collider>());
            }
        }
    }
}


