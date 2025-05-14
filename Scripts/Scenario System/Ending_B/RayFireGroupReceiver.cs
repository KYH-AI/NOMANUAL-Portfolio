using RayFire;
using UnityEngine;
using UnityEngine.Playables;

public class RayFireGroupReceiver : MonoBehaviour, INotificationReceiver
{
    [Header("======== RayFire �׷� ========")]
    [SerializeField] private RayfireRigid[] rayFireGroup;
    
    [Header("======== RayFire ������ �׷� ========")]
    [SerializeField] private AudioSource[] rayFireAudioGroup;
    public virtual void PlayRayFire()
    {
        for (int i = 0; i < rayFireGroup.Length; i++)      rayFireGroup[i].Initialize();
        for (int i = 0; i < rayFireAudioGroup.Length; i++)      rayFireAudioGroup[i].Play();
    }

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        // Marker�� RayFireMarker���� Ȯ��
        if (notification is RayFireMarker)
        {
            PlayRayFire();
        }
    }
}

