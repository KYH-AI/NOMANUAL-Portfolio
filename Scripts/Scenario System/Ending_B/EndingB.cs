using System;
using System.Collections;
using UnityEngine;
using Cinemachine;
using NoManual.Managers;
using UnityEngine.Playables;

public class EndingB : MonoBehaviour
{
  [SerializeField] private bool debugMode;
    
    [Header("======== 게이트 ========")]
    [SerializeField] private Animation gate;
    [SerializeField] private AudioSource audioSource;

    [Space(10)] [Header("======== 시네머신 라인 ========")]
    [SerializeField] private PlayableDirector timeLinePlayable;
    [SerializeField] private Camera trackMainCamera;
    [SerializeField] private CinemachineVirtualCamera trackVirtualCamera;
    [SerializeField] private CinemachineDollyCart dollyCart;

    [Space(10)] 
    [Header("======== RayFire 그룹 ========")]
    [SerializeField] private RayFireGroupReceiver[] rayFireGroup;

    [Space(10)] 
    [Header("======== 카메라 쉐이크 ========")]
    [SerializeField] private NoiseSettings bigShakeNoiseSettings;
    [SerializeField] private NoiseSettings normalNoiseSettings;
    private CinemachineBasicMultiChannelPerlin _shakeCam;
    private Coroutine _shakeStopCoroutine = null;
    
    private void Awake()
    {
        trackMainCamera.gameObject.SetActive(false);
        dollyCart.gameObject.SetActive(false);
        _shakeCam = trackVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        NormalShakeVirtualCamera();
    }
    

    public void DoorTrigger()
    {
        timeLinePlayable.Play();
    }

    public void DoorAnim()
    {
        // 문 열기 애니메이션 트리거 설정
        gate.Play();

        // 문 SFX 재생
        if (audioSource != null)
        {
            audioSource.Play();
        }
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
    
    private void ShakeVirtualCamera(float amplitude, float frequency, bool useBigShakeProfile = false)
    {
        CheckStopShakeCamCoroutine();
        
        _shakeCam.m_NoiseProfile = useBigShakeProfile ? bigShakeNoiseSettings : normalNoiseSettings;
        
        _shakeCam.m_AmplitudeGain = amplitude;
        _shakeCam.m_FrequencyGain = frequency;
    }
    
    public void NormalShakeVirtualCamera()
    {
        ShakeVirtualCamera(0.5f, 1f);
    }
    
    public void BigShakeVirtualCamera()
    {
        ShakeVirtualCamera(7f, 10f, true);
    }
    
    public void VeryBigShakeVirtualCamera()
    {
        ShakeVirtualCamera(12f, 15f, true);
    }
    
    public void StopShakeVirtualCamera()
    {
        CheckStopShakeCamCoroutine();
        _shakeStopCoroutine = StartCoroutine(SmoothStopShake(1f));
    }
    
    private void CheckStopShakeCamCoroutine()
    {
        if (_shakeStopCoroutine != null)
        {
            StopCoroutine(_shakeStopCoroutine);
            _shakeStopCoroutine = null;
        }
    }
    


    private IEnumerator SmoothStopShake(float duration)
    {
        float elapsed = 0f;
        float initialAmplitude = _shakeCam.m_AmplitudeGain;
        float initialFrequency = _shakeCam.m_FrequencyGain;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            _shakeCam.m_AmplitudeGain = Mathf.Lerp(initialAmplitude, 0f, t);
            _shakeCam.m_FrequencyGain = Mathf.Lerp(initialFrequency, 0f, t);

            yield return null;
        }
        
        _shakeCam.m_AmplitudeGain = 0f;
        _shakeCam.m_FrequencyGain = 0f;
        
        _shakeStopCoroutine = null;
        NormalShakeVirtualCamera();
    }
    
    public void RayFireGroupPlay(int groupId)
    {
        rayFireGroup[groupId].PlayRayFire();        
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
