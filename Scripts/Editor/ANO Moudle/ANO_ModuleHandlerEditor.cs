#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ANO_ModuleHandler))]
public class ANO_ModuleHandlerEditor : Editor
{
    private ANO_ModuleHandler moduleHandler;

    private void OnEnable()
    {
        moduleHandler = (ANO_ModuleHandler)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        UpdateComponents();
    }
    
    private void UpdateComponents()
    {
        // 각 컴포넌트 추가/제거 로직
        switch (moduleHandler.useANO_Collider_Object)
        {
            case true:
                if (moduleHandler.colliderObjectModule == null)
                {
                    moduleHandler.colliderObjectModule = moduleHandler.gameObject.AddComponent<ANO_Collider_Object_Module>();
                }
                break;

            case false:
                if (moduleHandler.colliderObjectModule != null)
                {
                    DestroyImmediate(moduleHandler.colliderObjectModule);
                    moduleHandler.colliderObjectModule = null;
                }
                break;
        }

        switch (moduleHandler.useANO_Drop_Effect)
        {
            case true:
                if (moduleHandler.dropEffectModule == null)
                {
                    moduleHandler.dropEffectModule = moduleHandler.gameObject.AddComponent<ANO_Drop_Effect_Module>();
                }
                break;

            case false:
                if (moduleHandler.dropEffectModule != null)
                {
                    DestroyImmediate(moduleHandler.dropEffectModule);
                    moduleHandler.dropEffectModule = null;
                }
                break;
        }

        switch (moduleHandler.useANO_Change_Effect)
        {
            case true:
                if (moduleHandler.changeEffectModule == null)
                {
                    moduleHandler.changeEffectModule = moduleHandler.gameObject.AddComponent<ANO_Change_Effect_Module>();
                }
                break;

            case false:
                if (moduleHandler.changeEffectModule != null)
                {
                    DestroyImmediate(moduleHandler.changeEffectModule);
                    moduleHandler.changeEffectModule = null;
                }
                break;
        }
        
        switch (moduleHandler.useANO_Animation_Effect)
        {
            case true:
                if (moduleHandler.animationEffectModule == null)
                {
                    moduleHandler.animationEffectModule = moduleHandler.gameObject.AddComponent<ANO_Animation_Effect_Module>();
                }
                break;

            case false:
                if (moduleHandler.animationEffectModule != null)
                {
                    DestroyImmediate(moduleHandler.animationEffectModule);
                    moduleHandler.animationEffectModule = null;
                }
                break;
        }
    }
}
#endif