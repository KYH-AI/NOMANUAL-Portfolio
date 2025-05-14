using HFPS.Player;
using HFPS.Systems;
using UnityEngine;
using NoManual.ANO;
using NoManual.Managers;

public class ANO37_BlinkLights : ANO_Component
{
    [Header("ANO ����")]
    [SerializeField] private GameObject[] anoLights;  // �����̴� ��
    [SerializeField] private Collider[] anoStarts;    // ���� Ʈ����
    [SerializeField] private Collider[] anoEnds;      // ���� Ʈ����
    [SerializeField] private GameObject anoNpc;       // NPC ������
    [SerializeField] private AudioSource anoSfx;      // NPC ���� �������� Ambience

    private bool isNpcSpawned = false; // NPC�� �����Ǿ����� ���θ� �����ϴ� ����
    private float timeInEndZone = 0f;
    private float timeToSpawnNpc;
    
    private Transform player;  // �÷��̾ �����ϱ� ���� ����
    
    private void Start()
    {
        // ó�� ���� �� ���� NPC ���� �ð� ����
        SetRandomTimeToSpawnNpc();
        
        // �÷��̾� ��ü�� ã�� (Player �±׷� ����)
        player = ScriptManager.Instance.gameObject.transform;
    }

    private void Update()
    {
        // NPC�� ���� �������� �ʾҰ�, �÷��̾ EndZone�� ���� ���� üũ
        if (!isNpcSpawned && IsPlayerInEndZone())
        {
            timeInEndZone += Time.deltaTime;

            // �÷��̾ ���� �ð� ���� �ӹ��� NPC�� ����
            if (timeInEndZone >= timeToSpawnNpc)
            {
                SpawnNpc();
                isNpcSpawned = true;  // NPC�� �� ���� �����ǵ��� ����
            }
        }
        else
        {
            // �÷��̾ anoEnds ������ ����� Ÿ�̸Ӹ� �ʱ�ȭ
            if (timeInEndZone > 0)
            {
                anoSfx.Stop();
            }
            timeInEndZone = 0f;
        }
    }

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        // anoStarts �迭 �� �ش� �ݶ��̴��� ���� ��, ���� �ε����� anoLights �ִϸ��̼� ���
        /*
        for (int i = 0; i < anoStarts.Length; i++)
        {
            if (anoTriggerZone == anoStarts[i])
            {
                Debug.Log($"anoStarts[{i}]�� �÷��̾ �����߽��ϴ�. �ִϸ��̼� ����� �����մϴ�.");
            }
        }
        */
        // NPC ���� Ÿ�̸Ӱ� Ȱ��ȭ�� ���·� ����
        SetRandomTimeToSpawnNpc();  // ���ο� ���� �ð��� ����
    }

    private void SpawnNpc()
    {
        if (anoNpc != null && player != null)
        {
            GameObject spawnedNpc = Instantiate(anoNpc,player,true);
            spawnedNpc.transform.localPosition = new Vector3(0f, -2f, 0.75f);
            spawnedNpc.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            
            
            // !! �������ɾ� ��� �� Damage !!
            NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(4, 0f);
            PlayerController.Instance.DecreaseMentality(20);

            // 1�� �ڿ� NPC ����
            Destroy(spawnedNpc, 1f);
            isNpcSpawned = false;  // ���ο� NPC ������ �����ϵ��� �ʱ�ȭ
        }
    }

    // �÷��̾ anoEnds �� �ϳ��� �ִ��� Ȯ���ϴ� �Լ�
    private bool IsPlayerInEndZone()
    {
        foreach (Collider end in anoEnds)
        {
            if (end.bounds.Contains(player.transform.position))
            {
                return true;
            }
        }
        return false;
    }

    // NPC ������ ���� ���� �ð��� �����ϴ� �޼ҵ�
    private void SetRandomTimeToSpawnNpc()
    {
        timeToSpawnNpc = Random.Range(1.5f, 3f);  // 2�ʿ��� 3�� ������ ���� �� ����
    }
}
