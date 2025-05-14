using UnityEngine;

/// <summary>
/// UI를 Drag 가능한 컴포넌트 인터페이스
/// </summary>
public interface IDragItem 
{
    /// <summary>
    /// 부모를 재설정하고 위치도 설정
    /// </summary>
    public void SetParentAndPosition(Transform parent);

    /// <summary>
    /// 현재 부모 트랜스폼 정보
    /// </summary>
    public Transform GetCurrentParent();
}
