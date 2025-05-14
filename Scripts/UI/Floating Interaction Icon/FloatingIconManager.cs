using System;
using System.Collections.Generic;
using NoManual.ANO;
using NoManual.Interaction;
using NoManual.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace NoManual.Managers
{
    public class FloatingIconManager : MonoBehaviour
    {
        [Header("Floating 아이콘 이미지 Root")] public Transform floatingIconPanel;
        [SerializeField]
        [Header("Floating 아이콘 이미지 프리팹")] private GameObject floatingIconPrefab;
        [SerializeField] 
        [Header("Floating 오브젝트 레이어")] private LayerMask floatingIconTargetLayerMask;
        [SerializeField] 
        [Header("크로스 헤어 기준")] private bool crossHairTarget;
        [SerializeField]
        [Header("아이콘 UI 인식 최소거리 (4 기본 값)")] private float iconHitDistance = 4f;
        [SerializeField]
        [Header("상호작용 UI 인식 최소거리 (1.25 기본 값)")] private float interactUiHitDistance = 1.25f;
        
        [Serializable]
        public struct FloatingIconImages
        {
            [Header("기본 상호작용 아이콘 이미지")]
            public Sprite defaultInteractionIcon;
            [Header("ANO 상호작용 아이콘 이미지")]
            public Sprite anoInteractionIon;
        }
        [Header("상호작용 아이콘 이미지")]
        public FloatingIconImages floatingIconImages = new FloatingIconImages();
        
        // 아이템 인지 범위
        private Vector3 _iconHitBoxSize = new Vector3(2f, 2f, 4f);
        // 크로스헤어 전용
        private bool _onEnableIcon = false;
        // 상호작용 UI 출력 확인
        public bool ShowInteractUi { get; set; } = false;

        private List<FloatingIcon> _floatingIconList = new List<FloatingIcon>();
        private GameObject _player;
        private Camera _playerMainCamera;
        // InteractManager에서 Ray Hit된 오브젝트
        public InteractionBase rayCastComponent { get; set; } = null;

        private void Awake()
        {
            _player = HFPS.Systems.HFPS_GameManager.Instance.m_PlayerObj;
            _playerMainCamera = HFPS.Systems.ScriptManager.Instance.MainCamera;

            if (crossHairTarget)
            {
                FloatingIcon icon = Instantiate(floatingIconPrefab, Vector3.zero, Quaternion.identity, floatingIconPanel).GetComponent<FloatingIcon>();
                icon.Initialize(_player.transform, iconHitDistance, interactUiHitDistance);
                _floatingIconList.Add(icon);
                DestroyIconList(null);
            }
        }

        private void Update()
        {
            if (crossHairTarget)
                CrossHairShowFloatingIcon();
            else
                PlayerNearFloatingIcon();
        }     

        /// <summary>
        /// 크로스 헤어 기준 Icon
        /// </summary>
        private void CrossHairShowFloatingIcon()
        {
            // CrossHairShowFloatingIconVer1();
            
            CrossHairShowFloatingIconVer2();
        }

        /// <summary>
        /// 크로스헤어 기준 Ver1 [사용 X]
        /// </summary>
        private void CrossHairShowFloatingIconVer1()
        {
            Vector3 rayOrigin = _playerMainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
            Vector3 rayDir = _playerMainCamera.transform.forward;
            Ray ray = new Ray(rayOrigin, rayDir);
            
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
            
            #region 버전 1
        
            if (Physics.Raycast(ray, out RaycastHit hit, iconHitDistance,floatingIconTargetLayerMask))
            {
                Sprite iconSprite = floatingIconImages.defaultInteractionIcon;

              //  Vector3 screenPos = _playerMainCamera.WorldToScreenPoint(hit.transform.position);
                _floatingIconList[0].SetTargetIconInfo(iconSprite, hit.transform.gameObject, hit.transform.position, _playerMainCamera);
                _floatingIconList[0].IsShow = _onEnableIcon = true;
                _floatingIconList[0].gameObject.SetActive(_onEnableIcon);
            }
            else if (_floatingIconList[0].IsShow)
            {
                DestroyIconList(_floatingIconList[0]);
            }

            #endregion
        }
        
        /// <summary>
        /// 크로스헤어 기준 Ver2
        /// </summary>
        private void CrossHairShowFloatingIconVer2()
        {
            Vector3 rayOrigin = _playerMainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
            Vector3 rayDir = _playerMainCamera.transform.forward;
            Ray ray = new Ray(rayOrigin, rayDir);
            
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
            
            #region 버전2
        
            if (!_onEnableIcon)
            {
                if (Physics.Raycast(ray, out RaycastHit hit, iconHitDistance,floatingIconTargetLayerMask))
                {
                    // Vector3 screenPos = _playerMainCamera.WorldToScreenPoint(hit.transform.position);
                   
                    GameObject rayObject = hit.collider.gameObject;
                   //  Vector3 iconPosition = hit.transform.position;
                    Sprite iconSprite = floatingIconImages.defaultInteractionIcon;
             
                    InteractionBase interactionComponent = rayObject.GetComponentInParent<InteractionBase>();
                    // Floating Icon 보여줄 위치 값 가져오기
                    Vector3 iconPosition = interactionComponent.GetAnotherInteractionObject().transform.position;
                  //  Debug.Log("Floating Icon Manager : " + hit.transform.gameObject.name);

                    // Floating Icon 스프라이트 할당 그리고 표시할 Icon 좌표 할당
                    if (interactionComponent is ItemComponent)
                    {
                        iconSprite = floatingIconImages.defaultInteractionIcon;
                    }
                    else if (interactionComponent is ANO_Component anoComponent)
                    {
                        // ANO 오브젝트가 이미 조치가 완료된 경우 생략
                        if (anoComponent.Get_ANO_ClearStatus())
                        {
                            // Floating Icon 비활성화
                            DestroyIconList(null);
                            return;
                        }
                        iconSprite = floatingIconImages.anoInteractionIon; // [24.03.27 데모 ANO 상호작용 이미지  defaultIcon]
                        // ANO_Component는 Floating Icon 위치를 마우스 커서 기준으로 설정
                        iconPosition = hit.point;
                    }
                    else if (interactionComponent is ReportingDeskTopPC_Component pcComponent)
                    {
                        // 보고 시스템이 이미 작동중인 경우 생략
                        if (pcComponent.IsReporting)
                        {
                            // Floating Icon 비활성화
                            DestroyIconList(null);
                            return;
                        }
                        iconSprite = floatingIconImages.defaultInteractionIcon;
                    }
                    else if (interactionComponent is DoorComponent doorComponent)
                    {
                        // 문 이미 작동중인 경우 생략
                        if (doorComponent.IsBusy)
                        {
                            // Floating Icon 비활성화
                            DestroyIconList(null);
                            return;
                        }
                        iconSprite = floatingIconImages.defaultInteractionIcon;
                    }

                    _floatingIconList[0].SetTargetIconInfo(iconSprite, hit.transform.gameObject, iconPosition, _playerMainCamera);
                    _floatingIconList[0].IsShow = _onEnableIcon = true;
                    _floatingIconList[0].gameObject.SetActive(_onEnableIcon);
                }
            }
            else
            {
                /*
                if (!IsVisibleFrustum() || !IsObjectVisibleByCamera() )
                {
                    DestroyIconList(null);
                }
                */
                
                if (_floatingIconList[0].IsShow)
                    _onEnableIcon = false;
            }
            
            #endregion
        }
        
        /// <summary>
        /// 플레이어 주변 기준 Icon [사용 X] (개선 필요 24.03.12)
        /// </summary>
        private void PlayerNearFloatingIcon()
        {
            var collider = Physics.OverlapBox(_playerMainCamera.transform.position, 
                                                    _iconHitBoxSize * 0.5f, 
                                                            _playerMainCamera.transform.rotation,
                                                            floatingIconTargetLayerMask);

            foreach (var targetObject in collider)
            {
                bool isIconShow = false;
                if (Physics.Linecast(_playerMainCamera.transform.position, targetObject.transform.position, out RaycastHit hit, floatingIconTargetLayerMask))
                {
                    for (int i = 0; i < _floatingIconList.Count; i++)
                    {
                        if ( _floatingIconList[i].InteractionObject == targetObject.transform)
                        {
                            isIconShow = true;
                            break;
                        }
                    }

                    if (!isIconShow)
                    {
                        Sprite iconSprite = floatingIconImages.defaultInteractionIcon;
                        FloatingIcon icon = Instantiate(floatingIconPrefab, Vector3.zero, Quaternion.identity, floatingIconPanel).GetComponent<FloatingIcon>();
                        icon.Initialize(_player.transform, iconHitDistance, interactUiHitDistance);
                        icon.SetTargetIconInfo(iconSprite, hit.transform.gameObject, hit.transform.position, _playerMainCamera);
                        icon.IsShow = _onEnableIcon = true;
                        icon.gameObject.SetActive(true);
                        _floatingIconList.Add(icon);
                    }
                }
            }
        }

        
        /// <summary>
        /// 상호작용 오브젝트 가림막 확인
        /// </summary>
        private bool IsObjectVisibleByCamera()
        {
            if (Physics.Linecast(_playerMainCamera.transform.position, _floatingIconList[0].InteractionObject.transform.position, out RaycastHit hit, floatingIconTargetLayerMask))
            {
                Debug.Log("상호작용 아이템이 특정 물체에 가리지 않고 있음");
                return true;
            }
            Debug.Log("상호작용 아이템이 특정 물체에 가리고 있음");
            return false;
        }
        
        /// <summary>
        /// 카메라 시점에 상호작용 오브젝트를 확인
        /// </summary>
        public bool IsVisibleFrustum()
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_playerMainCamera);
            //콜라이더 bounds 범위기준으로 확인
            return GeometryUtility.TestPlanesAABB(planes, _floatingIconList[0].InteractionObject.GetComponent<Collider>().bounds);
            
            /*  //평면과 점 사이의 최단거리로 범위기준 확인
            var point = _floatingIconList[0].InteractionObject.transform.position;

            foreach (var plane in planes)
            {
                if (plane.GetDistanceToPoint(point) < 0)
                {
                    return false;
                }
            }
            return true;
            */
        }
        
        /// <summary>
        /// Icon 오브젝트 삭제
        /// </summary>
        public void DestroyIconList(FloatingIcon floatingIcon)
        {
            if (crossHairTarget)
            {
                _floatingIconList[0].gameObject.SetActive(false);
                _floatingIconList[0].HideInteractUI();
                _floatingIconList[0].IsShow = _onEnableIcon = false;
            }
            else
            {
                for (int i = 0; i < _floatingIconList.Count; i++)
                {
                    if (_floatingIconList[i] == floatingIcon)
                    {
                        _floatingIconList.RemoveAt(i);
                        Destroy(floatingIcon.gameObject);
                        _onEnableIcon = false;
                    }
                }
            }
            rayCastComponent = null;
        }
        

        /// <summary>
        /// 상호작용 UI 활성화
        /// </summary>
        public void ShowInteractUI( GameObject targetObject,
                                    string title = null, 
                                    InteractionType interactionType = InteractionType.None)
                                    /*
                                    bool isShowInteractControl1 = false,
                                    TextTip controlType1 = TextTip.None,
                                    bool isShowInteractControl2 = false,
                                    TextTip controlType2 = TextTip.None)
                                    */
        {
            foreach (var icon in _floatingIconList)
            {
              //  Debug.Log("[ShowInteractUI] Floating Icon  : " + icon.InteractionObject.gameObject.name +" = "+ targetObject.name + " : Interact Manager");
                if (icon.InteractionObject.gameObject == targetObject)
                {
                    icon.ShowInteractUI(title, interactionType); //, isShowInteractControl1, controlType1, isShowInteractControl2, controlType2);
                }
            }
        }
        
        
        /// <summary>
        /// 플레이어 아이템 Icon 인지 범위
        /// </summary>
        void OnDrawGizmos()  
        {
            if (_playerMainCamera)
            {
                Gizmos.matrix = _playerMainCamera.transform.localToWorldMatrix;  
                Gizmos.color = Color.yellow;  
                Gizmos.DrawWireCube(Vector3.zero, _iconHitBoxSize);  
            }
        }  
    }
}

