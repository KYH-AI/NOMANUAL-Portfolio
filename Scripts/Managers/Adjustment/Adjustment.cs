using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NoManual.Inventory;
using NoManual.StateMachine;
using NoManual.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace NoManual.Managers
{
    
    public enum MedicalServiceEnum
    {
        Adjustment_MedicalTip1,
        Adjustment_MedicalTip2,
        Adjustment_MedicalTip3,
    }
    
    public class Adjustment : MonoBehaviour
    {
        [Header("=== 디버깅 ===")] 
        [SerializeField] private bool debugMode = false;
        [SerializeField] private int testUserCash;
        [SerializeField] private int testClearAnoCount;
        [Range(0f, 1f)] [SerializeField] private float testMentality;
        [SerializeField] private RS_TradersDeliveryMapper[] testDeliveryMappers;

        [Space(10)] 
        [Header("=== 참조 ===")]
        [SerializeField] private ItemDataBaseScriptable itemDB;
        [SerializeField] private Signature signature;
        [SerializeField] private Animation receiptAnimation;
        [SerializeField] private CanvasGroup mainCanvasGroup;
        [SerializeField] private CanvasGroup noMedicalCanvasGroup;
        [SerializeField] private CanvasGroup defaultMedicalCanvasGroup;
        [SerializeField] private CanvasGroup medicineCanvasGroup;
        [SerializeField] private CanvasGroup signatureCanvasGroup;
        
        [Space(10)]
        [Header("=== UI ===")] 
        [Space(5)]
        [Header("동적 텍스트")]
        [SerializeField] private TextMeshProUGUI codeNumberText;
        [SerializeField] private TextMeshProUGUI userTempCashText;
        [SerializeField] private TextMeshProUGUI bonusRsCashText;
        [SerializeField] private TextMeshProUGUI rsTradersCashText;
        [SerializeField] private TextMeshProUGUI medicineCashText;
        [SerializeField] private TextMeshProUGUI userResultRsCashText;

        [Space(5)] 
        [Header("정적 텍스트")]
        [SerializeField] private TextMeshProUGUI stdCashText;
        [SerializeField] private TextMeshProUGUI noMedicalCashText;
        [SerializeField] private TextMeshProUGUI defaultMedicalCashText;

        [Space(5)] 
        [Header("싸인 패널")] 
        [SerializeField] private GameObject signaturePanel;

        [Space(10)] 
        [Header("토글")]
        [SerializeField] private Toggle noMedicalToggle;
        [SerializeField] private Toggle defaultMedicalToggle;
        [SerializeField] private Toggle medicineToggle;

        [Space(10)] 
        [Header("버튼")]
        [SerializeField] private Button nextDayButton;
        
        [Space(10)]
        [Header("오디오")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip receiptIntroClip;
        [SerializeField] private AudioClip receiptEndingClip;
        [SerializeField] private AudioClip[] coinClip;

        [Space(10)]
        [Header("툴 팁")]
        [SerializeField] private RectTransform medicalServiceTipObject;
        [SerializeField] private TextMeshProUGUI medicalServiceTipText;
        [SerializeField] private TextMeshProUGUI medicalServiceTipMentalityText;
        private Vector2 offset = new Vector2(200f, -5f); // 우측으로 약간의 간격
        private Camera uiCam;
        private bool showToolTip = false;

        private readonly int _FIXED_ALLOWANCE = 100;
        private readonly int _BONUS_ALLOWANCE = 10;
        private readonly int _MEDICAL_SERVICE_COST = 30;
        private readonly int _MEDICINE_ITEM_ID = 4028;
        private readonly float _TEXT_ALPHA_VALUE = 0.75f;
        private readonly float _TEXT_SCRAMBLE_DURACTION = 1.25f;
        private readonly float _TEXT_FADE_DURACTION = 1f;
        private readonly float _TEXT_SCALE_DURACTION = 0.075f;
        private readonly int _TEXT_YOYO_COUNT = 2;
        
        private int _medicineCost;
        private RS_TradersDeliveryMapper[] saveDeliveryMappers;
        private PlayerSaveData _playerSaveData;
        private int _clearAnoCount = 0;
        
        private int _userTempCash = 0;
        private int _bonusCash = 0;
        private int _rsTradersCash = 0;
        private bool _medicalServiceMode = false;
        private int _originalMentality = 0;
        private int _tempMentality = 0;

        private void Awake()
        {
            uiCam = Camera.main;

            mainCanvasGroup.blocksRaycasts = false;
            nextDayButton.interactable = false;
            
            _medicineCost = itemDB.GetItemDataToItemId(_MEDICINE_ITEM_ID).itemSettings.itemPrice;

            AllRemoveToggleEvent();
            nextDayButton.onClick.AddListener(NextDayButtonClickEvent);
            noMedicalToggle.isOn = true;
            noMedicalToggle.onValueChanged.AddListener(delegate { MedicalServiceToggleEvent(false); });
            defaultMedicalToggle.onValueChanged.AddListener(delegate { MedicalServiceToggleEvent(true); });
            medicineToggle.onValueChanged.AddListener(delegate {MedicineServiceToggleEvent();});
        
            medicalServiceTipObject.gameObject.SetActive(false);

            // 유저 정산 전 자금 초기화
#if UNITY_EDITOR
            if (debugMode)
            {
                saveDeliveryMappers = testDeliveryMappers;
                _clearAnoCount = testClearAnoCount;
                _userTempCash = testUserCash;
                _tempMentality = _originalMentality = (int)(testMentality * 100);
            }
            else
            {
                _playerSaveData = GameManager.Instance.SaveGameManager.CurrentPlayerSaveData;
                if (_playerSaveData == null)
                {
                    ErrorCode.SendError(ErrorCode.ErrorCodeEnum.GetSaveFile);
                    return;
                }
                saveDeliveryMappers = _playerSaveData.DeliveryItem;
                _clearAnoCount = _playerSaveData.ClearAnoCount;
                _userTempCash = _playerSaveData.RS_Cash;
                _tempMentality = _originalMentality = (int)(_playerSaveData.Mentatilty * 100);
            }
#else
            debugMode = false;
            _playerSaveData = GameManager.Instance.SaveGameManager.CurrentPlayerSaveData;
            if (_playerSaveData == null)
            {
                ErrorCode.SendError(ErrorCode.ErrorCodeEnum.GetSaveFile);
                return;
            }
                saveDeliveryMappers = _playerSaveData.DeliveryItem;
                _clearAnoCount = _playerSaveData.ClearAnoCount;
                _userTempCash = _playerSaveData.RS_Cash;
                _tempMentality = _originalMentality = (int)(_playerSaveData.Mentatilty * 100);
#endif
            
            _userTempCash += GetRsTradersPayCash(saveDeliveryMappers);
            _bonusCash = _clearAnoCount * _BONUS_ALLOWANCE;


            // PlaceHolder 텍스트 변경
            DateTime now = DateTime.Now;
            string yyyymmddhhmm = now.ToString("yyyyMMddHHmm");
            codeNumberText.text = GetPlaceHolderText(codeNumberText.text, yyyymmddhhmm);
            userTempCashText.text = GetPlaceHolderText(userTempCashText.text, _userTempCash.ToString());
            bonusRsCashText.text = GetPlaceHolderText(bonusRsCashText.text, _bonusCash.ToString());
            rsTradersCashText.text = GetPlaceHolderText(rsTradersCashText.text, _rsTradersCash.ToString());
            medicineCashText.text = GetPlaceHolderText(medicineCashText.text, _medicineCost.ToString());

            _userTempCash = GetTempResultUserCash();
            userResultRsCashText.text = GetPlaceHolderText(userResultRsCashText.text, _userTempCash.ToString());
            ControlMedicineToggleButton();

            SetCanvasGroupAlpha(noMedicalCanvasGroup, 0f);
            SetCanvasGroupAlpha(defaultMedicalCanvasGroup, 0f);
            SetCanvasGroupAlpha(medicineCanvasGroup, 0f);
            SetCanvasGroupAlpha(signatureCanvasGroup, 0f);
            nextDayButton.GetComponent<CanvasGroup>().DOFade(0f, 0f);

            codeNumberText.DOFade(0f, 0f);
            userTempCashText.DOFade(0f, 0f);
            stdCashText.DOFade(0f, 0f);
            bonusRsCashText.DOFade(0f, 0f);
            rsTradersCashText.DOFade(0f, 0f);
            userResultRsCashText.DOFade(0f, 0f);
            
            audioSource.PlayOneShot(receiptIntroClip);
        }
        
        public void CompleteAnimation()
        {
            StartDOTweenTextEffect(codeNumberText).OnComplete
            (() => StartDOTweenTextEffect(userTempCashText).OnComplete
            (() => StartDOTweenTextEffect(stdCashText).OnComplete
            (() => StartDOTweenTextEffect(bonusRsCashText).OnComplete
            (() => StartDOTweenTextEffect(rsTradersCashText).OnComplete
            (() => StartDOTweenCanvasGroupEffect(noMedicalCanvasGroup).OnComplete
            (() => StartDOTweenCanvasGroupEffect(defaultMedicalCanvasGroup).OnComplete
            (() => StartDOTweenCanvasGroupEffect(medicineCanvasGroup).OnComplete
            (() => StartDOTweenTextEffect(userResultRsCashText).OnComplete
            (() => StartDOTweenCanvasGroupEffect(signatureCanvasGroup).OnComplete
            (() =>
            {
                mainCanvasGroup.blocksRaycasts = true;
                signature.InitSignature(CompleteSignEvent);
            }))))))))));
        }

        private Tween StartDOTweenTextEffect(TextMeshProUGUI textMesh)
        {
            audioSource.PlayOneShot(coinClip[UnityEngine.Random.Range(0, coinClip.Length)]);
            return DOTween.Sequence()
                .Append(textMesh.DOText(textMesh.text, _TEXT_SCRAMBLE_DURACTION, true, ScrambleMode.All))
                .Join(textMesh.DOFade(1f, _TEXT_FADE_DURACTION))
                .Join(textMesh.DOScale(1.5f, _TEXT_SCALE_DURACTION).SetEase(Ease.OutQuart).SetLoops(_TEXT_YOYO_COUNT, LoopType.Yoyo));
        }

        private Tween StartDOTweenCanvasGroupEffect(CanvasGroup cg)
        {
            audioSource.PlayOneShot(coinClip[UnityEngine.Random.Range(0, coinClip.Length)]);
            return DOTween.Sequence()
                .Append(cg.DOFade(1f, 1f))
                .Join(cg.transform.DOScale(1.25f, _TEXT_SCALE_DURACTION).SetEase(Ease.OutQuart).SetLoops(_TEXT_YOYO_COUNT, LoopType.Yoyo));
        }

        private void SetTempUserCash(int cash)
        {
            _userTempCash = cash;
            userResultRsCashText.text = cash + "$";
        }

        private int GetTempResultUserCash()
        {
            int tempResult = _userTempCash;
            tempResult += _FIXED_ALLOWANCE;
            tempResult += _bonusCash;
            tempResult -= _rsTradersCash;
            return Mathf.Max(tempResult, 0);
        }

        private int GetRsTradersPayCash(RS_TradersDeliveryMapper[] deliveryMappers)
        {
            _rsTradersCash = 0;
            if (deliveryMappers == null || deliveryMappers.Length == 0) return _rsTradersCash;
            foreach (var delivery in deliveryMappers)
            {
                int itemPrice = itemDB.GetItemDataToItemId(delivery.itemId).itemSettings.itemPrice;
                _rsTradersCash += itemPrice * delivery.itemStack;
            }
            return _rsTradersCash;
        }

        private void CompleteSignEvent()
        {
            nextDayButton.GetComponent<CanvasGroup>().DOFade(1f, 0.25f);
            nextDayButton.interactable = true;
        }
        
        private void NextDayButtonClickEvent()
        {
            nextDayButton.gameObject.SetActive(false);
            audioSource.PlayOneShot(receiptEndingClip);
            mainCanvasGroup.blocksRaycasts = false;
            
            // 진통제 아이템 추가
            if (!debugMode && medicineToggle.isOn)
            {
                if (_playerSaveData.DeliveryItem == null) _playerSaveData.DeliveryItem = new RS_TradersDeliveryMapper[0];
                List<RS_TradersDeliveryMapper> deliveryItemsList = _playerSaveData.DeliveryItem.ToList();
                deliveryItemsList.Add(new RS_TradersDeliveryMapper(_MEDICINE_ITEM_ID, 1));
                _playerSaveData.DeliveryItem = deliveryItemsList.ToArray();
            }
            
            // 현재 데이터 세이브 파일 덮어쓰기
            if (!debugMode)
            {
                // 정신력
                _playerSaveData.Mentatilty = _tempMentality / 100f;
                // R.S 머니 
                _playerSaveData.RS_Cash = _userTempCash;
                // 세이브 현재시간 변경
                _playerSaveData.SaveDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                // 일차 증가 
                _playerSaveData.Day = Mathf.Min(_playerSaveData.Day + 1, DayAndRound.MAX_DAY);
                GameManager.Instance.SaveGameManager.SaveFile();
                GameManager.Instance.NextScene = GameManager.SceneName.Hotel;
                DOTweenManager.FadeOutCanvasGroup(mainCanvasGroup, 
                    2f, 
                    new List<UnityAction>
                    {
                        () => GameManager.Instance.OpenScene(GameManager.SceneName.Loading)
                    });
                receiptAnimation.Play("ReceiptOut");
            }
            else
            {
                DOTweenManager.FadeOutCanvasGroup(mainCanvasGroup, 
                    2f, 
                    new List<UnityAction>
                    {
                        () => Debug.Log("로딩 씬으로 이동!")
                    });
                receiptAnimation.Play("ReceiptOut");
            }
            
        }

        private void MedicalServiceToggleEvent(bool isMedicalService)
        {
            if (isMedicalService && !_medicalServiceMode)
            {
                SetTempUserCash(Mathf.Max(_userTempCash - _MEDICAL_SERVICE_COST, 0));
               _tempMentality = (int)Mentality.MAX_MENTALITY * 100;
            }
            else if(!isMedicalService && _medicalServiceMode)
            { 
                SetTempUserCash(_userTempCash + _MEDICAL_SERVICE_COST);
                _tempMentality = _originalMentality;
            }
            _medicalServiceMode = isMedicalService;
            SetCanvasGroupAlpha(noMedicalCanvasGroup, isMedicalService ? _TEXT_ALPHA_VALUE : 1f);
            SetCanvasGroupAlpha(defaultMedicalCanvasGroup, isMedicalService ? 1f : _TEXT_ALPHA_VALUE);
            ControlMedicineToggleButton();
        }

        private void MedicineServiceToggleEvent()
        {
            if (medicineToggle.isOn)
            {
                SetTempUserCash(Mathf.Max(_userTempCash - _medicineCost, 0));
            }
            else
            {
                SetTempUserCash(_userTempCash + _medicineCost);
            }
            SetCanvasGroupAlpha(medicineCanvasGroup, medicineToggle.isOn ? 1f : _TEXT_ALPHA_VALUE);
            ControlMedicineToggleButton();
        }

        private void ControlMedicineToggleButton()
        {
           bool enoughCash = _userTempCash - _MEDICAL_SERVICE_COST >= 0;

           if (enoughCash)
           {
               defaultMedicalToggle.interactable = true;
           }
           else
           {
               if (!defaultMedicalToggle.isOn) defaultMedicalToggle.interactable = false;
           }

           enoughCash = _userTempCash - _medicineCost >= 0;

           if (enoughCash)
           {
               medicineToggle.interactable = true;
           }
           else
           {
               if (!medicineToggle.isOn) medicineToggle.interactable = false;
           }
        }

        private void Update()
        {
            if (showToolTip)
            {
                Vector2 anchoredPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    medicalServiceTipObject.parent as RectTransform, 
                    Input.mousePosition, 
                    uiCam, 
                    out anchoredPosition
                );
            
                medicalServiceTipObject.anchoredPosition = anchoredPosition + offset;
            }
        }

        public void ShowMedicalServiceTip(MedicalServiceEnum tipTextKey, bool hideMentalityText)
        {
            //  medicalServiceTipObject.position = mousePosition + (Vector3)offset;
          //  medicalServiceTipObject.gameObject.SetActive(true);
          
            // UI 상에서의 마우스 위치 계산
            medicalServiceTipObject.gameObject.SetActive(true);
            medicalServiceTipText.text = GameManager.Instance.localizationTextManager.GetText(LocalizationTable.TextTable.UI_Table, tipTextKey);
            if (!hideMentalityText)
            {
                string upgradeValueText = _originalMentality.ToString();
                if (tipTextKey.Equals(MedicalServiceEnum.Adjustment_MedicalTip2))
                {
                    upgradeValueText = (Mentality.MAX_MENTALITY * 100).ToString();
                }
                medicalServiceTipMentalityText.text = _originalMentality + " -> " + upgradeValueText;
            }
            medicalServiceTipMentalityText.gameObject.SetActive(!hideMentalityText);
            medicalServiceTipText.gameObject.SetActive(false);
            medicalServiceTipText.gameObject.SetActive(true);
            showToolTip = true;
        }
        
        public void HideMedicalServiceTip()
        {
            medicalServiceTipObject.gameObject.SetActive(false);
            showToolTip = false;
        }

        private void AllRemoveToggleEvent()
        {
            nextDayButton.onClick.RemoveAllListeners();
            noMedicalToggle.onValueChanged.RemoveAllListeners();
            defaultMedicalToggle.onValueChanged.RemoveAllListeners();
            medicineToggle.onValueChanged.RemoveAllListeners();
        }

        private string GetPlaceHolderText(string originalText, string replacementText)
        {
            string placeholder = LocalizationVariableHelper.GetPlaceHolderString(originalText);
            if (!NoManualUtilsHelper.FindStringEmptyOrNull(placeholder))
            {
                return originalText.Replace(placeholder, replacementText);
            }
            return originalText;
        }

        private void SetTextAlpha(TextMeshProUGUI originalText, float alpha)
        {
            originalText.color = new Color(originalText.color.r, originalText.color.g, originalText.color.b, alpha);
        }

        private void SetCanvasGroupAlpha(CanvasGroup canvasG, float alpha)
        {
            canvasG.alpha = alpha;
        }

        private void OnDestroy()
        {
            AllRemoveToggleEvent();
        }
    }
}


