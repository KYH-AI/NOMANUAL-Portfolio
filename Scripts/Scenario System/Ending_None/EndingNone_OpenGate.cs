using System;
using NoManual.Managers;
using UnityEngine;

public class EndingNone_OpenGate : MonoBehaviour
{
    [SerializeField] private Animator gateLeft; 
    [SerializeField] private Animator gateRight; 
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource[] bgmAudios;

    private void Awake()
    {
        FadeAudio(true);
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
            
            // �� ���� �ִϸ��̼� Ʈ���� ����
            gateLeft.SetTrigger("Open");
            gateRight.SetTrigger("Open");

            // SFX ���
            if (audioSource != null)
            {
                audioSource.Play();
            }

            FadeAudio(false);
        }
    }

    private void FadeAudio(bool isFadeIn)
    {
        float endValue = isFadeIn ? 0.5f : 0f;
        foreach (var audio in bgmAudios)
        {
            DOTweenManager.FadeAudioSource(isFadeIn, audio, 1f, endValue, null);
        }
    }
}

