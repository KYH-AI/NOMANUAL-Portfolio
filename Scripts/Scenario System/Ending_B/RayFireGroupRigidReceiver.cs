using UnityEngine;

public class RayFireGroupRigidReceiver : RayFireGroupReceiver
{
    [Header("======== Rigidbody ±×·ì ========")]
    [SerializeField] private Rigidbody[] groupRigid;

    public override void PlayRayFire()
    {
        base.PlayRayFire();
        for (int i = 0; i < groupRigid.Length; i++) groupRigid[i].useGravity = true;
    }
}
