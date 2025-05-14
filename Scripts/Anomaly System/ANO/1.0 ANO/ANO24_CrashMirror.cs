using System.Collections;
using UnityEngine;
using NoManual.ANO;

public class ANO24_CrashMirror : ANO_Component
{
    [Header("ANO ����")] 
    [SerializeField] private Collider anoStart; // Ʈ����
    [SerializeField] private GameObject[] anoObjs; // ����������
    [SerializeField] private AudioSource anoSfx; // ���� ����� AudioSource
    [SerializeField] private AudioClip[] dropSfx; // ��� ���� Ŭ��
    [SerializeField] private AudioClip[] treadSfx; // ������ �� ���� Ŭ��
    

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            anoStart.enabled = false;
            // MirrorDropCheck �޾��ְ�, Rigidbody �߰�, �� �߰�
            foreach (GameObject objs in anoObjs)
            {
                MirrorDropCheck dropCheck = objs.AddComponent<MirrorDropCheck>();
                Rigidbody rb = objs.AddComponent<Rigidbody>();
                
                // �ڲ� ���վ Extrapolate�� ����
                rb.interpolation = RigidbodyInterpolation.Extrapolate;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                
                float randomX = Random.Range(-0.25f, 0.25f);
                float randomY = Random.Range(-0.25f, 0.25f);
                float randomZ = Random.Range(-2.5f, -7f);


                rb.AddForce(new Vector3(randomX, randomY, randomZ), ForceMode.Impulse);

                // MirrorDropCheck�� dropSfx�� treadSfx �Ҵ�
                dropCheck.InitMirror(dropSfx, treadSfx);
               //   dropCheck.SetSfxClips(dropSfx, treadSfx);
               
               StartCoroutine(AddColliderAfterDelay(objs, 0.1f));
            }
            anoSfx.Play(); // ���� ���
        }
    }
    
    private IEnumerator AddColliderAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.AddComponent<BoxCollider>();
    }
}