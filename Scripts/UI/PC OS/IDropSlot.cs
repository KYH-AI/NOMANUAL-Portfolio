using UnityEngine;

/// <summary>
/// 드래그한 UI 대상으로 Drop 가능한 컴포넌트 인터페이스
/// </summary>
public interface IDropSlot
{
    /// <summary>
    /// 드랍 가능한 트랜스폼 정보
    /// </summary>
    public Transform GetDropSlotTransform();
}
