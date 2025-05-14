using HFPS.Player;
using HFPS.Systems;
using UnityEngine;
using NoManual.ANO;
using NoManual.Managers;

public class ANO37_BlinkLights : ANO_Component
{
    [Header("ANO 설정")]
    [SerializeField] private GameObject[] anoLights;  // 깜빡이는 불
    [SerializeField] private Collider[] anoStarts;    // 시작 트리거
    [SerializeField] private Collider[] anoEnds;      // 종료 트리거
    [SerializeField] private GameObject anoNpc;       // NPC 프리팹
    [SerializeField] private AudioSource anoSfx;      // NPC 등장 전까지의 Ambience

    private bool isNpcSpawned = false; // NPC가 생성되었는지 여부를 추적하는 변수
    private float timeInEndZone = 0f;
    private float timeToSpawnNpc;
    
    private Transform player;  // 플레이어를 추적하기 위한 변수
    
    private void Start()
    {
        // 처음 시작 시 랜덤 NPC 스폰 시간 설정
        SetRandomTimeToSpawnNpc();
        
        // 플레이어 객체를 찾기 (Player 태그로 가정)
        player = ScriptManager.Instance.gameObject.transform;
    }

    private void Update()
    {
        // NPC가 아직 스폰되지 않았고, 플레이어가 EndZone에 있을 때만 체크
        if (!isNpcSpawned && IsPlayerInEndZone())
        {
            timeInEndZone += Time.deltaTime;

            // 플레이어가 랜덤 시간 동안 머물면 NPC를 스폰
            if (timeInEndZone >= timeToSpawnNpc)
            {
                SpawnNpc();
                isNpcSpawned = true;  // NPC가 한 번만 스폰되도록 설정
            }
        }
        else
        {
            // 플레이어가 anoEnds 범위를 벗어나면 타이머를 초기화
            if (timeInEndZone > 0)
            {
                anoSfx.Stop();
            }
            timeInEndZone = 0f;
        }
    }

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        // anoStarts 배열 중 해당 콜라이더에 진입 시, 같은 인덱스의 anoLights 애니메이션 재생
        /*
        for (int i = 0; i < anoStarts.Length; i++)
        {
            if (anoTriggerZone == anoStarts[i])
            {
                Debug.Log($"anoStarts[{i}]에 플레이어가 진입했습니다. 애니메이션 재생을 시작합니다.");
            }
        }
        */
        // NPC 스폰 타이머가 활성화된 상태로 설정
        SetRandomTimeToSpawnNpc();  // 새로운 랜덤 시간을 설정
    }

    private void SpawnNpc()
    {
        if (anoNpc != null && player != null)
        {
            GameObject spawnedNpc = Instantiate(anoNpc,player,true);
            spawnedNpc.transform.localPosition = new Vector3(0f, -2f, 0.75f);
            spawnedNpc.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            
            
            // !! 점프스케어 재생 및 Damage !!
            NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(4, 0f);
            PlayerController.Instance.DecreaseMentality(20);

            // 1초 뒤에 NPC 삭제
            Destroy(spawnedNpc, 1f);
            isNpcSpawned = false;  // 새로운 NPC 스폰이 가능하도록 초기화
        }
    }

    // 플레이어가 anoEnds 중 하나에 있는지 확인하는 함수
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

    // NPC 스폰을 위한 랜덤 시간을 설정하는 메소드
    private void SetRandomTimeToSpawnNpc()
    {
        timeToSpawnNpc = Random.Range(1.5f, 3f);  // 2초에서 3초 사이의 랜덤 값 설정
    }
}
