using UnityEngine.Events;

namespace NoManual.UI
{
    public class UI_MonoButton : UI_MonoButtonBase
    {
        public override void AddButtonEvent(UnityAction buttonEvent)
        {
            button.onClick.AddListener(buttonEvent);
        }
        

        public override void OnClickEvent()
        {
            
        }

    }
}