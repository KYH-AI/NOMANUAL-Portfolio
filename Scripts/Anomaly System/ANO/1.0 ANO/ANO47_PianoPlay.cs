using HFPS.Player;
using UnityEngine;
using NoManual.ANO;
using NoManual.Managers;
using System.Collections;

public class ANO47_PianoPlay : ANO_Component
{
    [Header("ANO ����")]
    [SerializeField] private Collider anoStart;    // �÷��̾ ���� �ϴ� ����
    [SerializeField] private GameObject[] anoObjs; // �̸� ��ġ�� ANO ������Ʈ��
    [SerializeField] private AudioSource anoBGM;   // ANO �������
    [SerializeField] private AudioSource anoSfx0;  // ANO ȿ���� 0 (������ �߻� �� ���)
    [SerializeField] private AudioSource anoSfx1;  // ANO ȿ���� 1 (�÷��̾� ���� �� ���)

    private Coroutine damageCoroutine;             // ������ �ڷ�ƾ
    private bool isEffectTriggered = false;        // ȿ���� �ѹ� �ߵ��Ǹ� �ٽ� �ߵ����� ����
    private bool isPlayerInAnoStart = false;       // �÷��̾ anoStart�� �ִ��� ����

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart && !isEffectTriggered)
        {
            isEffectTriggered = true;
            isPlayerInAnoStart = true;
            
            // BGM ��� ����
            anoBGM.Play();
            
            StartCoroutine(FadeInAudio(anoSfx1, 3f));
            StartCoroutine(FadeInAudio(anoSfx0, 3f));
            
            StartCoroutine(StartEffectAfterDelay(3f));
        }
    }

    // 10�� �� ȿ�� �ߵ�
    private IEnumerator StartEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 10�� �Ŀ��� �÷��̾ ������ anoStart �ȿ� �ִ��� Ȯ��
        if (IsPlayerInAnoStart())
        {
            // �̸� ��ġ�� ANO ������Ʈ�� Ȱ��ȭ
            foreach (var obj in anoObjs)
            {
                obj.SetActive(true);
            }

            // ���� ���ɾ� ȿ�� �ߵ�
            NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(3, 0f);

            // BGM ����
            anoBGM.Stop();

            // �������� ������ �ڷ�ƾ ���� (�÷��̾ ������ �ִ� ����)
            if (isPlayerInAnoStart)
            {
                damageCoroutine = StartCoroutine(ApplyDamageOverTime());
            }
        }
    }

    // ���������� �÷��̾�� ���ŷ� ������ ���� (�÷��̾ ������ ���� ����)
    private IEnumerator ApplyDamageOverTime()
    {
        while (isPlayerInAnoStart)
        {
            PlayerController.Instance.DecreaseMentality(3); // 1�ʸ��� 3�� ���ŷ� ������ ����
            yield return new WaitForSeconds(1f);
        }
    }

    // �÷��̾ ������ ������ ������ �ڷ�ƾ ����
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

    // �÷��̾ anoStart ������ �ִ��� Ȯ���ϴ� �Լ�
    private bool IsPlayerInAnoStart()
    {
        // anoStart ���� �ȿ� �÷��̾ �ִ��� Ȯ��
        return anoStart.bounds.Contains(PlayerController.Instance.transform.position);
    }

    // ��ũ��Ʈ ��Ȱ��ȭ �� �ڷ�ƾ ����
    private void OnDisable()
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    // anoSfx0�� ������ ������ 0���� 1�� ������Ű�� �Լ�
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
