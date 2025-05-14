using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace NoManual.UI
{
    [RequireComponent(typeof(Button))]
    public abstract class UI_MonoButtonBase : UI_Base
    {
        protected Button button;
        
        public override void UIInitialize()
        {
            button = GetComponent<Button>();
        }
        
        public abstract void AddButtonEvent(UnityAction unityEvent);
    
        public abstract void OnClickEvent();
    
    }
}

