using UnityEngine;

[System.Serializable]
public class TextScript
{
    [Header("��� NPC CSV")]  public string TextKey = string.Empty;
    [Header("��� ���� Ŭ��")] public AudioClip VoiceClip  = null;
    [Header("��� ����� �ҽ� (��������)")] public AudioSource Voice3dAudioSource = null;
    [Header("�ؽ�Ʈ �߰� ���� �ð� (��ũ)")] public float Offset = 2f;
}
