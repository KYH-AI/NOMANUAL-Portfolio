using System;
using System.Collections.Generic;
using NoManual.ANO;
using NoManual.Interaction;
using UnityEngine;
using UnityEngine.UI;
using NoManual.Managers;
using TMPro;

namespace NoManual.UI
{
    public class FloatingIcon : MonoBehaviour
    {
        public bool IsShow { get; set; } = false;
        // Floating Icon 매니저에서 Ray Hit된 상호작용 오브젝트
        public GameObject InteractionObject { get;  private set; }
        // Icon 보여줄 좌표
        private Vector3 _hitPoint;

        [Header("InteractUI Root")]
        [SerializeField] private Transform interactUiPanel;

        [Serializable]
        public struct InteractUI
        {
            [Tooltip("상호작용 이미지")] public Image icon;
            [Tooltip("아이템 이름 텍스트")] public TextMeshProUGUI interactInfoText;
            [Tooltip("아이템 상호작용 Text1")] public TextMeshProUGUI interactControl1Text;
            [Tooltip("아이템 상호작용 Text2")] public TextMeshProUGUI interactControl2Text;
            [Tooltip("아이템 컨트롤 박스1")] public GameObject interactControl1;
            [Tooltip("아이템 컨트롤 박스2")] public GameObject interactControl2;
        }

        [Header("아이템 상호작용 UI")] 
        public InteractUI interactUI = new InteractUI();
        
        private float _iconRange;
        private float _interactUiRange;
        private Transform _playerTransform;
        private Camera _playerCam;
        // 상호작용 Text UI Fade 전용
        private CanvasGroup _canvasGroup;
        // 상호작용 Text 로컬라이징
        private Dictionary<InteractionType, string> _interactControlTextLocalization = new Dictionary<InteractionType, string>();

        
        public void Initialize(Transform target, float iconMinDistance, float interactUiRange)
        {
            this._playerTransform = target;
            this._iconRange = iconMinDistance;
            this._interactUiRange = interactUiRange;
            _canvasGroup = GetComponentInChildren<CanvasGroup>();
            _canvasGroup.alpha = 0f;
            _interactControlTextLocalization.Add(InteractionType.None, string.Empty);
            _interactControlTextLocalization.Add(InteractionType.Item, 
                GameManager.Instance.localizationTextManager.GetText(
                    LocalizationTable.TextTable.Interaction_Table,
                    LocalizationTable.InteractionTableTextKey.UI_Floating_TakeText));
            _interactControlTextLocalization.Add(InteractionType.ANO, 
                GameManager.Instance.localizationTextManager.GetText(
                    LocalizationTable.TextTable.Interaction_Table, 
                    LocalizationTable.InteractionTableTextKey.UI_Floating_StickerText));
            _interactControlTextLocalization.Add(InteractionType.Read, 
                GameManager.Instance.localizationTextManager.GetText(
                    LocalizationTable.TextTable.Interaction_Table, 
                    LocalizationTable.InteractionTableTextKey.UI_Floating_ReadText));
            _interactControlTextLocalization.Add(InteractionType.Door, 
                GameManager.Instance.localizationTextManager.GetText(
                    LocalizationTable.TextTable.Interaction_Table, 
                    LocalizationTable.InteractionTableTextKey.UI_Floating_DoorText));
            _interactControlTextLocalization.Add(InteractionType.PC, 
                GameManager.Instance.localizationTextManager.GetText(
                    LocalizationTable.TextTable.Interaction_Table, 
                    LocalizationTable.InteractionTableTextKey.UI_Floating_UseText));
            _interactControlTextLocalization.Add(InteractionType.Press, 
                GameManager.Instance.localizationTextManager.GetText(
                    LocalizationTable.TextTable.Interaction_Table, 
                    LocalizationTable.InteractionTableTextKey.UI_Floating_PressText));
            _interactControlTextLocalization.Add(InteractionType.Put, 
                GameManager.Instance.localizationTextManager.GetText(
                    LocalizationTable.TextTable.Interaction_Table, 
                    LocalizationTable.InteractionTableTextKey.UI_Floating_PutText));
            
            
            // 텍스트 정렬 (초기화)
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)interactUiPanel.GetComponent<VerticalLayoutGroup>().transform);
        }

        /// <summary>
        /// 상호작용 오브젝트 할당 (hitPoint 기준으로 Icon 좌표 활성화)
        /// </summary>
        public void SetTargetIconInfo(Sprite icon, GameObject interactionObject, Vector3 hitPoint, Camera playerCam)
        {
            this.interactUI.icon.sprite = icon;
            this.InteractionObject = interactionObject;
            this._hitPoint = hitPoint;
            this._playerCam = playerCam;
        }
        
        
        private void Update()
        {
            if (IsShow)
            {
                if (!InteractionObject)
                {
                    IsShow = false;
                    NoManualHotelManager.Instance.FloatingIconManager.DestroyIconList(this);
                    return;
                }
                
                float distFromPlayer = Vector3.Distance(_hitPoint, _playerTransform.position);
                if (distFromPlayer > _iconRange)
                {
                    IsShow = false;
                    NoManualHotelManager.Instance.FloatingIconManager.DestroyIconList(this);
                    return;
                }

                // 카메라에 상호작용 오브젝트 랜더링 안되는 경우
                if (!NoManualHotelManager.Instance.FloatingIconManager.IsVisibleFrustum())
                {
                   // Debug.Log(" IsVisibleFrustum : false ");
                   // 알파값 0로 설정
                    InteractUiTextAlphaController();
                    IconAlphaController();
                }
                else
                {
                    IconAlphaController(distFromPlayer);
                }
                
                // Icon 위치
                IconTransformController();
      

                if (NoManualHotelManager.Instance.FloatingIconManager.ShowInteractUi)
                {
                    if(ShowInteractUiText())
                    {
                        InteractUiTextAlphaController(distFromPlayer);
                    }
                }
                else
                {
                    InteractUiTextAlphaController();
                }
            }
        }

        /// <summary>
        /// 상호작용된 Ray 오브젝트 컴포넌트 null 값 확인
        /// </summary>
        private bool ShowInteractUiText()
        {
            if (NoManualHotelManager.Instance.FloatingIconManager.rayCastComponent)
            {
                InteractionBase interactionBase = NoManualHotelManager.Instance.FloatingIconManager.rayCastComponent;
                if (interactionBase.GetAnotherInteractionObject(InteractionObject))
                {
                    return true;
                }
                
                /*
                if (interactionBase is ANO_Component anoComponent)
                {
                    if (anoComponent.GetStickerTargetObject(InteractionObject.gameObject))
                        return true;
                }
                else if (interactionBase is DoorComponent doorComponent)
                {
                    if (doorComponent.doorHandleObject.gameObject == InteractionObject.gameObject)
                        return true;
                }
                else if(interactionBase.gameObject == InteractionObject.gameObject)
                {
                    return true;
                }
                */
            }
            return false;
        }

        private void IconTransformController()
        {
            Vector3 screenPos = _playerCam.WorldToScreenPoint(_hitPoint);//(InteractionObject.position);
            transform.position = screenPos;
        }

        private void IconAlphaController(float distFromPlayer)
        {
            float alpha = Mathf.Lerp(2.0f, 0f, Mathf.InverseLerp(0.0f, 2.5f, distFromPlayer));
            Color color = interactUI.icon.color;
            color.a = alpha;
            interactUI.icon.color = color;
        }

             
        /// <summary>
        /// 아이콘 이미지 UI 알파 값 0
        /// </summary>
        private void IconAlphaController()
        {
            Color color = interactUI.icon.color;
            color.a = 0f;
            interactUI.icon.color = color;
        }

        /// <summary>
        /// 상호작용 UI 알파 값 조절 (플레이어 위치 기준으로)
        /// </summary>
        private void InteractUiTextAlphaController(float distFromPlayer)
        {
            // 5.5f -> 알파 값 강도 (높을 수록 알파 값이 더 빠르게 올라감)
            // _interactUiRange -> 플레이어 인지 범위 (낮을 수록 플레이어가 아이템에 더 가까이 접근해야함)
            float alpha = Mathf.Lerp(5.5f, 0f, Mathf.InverseLerp(0.0f, _interactUiRange, distFromPlayer));
            _canvasGroup.alpha = alpha;
        }
        
        /// <summary>
        /// 상호작용 UI 알파 값 0
        /// </summary>
        private void InteractUiTextAlphaController()
        {
            _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, 0f, Time.deltaTime * 5f);
        }
        
        
        /// <summary>
        /// 상호작용 UI 활성화
        /// </summary>
        public void ShowInteractUI(string title = null, InteractionType interactionType = InteractionType.None)
            /*
                                    bool isShowInteractControl1 = false,
                                    TextTip controlType1 = TextTip.None,
                                    bool isShowInteractControl2 = false,
                                    TextTip controlType2 = TextTip.None)*/
        {
            if (interactionType == InteractionType.None)
            {
                HideInteractUI();
                return;
            }
            
            interactUI.interactInfoText.text = title;
            interactUI.interactControl1Text.text = _interactControlTextLocalization[interactionType];
            
            interactUI.interactInfoText.gameObject.SetActive(title != string.Empty);
            interactUI.interactControl1.SetActive(_interactControlTextLocalization[interactionType] != string.Empty);
            interactUiPanel.gameObject.SetActive(true);

            /* (241022 로컬라이징 X버젼 Legacy)
            if (!isShowInteractControl1 && !isShowInteractControl2)
            {
                HideInteractUI();
                return;
            }

            TextType textType = TextType.Interaction;
            interactUI.interactInfoText.text = title;

            
            if (isShowInteractControl1)
            {
                interactUI.interactControl1Text.text = _interactControlTextLocalization[controlType1]//NoManualHotelManager.Instance.TextDataBase.GetTextDataValue(textType, controlType1);
            }

            if (isShowInteractControl2)
            {
                interactUI.interactControl2Text.text = NoManualHotelManager.Instance.TextDataBase.GetTextDataValue(textType, controlType2);
            }
            interactUI.interactInfoText.gameObject.SetActive(title != string.Empty);
            interactUI.interactControl1.SetActive(isShowInteractControl1);
            interactUI.interactControl2.SetActive(isShowInteractControl2);
            interactUiPanel.gameObject.SetActive(true);
            */
        }

        /// <summary>
        /// 상호작용 및 아이콘 UI 비활성화
        /// </summary>
        public void HideInteractUI()
        {
            interactUI.interactInfoText.gameObject.SetActive(false);
            interactUI.interactControl1.SetActive(false);
            interactUI.interactControl2.SetActive(false);
            interactUiPanel.gameObject.SetActive(false);

            IconAlphaController();
            
            interactUI.interactInfoText.text = string.Empty;
            interactUI.interactControl1Text.text = string.Empty;
            interactUI.interactControl2Text.text = string.Empty;

        }
        
    }
}


