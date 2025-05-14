using System.Collections;
using UnityEngine;
using NoManual.ANO;

public class ANO24_CrashMirror : ANO_Component
{
    [Header("ANO 설정")] 
    [SerializeField] private Collider anoStart; // 트리거
    [SerializeField] private GameObject[] anoObjs; // 유리조각들
    [SerializeField] private AudioSource anoSfx; // 사운드 재생용 AudioSource
    [SerializeField] private AudioClip[] dropSfx; // 드랍 사운드 클립
    [SerializeField] private AudioClip[] treadSfx; // 밟혔을 때 사운드 클립
    

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            anoStart.enabled = false;
            // MirrorDropCheck 달아주고, Rigidbody 추가, 힘 추가
            foreach (GameObject objs in anoObjs)
            {
                MirrorDropCheck dropCheck = objs.AddComponent<MirrorDropCheck>();
                Rigidbody rb = objs.AddComponent<Rigidbody>();
                
                // 자꾸 벽뚫어서 Extrapolate로 변경
                rb.interpolation = RigidbodyInterpolation.Extrapolate;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                
                float randomX = Random.Range(-0.25f, 0.25f);
                float randomY = Random.Range(-0.25f, 0.25f);
                float randomZ = Random.Range(-2.5f, -7f);


                rb.AddForce(new Vector3(randomX, randomY, randomZ), ForceMode.Impulse);

                // MirrorDropCheck에 dropSfx와 treadSfx 할당
                dropCheck.InitMirror(dropSfx, treadSfx);
               //   dropCheck.SetSfxClips(dropSfx, treadSfx);
               
               StartCoroutine(AddColliderAfterDelay(objs, 0.1f));
            }
            anoSfx.Play(); // 사운드 재생
        }
    }
    
    private IEnumerator AddColliderAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.AddComponent<BoxCollider>();
    }
}