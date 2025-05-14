using UnityEngine;

[System.Serializable]
public class ANO_Audio_Module 
{
    [Header("����� ����")]
    public AudioSource audioSource;
    
    [Header("�Ҹ�")]
    [Range(0f, 1f)] public float audioVolumeSize;

    [Header("�Ҹ� ����")]
    public float minVolumeDistance;
    public float maxVolumeDistance;

    [Header("����� Roll off Mode")] 
    public AudioRolloffMode audioRolloffMode;

    [Header("����� Ŭ�� ����")] 
    public AudioClip audioClip;

    [Header("����� ����")] 
    [Range(0f, 1f)] public float audioBlend;

    [Header("����� ���� ���")] 
    [HideInInspector] public bool loop = false;   
    [HideInInspector] public bool oneShot  = false;   
}
