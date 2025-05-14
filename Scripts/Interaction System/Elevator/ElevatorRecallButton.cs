using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NoManual.Interaction
{
    public class ElevatorRecallButton : ElevatorButton
    {
        [Header("현재 층")]
        [SerializeField] private int currentFloor;

        protected override void InteractElevatorButton()
        {
            if (elevatorComponent.CurrentElevatorState == ElevatorComponent.ElevatorState.Busy) return;
        
            _elevatorButtonAudio.PlayOneShot(_elevatorButtonAudio.clip);
            ToggleFloorButtonColor(true);
           elevatorComponent.CallElevator(currentFloor, new List<UnityAction>{()=>ToggleFloorButtonColor(false)});
        }

    }
}


