using System.Collections;
using UnityEngine;

public class Doors : MonoBehaviour
{
    private Animator doorAnim;
    private Interactable interactable;
    
    // 문 상호작용 딜레이 (코루틴)
    private WaitForSeconds delay = new WaitForSeconds(2.5f);
    private bool isDelayed = false;
    private bool isOpened = false;
    
    // 애니메이터 변수명
    [SerializeField]
    public bool isLocked = false;
    
    private static readonly int doorInteract = Animator.StringToHash("doorInteract");
    private static readonly int isOpen = Animator.StringToHash("isOpen");
    private static readonly int doorLocked = Animator.StringToHash("doorLocked");

    // 애니메이터 안의 변수들 readonly 변수

    private void Start()
    {
        interactable = GetComponentInChildren<Interactable>();
        doorAnim = GetComponent<Animator>();

        // Interactable 스크립트 객체의 onInteract 이벤트에 Subscribe(구독)
        if (interactable != null)
        {
            interactable.onInteract.AddListener(DoorControl);
        }
    }

    void DoorControl()
    {
        
        if (!isOpened && !isDelayed && !isLocked)
        {
            isOpened = !isOpened;
            doorAnim.SetTrigger(doorInteract);
            doorAnim.SetBool(isOpen, true);
            StartCoroutine(nameof(DoorDelay));
        }
        else if(isOpened && !isDelayed && !isLocked)
        {
            isOpened = !isOpened;
            doorAnim.SetTrigger(doorInteract);
            doorAnim.SetBool(isOpen, false);
            StartCoroutine(nameof(DoorDelay));
        }
        
        else if (!isOpened && !isDelayed && isLocked)
        {
            doorAnim.SetTrigger(doorLocked);
            StartCoroutine(nameof(DoorDelay));
        }
    }

    IEnumerator DoorDelay()
    {
        isDelayed = true;
        yield return delay;
        isDelayed = false;
    }
}