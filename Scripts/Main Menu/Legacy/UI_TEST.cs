using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_TEST : MonoBehaviour
{
    void Update()
    {
        // 마우스의 현재 위치에 있는 UI 오브젝트 가져오기
        GameObject hoveredObject = GetHoveredUIObject();

        // 가져온 UI 오브젝트의 이름 출력
        if (hoveredObject != null)
        {
            Debug.Log("Hovered UI Object: " + hoveredObject.name);
        }
    }

    GameObject GetHoveredUIObject()
    {
        // 마우스 이벤트를 통해 현재 마우스 위치에 있는 UI 오브젝트 가져오기
        if (EventSystem.current == null)
        {
            return null;
        }

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // 결과에서 가장 앞에 있는 UI 오브젝트 반환
        foreach (RaycastResult result in results)
        {
            return result.gameObject;
        }

        return null;
    }
}
