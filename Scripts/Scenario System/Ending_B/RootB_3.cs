using System.Collections;
using HFPS.Player;
using UnityEngine;

public class RootB_3 : MonoBehaviour
{
    [Header("SC ���� ����")] 
    [SerializeField] private GameObject npcObj;
    [SerializeField] private Collider limitArea; // �̺�Ʈ�� ����Ǵ� ���� ������ ���ϰ� ��
    [SerializeField] private Collider scCollider;  // SC Collider
    [SerializeField] private Animator chadAnim;    // Chad �ִϸ�����
    [SerializeField] private Animator doorAnim;    // �� �ִϸ�����
    [SerializeField] private float delayTime = 6f; // scCollider�� isTrigger ������ �ð�
    [SerializeField] private float raycastDistance = 10f; // Raycast �Ÿ�
    [SerializeField] private AudioSource footStep;
    
    private bool eventTriggered = false; // �ߺ� ���� ���� 
    
    private readonly int Open = Animator.StringToHash("Open");
    private readonly int Close = Animator.StringToHash("Close");
    private readonly int ChadMove = Animator.StringToHash("Chad Move");

    private void Start()
    {
        GameObject doorObj = GameObject.Find("New_Hotel Door (215)");
        doorAnim = doorObj.GetComponent<Animator>();
        doorAnim.SetTrigger(Open); // Start �� �� ����
    }

    private void Update()
    {
        CheckRaycastHit(); // �� ������ Raycast �˻�
    }

    // Raycast �˻�
    private void CheckRaycastHit()
    {
        if (eventTriggered) return; // �̺�Ʈ �ߺ� ���� ����

        // �÷��̾� ��ġ���� ī�޶��� ���� �������� Raycast �߻�
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        // Raycast�� scCollider�� ���� ���
        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            if (hit.collider == scCollider)
            {
                eventTriggered = true; // �̺�Ʈ �ߺ� ���� ����
                chadAnim.SetTrigger(ChadMove); // Chad �ִϸ��̼� ����
                StartCoroutine(CloseDoorAfterDelay()); // ������ �� �� �ݱ�
            }
        }
    }

    // ���� �ð� �� �� �ݱ�
    private IEnumerator CloseDoorAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);
        doorAnim.SetTrigger(Close); // �� �ݱ�
        Destroy(limitArea);
        Destroy(npcObj);
    }
    
    public void FootStep()
    {
        footStep.Play();
    }
}