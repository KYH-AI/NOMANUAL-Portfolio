using UnityEngine;
using UnityEditor;

public class DisableDynamicOcclusion : ScriptableObject
{

    [MenuItem("Tools/Disable Dynamic Occlusion for All Children")]
    static void DisableDynamicOcclusionForAllChildren()
    {
        if (Selection.activeGameObject != null)
        {
            DisableOcclusionRecursively(Selection.activeGameObject.transform);
            Debug.Log("모든 하위 오브젝트의 다이나믹 오클루전 끔");
        }
        else
        {
            Debug.Log("오브젝트를 선택하세요.");
        }
    }

    static void DisableOcclusionRecursively(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.allowOcclusionWhenDynamic = false;
            }
            DisableOcclusionRecursively(child);
        }
    }
}
