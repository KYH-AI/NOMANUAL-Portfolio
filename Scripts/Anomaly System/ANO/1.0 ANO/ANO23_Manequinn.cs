using HFPS.Player;
using UnityEngine;
using NoManual.ANO;
using NoManual.Managers;

public class ANO23_Manequinn : ANO_Component
{
    [Header("ANO ")] 
    [SerializeField] private Collider[] anoStart;  // ANO ݶ̴
    [SerializeField] private GameObject[] anoObj;   // ŷ Ʈ
    [SerializeField] private AudioSource anoSfx;     // ȿ
    [SerializeField] private float raycastDistance = 10f; // ĳƮ Ÿ

    private Camera mainCamera; //  ī޶
    private bool isJumpscared = false; // ɾ
    private RaycastHit hit; // 캐싱용 RaycastHit 추가
    private Ray ray; // 캐싱용 Ray 추가

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        for(int i = 0; i < anoStart.Length; i++)
        {   // 중괄호 추가
            if (anoTriggerZone == anoStart[i])
            {
                ActivateManequin(i);
            }
        }
    }

    private void Update()
    {
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            for(int i = 0; i < anoObj.Length; i++)
            {   // 중괄호 추가
                if (!isJumpscared && hit.collider == anoObj[i].GetComponent<Collider>())
                {
                    NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(2, 0f);
                    PlayerController.Instance.DecreaseMentality(10);
                    isJumpscared = true; // ɾ 
                }
            }
        }
    }

    private void ActivateManequin(int index)
    {
        // ŷ Ȱȭ 
        anoObj[index].SetActive(true);

        for (int i = 0; i < anoStart.Length; i++)
        {
            anoStart[i].enabled = false;
        }

        Rigidbody rb = anoObj[index].AddComponent<Rigidbody>();
        rb.AddForce(Vector3.forward, ForceMode.Impulse);
    }
    
    
}
