using HFPS.Player;
using UnityEngine;
using NoManual.ANO;
using NoManual.Managers;
using System.Collections;

public class ANO47_PianoPlay : ANO_Component
{
    [Header("ANO 설정")]
    [SerializeField] private Collider anoStart;    // 플레이어가 들어가야 하는 영역
    [SerializeField] private GameObject[] anoObjs; // 미리 배치된 ANO 오브젝트들
    [SerializeField] private AudioSource anoBGM;   // ANO 배경음악
    [SerializeField] private AudioSource anoSfx0;  // ANO 효과음 0 (데미지 발생 시 재생)
    [SerializeField] private AudioSource anoSfx1;  // ANO 효과음 1 (플레이어 진입 시 재생)

    private Coroutine damageCoroutine;             // 데미지 코루틴
    private bool isEffectTriggered = false;        // 효과가 한번 발동되면 다시 발동되지 않음
    private bool isPlayerInAnoStart = false;       // 플레이어가 anoStart에 있는지 여부

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart && !isEffectTriggered)
        {
            isEffectTriggered = true;
            isPlayerInAnoStart = true;
            
            // BGM 재생 시작
            anoBGM.Play();
            
            StartCoroutine(FadeInAudio(anoSfx1, 3f));
            StartCoroutine(FadeInAudio(anoSfx0, 3f));
            
            StartCoroutine(StartEffectAfterDelay(3f));
        }
    }

    // 10초 후 효과 발동
    private IEnumerator StartEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 10초 후에도 플레이어가 여전히 anoStart 안에 있는지 확인
        if (IsPlayerInAnoStart())
        {
            // 미리 배치된 ANO 오브젝트들 활성화
            foreach (var obj in anoObjs)
            {
                obj.SetActive(true);
            }

            // 점프 스케어 효과 발동
            NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(3, 0f);

            // BGM 중지
            anoBGM.Stop();

            // 지속적인 데미지 코루틴 시작 (플레이어가 영역에 있는 동안)
            if (isPlayerInAnoStart)
            {
                damageCoroutine = StartCoroutine(ApplyDamageOverTime());
            }
        }
    }

    // 지속적으로 플레이어에게 정신력 데미지 적용 (플레이어가 영역에 있을 때만)
    private IEnumerator ApplyDamageOverTime()
    {
        while (isPlayerInAnoStart)
        {
            PlayerController.Instance.DecreaseMentality(3); // 1초마다 3의 정신력 데미지 적용
            yield return new WaitForSeconds(1f);
        }
    }

    // 플레이어가 영역을 떠나면 데미지 코루틴 중지
    private void Update()
    {
        if (isEffectTriggered && !IsPlayerInAnoStart())
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }

            isPlayerInAnoStart = false;
        }
    }

    // 플레이어가 anoStart 영역에 있는지 확인하는 함수
    private bool IsPlayerInAnoStart()
    {
        // anoStart 영역 안에 플레이어가 있는지 확인
        return anoStart.bounds.Contains(PlayerController.Instance.transform.position);
    }

    // 스크립트 비활성화 시 코루틴 정지
    private void OnDisable()
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    // anoSfx0의 볼륨을 서서히 0에서 1로 증가시키는 함수
    private IEnumerator FadeInAudio(AudioSource audioSource, float duration)
    {
        audioSource.volume = 0f;
        audioSource.Play();
        float currentTime = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, 1f, currentTime / duration);
            yield return null;
        }

        audioSource.volume = 1f;
    }
}
