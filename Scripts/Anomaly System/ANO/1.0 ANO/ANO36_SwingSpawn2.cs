using System;
using System.Collections;
using HFPS.Player;
using NoManual.ANO;
using UnityEngine;
using Random = UnityEngine.Random;

public class ANO36_SwingSpawn2 : ANO_Component
{
    [Header("ANO 설정")] 
    [SerializeField] private Collider anoStart;
    [SerializeField] private Collider[] anoEnds;
    [SerializeField] private AudioSource anoSfx0;
    [SerializeField] private AudioSource anoSfx1;
    [SerializeField] private AudioSource anoBGM;
    [SerializeField] private Animator anoAnim;
    
    [Header("Swing Metal 효과음")] 
    [SerializeField] private AudioClip[] swingMetalSfx;

    private ANO_DataScriptable anoData;
    private Coroutine damageCoroutine;
    private Coroutine randSwingSfxCoroutine;
    private readonly int Trigger1036 = Animator.StringToHash("isSwing");


    private float anoSfx1Volume;
    private float anoBgmVolume;

    private void Start()
    {
        anoSfx1Volume = anoSfx1.volume;
        anoBgmVolume = anoBGM.volume;
        Debug.Log("Mentality Damage : " + anoData.MentalityDamage);
    }

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            StopAllCoroutines();

            anoSfx1.volume = anoSfx1Volume;
            anoBGM.volume = anoBgmVolume;
            anoSfx1.Play();
            anoBGM.Play();
            
            randSwingSfxCoroutine = StartCoroutine(PlayRandomSwingMetalSfx()); 
            damageCoroutine = StartCoroutine(DamagePerSecond());
            
            anoAnim.SetTrigger(Trigger1036);
        }
        else if (anoTriggerZone == anoEnds[0] || anoTriggerZone == anoEnds[1])
        {
            if (damageCoroutine == null) return;
            
            // 플레이어가 anoStart를 벗어날 경우
            if (damageCoroutine != null)
            {
                
                StopCoroutine(damageCoroutine);
                StopCoroutine(randSwingSfxCoroutine);
                damageCoroutine = null;
                randSwingSfxCoroutine = null;
                
                // SFX 및 BGM 페이드 아웃
                StartCoroutine(FadeOutAudio(anoSfx0));
                StartCoroutine(FadeOutAudio(anoSfx1));
                StartCoroutine(FadeOutAudio(anoBGM));
            }

        }
    }

    private IEnumerator PlayRandomSwingMetalSfx()
    {
        while (true)
        {
            int randomIndex = Random.Range(0, swingMetalSfx.Length); // 랜덤 인덱스 선택
            AudioClip selectedClip = swingMetalSfx[randomIndex]; // 랜덤 클립 선택

            anoSfx0.clip = selectedClip;
            anoSfx0.Play();
            
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator DamagePerSecond()
    {
        while (true)
        {
            PlayerController.Instance.DecreaseMentality(5);
            yield return new WaitForSeconds(1.5f);
        }
    }

    private IEnumerator FadeOutAudio(AudioSource audioSource)
    {
        float startVolume = audioSource.volume;

        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}
