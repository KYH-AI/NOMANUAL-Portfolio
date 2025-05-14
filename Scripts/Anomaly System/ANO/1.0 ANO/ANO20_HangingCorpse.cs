using UnityEngine;
using NoManual.ANO;

public class ANO20_HangingCorpse : ANO_Component
{
    [Header("ANO ����")] 
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
            SetRandomAnimationSpeed(); // �ִϸ��̼� �ӵ� ����
            StartCoroutine(PlayRandomClipAtRandomInterval()); // ���� �������� Ŭ�� ���
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

        // �ִϸ����Ϳ� �ӵ� ����
        anoAnim0.speed = randomSpeed0;
        anoAnim1.speed = randomSpeed1;
    }
    
    private System.Collections.IEnumerator PlayRandomClipAtRandomInterval()
    {
        while (true)
        {
            // ������ �ð� ����(2�ʿ��� 5�� ����)�� ����
            float randomInterval = Random.Range(2f, 5f);
            yield return new WaitForSeconds(randomInterval);

            // anoClips �迭���� �����ϰ� �ϳ��� Ŭ���� ����
            AudioClip randomClip = anoClips[Random.Range(0, anoClips.Length)];

            // anoSfx0�� anoSfx1�� Ŭ���� ���
            anoSfx0.clip = randomClip;
            anoSfx0.Play();

            anoSfx1.clip = randomClip;
            anoSfx1.Play();
        }
    }
}