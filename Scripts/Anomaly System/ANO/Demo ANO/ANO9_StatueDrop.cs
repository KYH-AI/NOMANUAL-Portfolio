using UnityEngine;
using NoManual.ANO;

    public class ANO9_StatueDrop : ANO_Component
    {
        [Header("밀치기 파워")] [SerializeField] private float pushPower = 1.1f;

        public AudioSource statueFallStartSfx;
        public Collider anoStart;
        public GameObject anoObject;

        public override void ANO_TriggerCheck(Collider anoTriggerZone)
        {
            if (anoTriggerZone == anoStart)
            {
                Rigidbody rigid = anoObject.GetComponent<Rigidbody>();
                if (rigid == null)
                {
                    rigid = anoObject.AddComponent<Rigidbody>();
                }

                rigid.AddForce(Vector3.back * pushPower, ForceMode.Impulse);
                statueFallStartSfx.Play();
                anoStart.enabled = false;
            }
        }
    }
