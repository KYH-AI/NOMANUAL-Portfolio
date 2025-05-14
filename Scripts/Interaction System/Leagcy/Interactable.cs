using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    private GameObject objPlayer;
    private Image uiUse;
    public Image prefabFloatUI;
    
    // UI 띄우는 범위
    [SerializeField] float isPlayerNearByRange = 2.5f;
    // 상호작용 가능한 범위
    [SerializeField] private float isPlayerInteractRange = 1.5f;
    
    public UnityEvent onInteract;
    public int ID;
    
    void Start()
    {
        ID = Random.Range(0, 999999);
        objPlayer = GameObject.FindGameObjectWithTag("Player");

    }

    void Update()
    {
        InteractableUIUpdate();
        float distFromPlayer = Vector3.Distance(transform.position, objPlayer.transform.position);

        // 플레이어와의 거리가 설정 범위 내에 있을 때만 UI 생성/업데이트 및 Fade 처리
        if (distFromPlayer <= isPlayerNearByRange)
        {
            if (uiUse == null)
            {
                CreateFloatingIcon();
            }
            else if(uiUse != null)
            {
                UpdateUIOptions(distFromPlayer);
            }
        }
        else if (distFromPlayer >= isPlayerNearByRange && uiUse != null)
        {
            DestroyFloatingIcon();
        }
        
        // 플레이어와의 거리가 설정 범위 내에 있으며 'E' 키를 눌렀을 때 onInteract 이벤트 발생
        if (distFromPlayer <= isPlayerInteractRange && Input.GetKeyDown(KeyCode.E))
        {
            onInteract.Invoke();
        }
    }

    void InteractableUIUpdate()
    {
        
    }
    
    void CreateFloatingIcon()
    {
        uiUse = Instantiate(prefabFloatUI, FindObjectOfType<Canvas>().transform).GetComponent<Image>();
        uiUse.transform.position = Camera.main.WorldToScreenPoint(transform.position);
    }

    void UpdateUIOptions(float distFromPlayer)
    {
        // UI 포지션 설정(거리가 가까울수록 UI Scale 확대)
        uiUse.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        float clampedDist = Mathf.Clamp(distFromPlayer, 1.0f, 1.7f);
        uiUse.transform.localScale = new Vector3(clampedDist, clampedDist, 0);
        
        // 거리에 따른 투명도 설정 (거리가 가까울수록 투명도가 낮아짐)
        float alpha = Mathf.Lerp(2.0f, 0f, Mathf.InverseLerp(0.0f, isPlayerNearByRange, distFromPlayer));
        Color color = uiUse.color;
        color.a = alpha;
        uiUse.color = color;
    }

    void DestroyFloatingIcon()
    {
        Destroy(uiUse.gameObject);
    }
}

