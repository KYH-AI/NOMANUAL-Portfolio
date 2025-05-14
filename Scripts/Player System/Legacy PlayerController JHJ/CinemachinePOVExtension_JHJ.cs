using Cinemachine;
using UnityEngine;

public class CinemachinePOVExtension : CinemachineExtension
{

    // 카메라 회전 속도 제한
    [SerializeField] private float horizontalSpeed = 10f;
    [SerializeField] private float verticalSpeed = 10f;
    // 카메라 각도 제한 
    [SerializeField] private float clmapAngle = 80f;
    
    private InputManager inputManager;
    private Vector3 startingRotation;
    
    protected override void Awake()
    {
        inputManager = InputManager.Instance;
        // base의 Awake 호출
        base.Awake();
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage,
        ref CameraState state, float deltaTime) {
        if (vcam.Follow)
        {
            // 1인칭 구현
            if (stage == CinemachineCore.Stage.Aim)
            {
                if (startingRotation == null) startingRotation = transform.localRotation.eulerAngles;
                Vector2 deltaInput = inputManager.GetMouseDelta();
                startingRotation.x += deltaInput.x * verticalSpeed * Time.deltaTime;
                startingRotation.y += deltaInput.y * horizontalSpeed * Time.deltaTime;
                startingRotation.y = Mathf.Clamp(startingRotation.y, - clmapAngle, clmapAngle);
                state.RawOrientation = Quaternion.Euler(-startingRotation.y, startingRotation.x, 0f);
            }
        }
    }
}
