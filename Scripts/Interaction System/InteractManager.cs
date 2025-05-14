using System;
using NoManual.ANO;
using NoManual.Interaction;
using NoManual.Inventory;
using NoManual.UI;
using ThunderWire.Input;
using ThunderWire.Utility;
using UnityEngine;

namespace NoManual.Managers
{
    public class InteractManager : MonoBehaviour
    {
        [Header("상호작용 거리 (기본 값 1.5)")] 
        [SerializeField] private float raycastRange = 1.5f;
        [Header("상호작용 타겟 레이어마스크")]
        [SerializeField] private LayerMask rayCastTargetLayerMask;
       
        // 상호작용 가능한 여부 확인
        public bool CanInteract { get; set; } = true;
        private bool _interactionOption1Pressed = false; // E키 입력 확인
        private bool _interactionOption2Pressed = false; // R키 입력 확인
        private bool _isShowInteractionUi = false;
        
        private GameObject _rayCastObject;
        private GameObject _lastRayCastObject;
        private InteractionBase _rayCastObjectComponent;
        private Vector3 _interactANO_StickerPoint;
        private Vector3 _interactANO_StickerNormal;
        private Camera _playerCam;
        private EquipItemSwitcherManager _switcherManager;

        // 아이템 상호작용 Tag
        private const string _INTERACT_ITEM_TAG = "InteractItem";
        // ANO 상호작용 Tag
        private const string _INTERACT_ANO_TAG = "InteractANO";
        // Door 상호작용 Tag
        private const string _INTERACT_DOOR_TAG = "InteractDoor";
        
        private void Awake()
        {
            _playerCam = HFPS.Systems.ScriptManager.Instance.MainCamera;
           // _switcherManager = Sw
        }

        private void Update()
        {
            // 상호작용 불가능 상태 예외처리
            if(!CanInteract) return;

            CheckInputInteract();
            InteractObject();
            RayCast();
        }
        
        /// <summary>
        /// 상호작용 아이템 레이캐스트
        /// </summary>
        private void RayCast()
        {
            Ray playerAim = _playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            
            if (Physics.Raycast(playerAim, out RaycastHit hit, raycastRange, rayCastTargetLayerMask))
            {
                _rayCastObject = hit.collider.gameObject;

                // 기존에 확인한 오브젝트와 같으면 무시
                if (!_lastRayCastObject)
                {
                    if (_lastRayCastObject == _rayCastObject) return;
                }

                if (_rayCastObject)
                {
                    GameObject interactionTargetObject = null;//_rayCastObject;
                    string title = string.Empty;
                    /*
                    bool isShowInteractControl1 = false;
                    TextTip controlType1 = TextTip.None;
                    bool isShowInteractControl2 = false;
                    TextTip controlType2 = TextTip.None;
                    */
                    InteractionType interactionType = InteractionType.None;
                    
                    InteractionBase interactionComponent = _rayCastObject.GetComponentInParent<InteractionBase>();
                    interactionTargetObject = interactionComponent.GetAnotherInteractionObject(_rayCastObject); // 상호작용 가능한 오브젝트 정보얻기
                    NoManualHotelManager.Instance.FloatingIconManager.rayCastComponent =  _rayCastObjectComponent = interactionComponent;
                    
                    // 상호작용 오브젝트 RayCast
                    if (!interactionComponent)
                    {
                        _rayCastObject = null;
                        _rayCastObjectComponent = null;
                        return;
                    }
                    // RayCast 가능한지 상태확인
                    if (interactionComponent.InteractRayCast())
                    {
                        // ANO 컴포넌트 경우는 RayCast 정보 전달필요
                        if (interactionComponent is ANO_Component anoComponent)
                        {
                            anoComponent.Set_ANO_StickerPoint(hit.point, hit.normal);
                            Debug.DrawLine(hit.point,   hit.point + hit.normal * 5f, Color.blue);
                        }
                        
                        title = interactionComponent.title;
                        interactionType = interactionComponent.interactionType;
                        /*
                        controlType1 = interactionComponent.controlType1;
                        controlType2 = interactionComponent.controlType2;
                        isShowInteractControl1 = controlType1 != TextTip.None;
                        isShowInteractControl2 = controlType2 != TextTip.None;
                        */
                    }

                    NoManualHotelManager.Instance.FloatingIconManager.ShowInteractUI(interactionTargetObject, title, interactionType); //(interactionTargetObject, title, isShowInteractControl1, controlType1, isShowInteractControl2, controlType2);
                    NoManualHotelManager.Instance.FloatingIconManager.ShowInteractUi = _isShowInteractionUi = true;
                    _lastRayCastObject = _rayCastObject;
                }
            }
            else
            {
                // 상호작용 오브젝트가 발견되지 않고 상호작용 UI가 활성화된 경우 
                // RayCast 오브젝트 정보 비활성화
                if (_isShowInteractionUi)
                {
                    NoManualHotelManager.Instance.FloatingIconManager.ShowInteractUi = _isShowInteractionUi = false;
                    NoManualHotelManager.Instance.FloatingIconManager.rayCastComponent = _rayCastObjectComponent = null;
                    _lastRayCastObject = _rayCastObject = null;
                }
            }
        }

        #region 상호작용

        /// <summary>
        /// 상호작용 
        /// </summary>
        private void InteractObject()
        {
            if (!_interactionOption1Pressed || !_rayCastObject || !_rayCastObjectComponent) return;
            
            // 컴포넌트 상호작용
            _rayCastObjectComponent.Interact();
            RestRayCastInfo();
        }

        /// <summary>
        /// 상호작용 E키 입력 확인
        /// </summary>
        private void CheckInputInteract()
        {
            _interactionOption1Pressed = InputHandler.ReadButtonOnce(this, "Use");
            _interactionOption2Pressed = InputHandler.ReadButtonOnce(this, "Reload");
        }

        /// <summary>
        /// 기록된 RayCast 오브젝트 정보 삭제
        /// </summary>
        private void RestRayCastInfo()
        {
            _lastRayCastObject = _rayCastObject = null;
            _rayCastObjectComponent = null;
            NoManualHotelManager.Instance.FloatingIconManager.DestroyIconList(null);
        }

        
        /// <summary>
        /// 플레이어 상호작용 범위
        /// </summary>
        void OnDrawGizmos()  
        {
            if (_playerCam)
            {
                Gizmos.color = Color.green;  
                Gizmos.DrawRay (_playerCam.transform.position, _playerCam.transform.forward * raycastRange);
            }
        }
        
        #endregion
    }
}


