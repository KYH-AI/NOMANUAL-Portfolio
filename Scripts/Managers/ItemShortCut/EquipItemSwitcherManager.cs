using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HFPS.Systems;

namespace NoManual.Managers
{
    public class EquipItemSwitcherManager : MonoBehaviour
    {
        public List<SwitcherBehaviour> equipItemList = new List<SwitcherBehaviour>();
        private Camera _mainCamera;

        private InventorySlotType _currentEquipItemSlotType;
        private int _currentEquipItemSlotId = -1;
        public int CurrentEquipItemId { get; set; } = -1;
        private int _tempEquipItemId = -1;

        #region 벽 판정 설정

        [Header("Wall Detecting")]
        public LayerMask HitMask;
        public bool detectWall;
        public bool showGizmos;
        public float wallHitRange;

        [Header("Wall Hit Position")]
        public Transform WallHitTransform;
        public float WallHitSpeed = 5f;
        public Vector3 WallHitHideWeapon;
        public Vector3 WallHitShowWeapon;

        [Header("Item On Start")]
        public bool startWithCurrentItem;
        public bool startWithoutAnimation;

        [Tooltip("Item ID in the inventory database. Leave -1 unless it is an inventory item.")]
        public int startingItemID = -1;

        [HideInInspector]
        public int weaponItem = -1;

        private int newItem = 0;

        private bool hideWeapon;
        private bool handsFreed;
        private bool antiSpam;
        private bool spam;

        #endregion

        private void Awake()
        {
            _mainCamera = ScriptManager.Instance.MainCamera;
        }

        void Update()
        {
            WallRayCastHit();
        }
        
        
        /// <summary>
        /// Wall Hit 판정 확인
        /// </summary>
        private void WallRayCastHit()
        {
            if (WallHitTransform && detectWall && !handsFreed && CurrentEquipItemId != -1)
            {
                if (OnWallHit())
                {
                    if (equipItemList[CurrentEquipItemId] != null)
                    {
                        equipItemList[CurrentEquipItemId].OnSwitcherWallHit(true);
                    }

                    hideWeapon = true;
                }
                else
                {
                    if (equipItemList[CurrentEquipItemId] != null)
                    {
                        equipItemList[CurrentEquipItemId].OnSwitcherWallHit(false);
                    }

                    hideWeapon = false;
                }
            }
            
            if (WallHitTransform)
            {
                if (!hideWeapon)
                {
                    WallHitTransform.localPosition = Vector3.MoveTowards(WallHitTransform.localPosition, WallHitShowWeapon, Time.deltaTime * WallHitSpeed);
                }
                else
                {
                    WallHitTransform.localPosition = Vector3.MoveTowards(WallHitTransform.localPosition, WallHitHideWeapon, Time.deltaTime * WallHitSpeed);
                }
            }

        }
        
        /// <summary>
        /// Wall Hit Ray Cast
        /// </summary>
        private bool OnWallHit()
        {
            if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.TransformDirection(Vector3.forward), out RaycastHit hit, wallHitRange, HitMask))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        /// <summary>
        /// 장착 아이템 교체
        /// </summary>
        public void SelectSwitcherItem(int switchID)
        {
            if (switchID == -1) return;
            
            // 아이템 착용
            if (switchID != CurrentEquipItemId)
            {
                _tempEquipItemId = switchID;

                if (IsItemsDeactivated())
                {
                    if (equipItemList[_tempEquipItemId] != null)
                    {
                        equipItemList[_tempEquipItemId].OnSwitcherSelect();
                        CurrentEquipItemId = _tempEquipItemId;
                    }
                    else
                    {
                        Debug.LogError("[Item Switcher] Object does not contains SwitcherBehaviour subcalss!");
                    }
                }
                else
                {
                    StopAllCoroutines();
                    StartCoroutine(SwitchItem());
                }
            }
            // 착용 해체
            else
            {
                DeselectItems();
            }
        }

        /// <summary>
        /// 아이템 선택 해체 (비활성화)
        /// </summary>
        public void DeselectItems()
        {
            // 현재 활성화 된 아이템이 없는 경우 예외처리
            if (CurrentEquipItemId == -1) return;

            // 장착된 아이템 선택 해체
            equipItemList[CurrentEquipItemId].OnSwitcherDeselect();
            StopAllCoroutines();
            StartCoroutine(DeselectWait());
        }

        /// <summary>
        /// 아이템 선택해체 될 떄까지 확인
        /// </summary>
        IEnumerator DeselectWait()
        {
            yield return new WaitUntil(() => IsItemsDeactivated());
            CurrentEquipItemId = -1;
        }
        
        /// <summary>
        /// 현재 선택된 장작 아이템 얻기 (JumpScare 용도)
        /// </summary>
        public GameObject GetCurrentItem()
        {
            if (CurrentEquipItemId != -1)
            {
                return equipItemList[CurrentEquipItemId].gameObject;
            }

            return null;
        }
        
        /// <summary>
        /// 장착 아이템 비활성화
        /// </summary>
        public void DisableItems()
        {
            if (CurrentEquipItemId == -1) return;

            equipItemList[CurrentEquipItemId].GetComponent<SwitcherBehaviour>().OnSwitcherDeactivate();
            CurrentEquipItemId = -1;
        }
        

        /// <summary>
        /// 모든 장착 아이템 비활성화 여부 확인
        /// </summary>
        bool IsItemsDeactivated()
        {
            return equipItemList.All(x => !x.transform.GetChild(0).gameObject.activeSelf);
        }

        /// <summary>
        /// 장비 아이템 교체 코루틴
        /// </summary>
        IEnumerator SwitchItem()
        {
            if (CurrentEquipItemId > -1 && _tempEquipItemId > -1)
            {
                // 기존에 있던 아이템 비활성화
                equipItemList[CurrentEquipItemId].OnSwitcherDeselect();

                // 비활성화 될 때까지 대기
                yield return new WaitUntil(() => !equipItemList[CurrentEquipItemId].transform.GetChild(0).gameObject.activeSelf);

                // 새롭게 장착한 아이템 활성화
                if (equipItemList[_tempEquipItemId] != null)
                {
                    equipItemList[_tempEquipItemId].OnSwitcherSelect();
                    CurrentEquipItemId = _tempEquipItemId;
                }
                else
                {
                    Debug.LogError("[Item Switcher] Object does not contains SwitcherBehaviour subcalss!");
                }

                yield return new WaitForSeconds(1f);
            }
        }
        
        
        /// <summary>
        /// Wall Hit 판정 기즈모
        /// </summary>
        void OnDrawGizmosSelected()
        {
            if (detectWall && showGizmos)
            {
                if (_mainCamera != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawRay(_mainCamera.transform.position, _mainCamera.transform.TransformDirection(Vector3.forward * wallHitRange));
                }
            }
        }
    }
}
