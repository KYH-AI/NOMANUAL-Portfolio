using NoManual.Utils;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == (int)Layer.LayerIndex.PostProcessingArm)
        {
            HFPS.Player.PlayerController.Instance.mentality.DecreaseMentality(100);
        }
    }
}
