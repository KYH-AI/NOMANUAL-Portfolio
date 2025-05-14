using NoManual.Managers;
using UnityEngine;

public class EndingNone_GamiginActive : MonoBehaviour
{
    [SerializeField] private Transform lookAtPos;
    [SerializeField] private Animator gamiginAnim;
    [SerializeField] private AudioSource gamiginSfx1;
    [SerializeField] private AudioSource gamiginSfx2;
    [SerializeField] private AudioSource gamiginSfx3;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HFPS.Systems.HFPS_GameManager.Instance.userInterface.Crosshair.enabled = false;
            Cursor.lockState = CursorLockMode.Locked; 
            Cursor.visible = false;
            PlayerAPI.SetFocusCameraTarget(lookAtPos);
            PlayerAPI.DisablePlayerKeyBoardInput();
        
            
            // 이속 Fade 느려지게 해서, 완전히 정지
            gamiginAnim.SetTrigger("Active");
            gamiginSfx1.Play();
            gamiginSfx2.Play(1);
            gamiginSfx3.Play(2);
            Destroy(gameObject);
        }
    }
}
