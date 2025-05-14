using RayFire;
using UnityEngine;
using UnityEngine.Playables;

public class RayFireGroupReceiver : MonoBehaviour, INotificationReceiver
{
    [Header("======== RayFire 그룹 ========")]
    [SerializeField] private RayfireRigid[] rayFireGroup;
    
    [Header("======== RayFire 오디으 그룹 ========")]
    [SerializeField] private AudioSource[] rayFireAudioGroup;
    public virtual void PlayRayFire()
    {
        for (int i = 0; i < rayFireGroup.Length; i++)      rayFireGroup[i].Initialize();
        for (int i = 0; i < rayFireAudioGroup.Length; i++)      rayFireAudioGroup[i].Play();
    }

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        // Marker가 RayFireMarker인지 확인
        if (notification is RayFireMarker)
        {
            PlayRayFire();
        }
    }
}

