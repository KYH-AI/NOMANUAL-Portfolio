using System;
using Cinemachine;
using NoManual.Managers;
using UnityEngine;
using UnityEngine.Playables;

public class EndingNone : MonoBehaviour
{
    [SerializeField] private bool debugMode;
    
    [Header("======== 게이트 ========")]
    [SerializeField] private Animator gateLeft; 
    [SerializeField] private Animator gateRight; 
    [SerializeField] private AudioSource audioSource;

    [Space(10)] [Header("======== 시네머신 라인 ========")]
    private bool _starTrackLine;
    [SerializeField] private PlayableDirector timeLinePlayable;
    [SerializeField] private Camera trackMainCamera;
    [SerializeField] private CinemachineVirtualCamera trackVirtualCamera;
    [SerializeField] private CinemachineDollyCart dollyCart;
    [SerializeField] private Transform gamiginTeleportPos;

    [Space(10)] 
    [Header("======== 캐릭터 모델 ========")] 
    [SerializeField] private GameObject gamigin;
    [SerializeField] private GameObject playerModel;
    [SerializeField] private GameObject leftCultLeader;
    [SerializeField] private GameObject rightCultLeader;


    private void Awake()
    {
        trackMainCamera.gameObject.SetActive(false);
        dollyCart.gameObject.SetActive(false);
        
        
        playerModel.gameObject.SetActive(false);
        leftCultLeader.gameObject.SetActive(false);
        rightCultLeader.gameObject.SetActive(false);
        
    }



    public void DoorTrigger()
    {
        // 문 열기 애니메이션 트리거 설정
        gateLeft.SetTrigger("Open");
        gateRight.SetTrigger("Open");

        // SFX 재생
        if (audioSource != null)
        {
            audioSource.Play();
        }
            
        timeLinePlayable.Play();
    }

    public void HidePlayer()
    {
        NoManualHotelManager.Instance.DisablePlayer();
        HFPS.Player.PlayerController.Instance.gameObject.SetActive(false);
        trackMainCamera.gameObject.SetActive(true); 
     //   trackVirtualCamera.gameObject.SetActive(true);
      
    }

    public void PlayDollyCart()
    {
        dollyCart.gameObject.SetActive(true);
    }

    public void PullingStep()
    {
        playerModel.gameObject.SetActive(true);
        leftCultLeader.gameObject.SetActive(true);
        rightCultLeader.gameObject.SetActive(true);
        ShakeVirtualCamera();
    }

    public void ShakeVirtualCamera()
    {
        var shake = trackVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        shake.m_AmplitudeGain = 0.2f;
        shake.m_FrequencyGain = 0.08f;
    }

    public void GamiginTeleportToPlayer()
    {
        gamigin.transform.position = gamiginTeleportPos.position;
        Animator gamiginAnimator = gamigin.GetComponent<Animator>();
        gamiginAnimator.SetLayerWeight(1, 0.75f);
    }

    public void GamiginKniftStabbingPlayer()
    {
        gamigin.GetComponent<Animator>().CrossFade("Stabbing", 0.2f);
    }

    public void FadeOutAudio(string audioName)
    {
        
    }

    public void SceneMove()
    {
#if !UNITY_EDITOR
    debugMode = false;
#endif        
        if(!debugMode) GameManager.Instance.SaveGameManager.CurrentPlayerSaveData.Day++;
        Monologue.EndingCredit = true;
        NoManualHotelManager.Instance.SceneMove(false, false, GameManager.SceneName.Monologue, 0f);
    }
}
