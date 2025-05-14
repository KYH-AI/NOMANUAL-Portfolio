using UnityEngine;
using UnityEngine.UI;


public class Interactor : MonoBehaviour
{
    [Header("메인카메라 / 인터렉터블 아이템 레이어")]
    public Camera mainCam;
    public LayerMask interactableLayermask = 7; // 상호작용 가능한 객체의 레이어를 설정

    [Header("상호작용 코드 / 상호작용 시 표시될 이미지 정보")]
    public Interactable interactable; // 현재 상호작용 가능한 객체를 저장하는 변수
    
    public Image interactUI; // 인터렉션 가이드 UI

    void Start()
    {
        mainCam = GetComponent<Camera>(); // 시작 시 주 카메라를 현재 카메라로 설정
        interactUI.gameObject.SetActive(false);
    }

    void Update()
    {
        InteractLogic(); // 매 프레임마다 상호작용 로직을 실행
    }

    void InteractLogic()
    {
        RaycastHit hit;
        
        // 주 카메라의 위치에서 정면으로 레이를 쏴 상호작용 가능한 객체를 감지
        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, 1.5f, interactableLayermask))
        {
            Interactable hitInteractable = hit.collider.GetComponent<Interactable>(); // Interactable 스크립트 확인
            
            if (hitInteractable != null)
            {
                interactUI.gameObject.SetActive(true);
                if (interactable == null || interactable.ID != hitInteractable.ID) // 현재 상호작용 가능한 객체가 없거나 새로운 객체라면
                {
                    interactable = hitInteractable; // 현재 상호작용 가능한 객체를 감지한 객체로 변경
                    Debug.Log(hit.collider.name + "is New Item!");
                }

                // 'E' 키를 눌렀을 때 상호작용 함수 호출
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.onInteract.Invoke();
                }
            }
            else
            {
                Debug.Log("DEFAULT State!");
            }
        }
        // 레이캐스트에 물체가 맞지 않을 때
        else
        {
            interactUI.gameObject.SetActive(false);
        }
    }
}