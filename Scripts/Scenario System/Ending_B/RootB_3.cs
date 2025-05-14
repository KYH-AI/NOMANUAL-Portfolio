using System.Collections;
using HFPS.Player;
using UnityEngine;

public class RootB_3 : MonoBehaviour
{
    [Header("SC 연출 설정")] 
    [SerializeField] private GameObject npcObj;
    [SerializeField] private Collider limitArea; // 이벤트가 실행되는 동안 들어오지 못하게 함
    [SerializeField] private Collider scCollider;  // SC Collider
    [SerializeField] private Animator chadAnim;    // Chad 애니메이터
    [SerializeField] private Animator doorAnim;    // 문 애니메이터
    [SerializeField] private float delayTime = 6f; // scCollider의 isTrigger 딜레이 시간
    [SerializeField] private float raycastDistance = 10f; // Raycast 거리
    [SerializeField] private AudioSource footStep;
    
    private bool eventTriggered = false; // 중복 실행 방지 
    
    private readonly int Open = Animator.StringToHash("Open");
    private readonly int Close = Animator.StringToHash("Close");
    private readonly int ChadMove = Animator.StringToHash("Chad Move");

    private void Start()
    {
        GameObject doorObj = GameObject.Find("New_Hotel Door (215)");
        doorAnim = doorObj.GetComponent<Animator>();
        doorAnim.SetTrigger(Open); // Start 시 문 열림
    }

    private void Update()
    {
        CheckRaycastHit(); // 매 프레임 Raycast 검사
    }

    // Raycast 검사
    private void CheckRaycastHit()
    {
        if (eventTriggered) return; // 이벤트 중복 실행 방지

        // 플레이어 위치에서 카메라의 정면 방향으로 Raycast 발사
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        // Raycast가 scCollider에 닿을 경우
        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            if (hit.collider == scCollider)
            {
                eventTriggered = true; // 이벤트 중복 실행 방지
                chadAnim.SetTrigger(ChadMove); // Chad 애니메이션 시작
                StartCoroutine(CloseDoorAfterDelay()); // 딜레이 후 문 닫기
            }
        }
    }

    // 일정 시간 후 문 닫기
    private IEnumerator CloseDoorAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);
        doorAnim.SetTrigger(Close); // 문 닫기
        Destroy(limitArea);
        Destroy(npcObj);
    }
    
    public void FootStep()
    {
        footStep.Play();
    }
}