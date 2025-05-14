using UnityEngine;
using NoManual.ANO;

public class ANO20_HangingCorpse : ANO_Component
{
    [Header("ANO 설정")] 
    [SerializeField] private Collider anoStart;
    [SerializeField] private Animator anoAnim0;
    [SerializeField] private Animator anoAnim1;
    [SerializeField] private AudioSource anoSfx0;
    [SerializeField] private AudioSource anoSfx1;
    [SerializeField] private AudioClip[] anoClips;
    
    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            anoStart.enabled = false;
            SetRandomAnimationSpeed(); // 애니메이션 속도 변경
            StartCoroutine(PlayRandomClipAtRandomInterval()); // 랜덤 간격으로 클립 재생
        }
    }
    
    private void SetRandomAnimationSpeed()
    {
        float randomSpeed0;
        float randomSpeed1;
        
        do
        {
            randomSpeed0 = Random.Range(0.5f, 1.3f); 
            randomSpeed1 = Random.Range(0.5f, 1.3f);
        } while (Mathf.Abs(randomSpeed0 - randomSpeed1) >= 0.5f);

        // 애니메이터에 속도 적용
        anoAnim0.speed = randomSpeed0;
        anoAnim1.speed = randomSpeed1;
    }
    
    private System.Collections.IEnumerator PlayRandomClipAtRandomInterval()
    {
        while (true)
        {
            // 랜덤한 시간 간격(2초에서 5초 사이)을 설정
            float randomInterval = Random.Range(2f, 5f);
            yield return new WaitForSeconds(randomInterval);

            // anoClips 배열에서 랜덤하게 하나의 클립을 선택
            AudioClip randomClip = anoClips[Random.Range(0, anoClips.Length)];

            // anoSfx0과 anoSfx1에 클립을 재생
            anoSfx0.clip = randomClip;
            anoSfx0.Play();

            anoSfx1.clip = randomClip;
            anoSfx1.Play();
        }
    }
}