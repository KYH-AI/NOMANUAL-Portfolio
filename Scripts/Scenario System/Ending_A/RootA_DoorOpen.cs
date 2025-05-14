
using UnityEngine;

public class RootA_DoorOpen : MonoBehaviour
{
    [SerializeField] private Animator doorAnim;


    private void Start()
    {
        GameObject door = GameObject.Find("New_Hotel Door (VIP)");
        doorAnim = door.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        doorAnim.SetTrigger("Open");
    }
}
