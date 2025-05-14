using UnityEngine;

namespace NoManual.ANO
{
    public class ANO19_FakeExitSign : ANO_Component
    {

        [Header("밀치기 파워")] [SerializeField] private float pushPower = 0.5f;

        [Header("ano 관련 설정")] [SerializeField] private Collider anoStart;
        [SerializeField] private GameObject anoObject;
        [SerializeField] private AudioSource connectOffSfx;

        public override void ANO_TriggerCheck(Collider anoTriggerZone)
        {
            if (anoStart == anoTriggerZone)
            {
                connectOffSfx.Play();
                Rigidbody rigid = anoObject.GetComponent<Rigidbody>();
                if (rigid == null)
                {
                    rigid = anoObject.AddComponent<Rigidbody>();
                }

                // 수직으로 떨어지는 걸 방지하기 위한 밀치기 파워
                rigid.AddForce(Vector3.right * pushPower, ForceMode.VelocityChange);
                connectOffSfx.Play();
                anoStart.enabled = false;
            }
        }
    }
}
