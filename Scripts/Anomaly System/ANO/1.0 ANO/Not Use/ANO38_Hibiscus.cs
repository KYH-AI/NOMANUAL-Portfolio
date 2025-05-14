using System.Collections;
using System.Collections.Generic;
using HFPS.Player;
using NoManual.ANO;
using NoManual.Managers;
using UnityEngine;

public class ANO38_Hibiscus : ANO_Component
{
    [Header("ANO ����")]
    [SerializeField] private GameObject[] anoLights;
    [SerializeField] private Collider[] anoStarts;
    [SerializeField] private Collider[] anoDeters;
    [SerializeField] private Collider[] anoSolves;
    [SerializeField] private GameObject[] anoNpcs;

    [SerializeField] private Animator anoLightsAnim;
    [SerializeField] private Animator[] anoNpcsAnim; // �迭�� ����

    private bool isMoving;
    private bool isLighting;
    private Vector3 lastPlayerPosition;

    private Coroutine lightCoroutine;

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        // anoStart�� ���� �� ���� ����
        for (int i = 0; i < anoStarts.Length; i++)
        {
            if (anoTriggerZone == anoStarts[i])
            {
                // ���ʷ� LightOn Trigger
                anoLightsAnim.SetTrigger("LightOn");
                // anoStart ��Ȱ��ȭ
                anoStarts[i].enabled = false;

                // ����Ʈ ���� ����
                if (lightCoroutine != null)
                    StopCoroutine(lightCoroutine);
                lightCoroutine = StartCoroutine(ToggleLights());

                // �÷��̾� �ʱ� ��ġ ����
                lastPlayerPosition = PlayerController.Instance.transform.position;
            }
        }

        // �÷��̾��� ������ �˻�
        for (int i = 0; i < anoDeters.Length; i++)
        {
            if (anoTriggerZone == anoDeters[i])
            {
                if (isMoving && isLighting && Vector3.Distance(PlayerController.Instance.transform.position, lastPlayerPosition) > 0.1f)
                {
                    // �ش� NPC�� Animator�� Running Trigger ��ȣ
                    if (i < anoNpcsAnim.Length) // �迭 ���� �˻�
                    {
                        anoNpcsAnim[i].SetTrigger("Running");
                        // �������ɾ� �� ��Ż ������ ó��
                        NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(3, 0);
                        PlayerController.Instance.DecreaseMentality(20);
                        Debug.Log($"�÷��̾ {i}��° deter�� ���� �� NPC�� �����߽��ϴ�. �������ɾ� �߻� �� ��Ż ������!");
                    }
                }
            }
        }

        // anoSolves ó��
        for (int i = 0; i < anoSolves.Length; i++)
        {
            if (anoTriggerZone == anoSolves[i])
            {
                // �ش��ϴ� NPC ����
                if (i < anoNpcs.Length && anoNpcs[i] != null)
                {
                    Destroy(anoNpcs[i]);
                    anoNpcs[i] = null; // ������ null�� �����Ͽ� ����
                    Debug.Log($"{i}��° NPC�� ���ŵǾ����ϴ�.");
                }
            }
        }
    }

    private IEnumerator ToggleLights()
    {
        while (true)
        {
            // ������ �ð� ���
            float waitTime = Random.Range(5f, 10f);
            yield return new WaitForSeconds(waitTime);
            
            // ����Ʈ ���� �� �ҵ�
            if (isLighting)
            {
                anoLightsAnim.SetTrigger("LightOff");
                isLighting = false;
            }
            else
            {
                anoLightsAnim.SetTrigger("LightOn");
                isLighting = true;
            }
        }
    }

    void Update()
    {
        // �÷��̾��� ������ üũ
        isMoving = PlayerController.Instance.transform.position != lastPlayerPosition;
        if (isMoving)
        {
            lastPlayerPosition = PlayerController.Instance.transform.position;
        }
    }
}
