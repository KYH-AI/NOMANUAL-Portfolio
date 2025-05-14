using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NoManual.Interaction
{
    public class ElevatorFloorButton : ElevatorButton
    {
        [Header("목표 층")]
        [SerializeField] private int targetFloor;

        protected override void InteractElevatorButton()
        {
            if (elevatorComponent.CurrentElevatorState == ElevatorComponent.ElevatorState.Busy) return;
            
            _elevatorButtonAudio.PlayOneShot(_elevatorButtonAudio.clip);
            ToggleFloorButtonColor(true);
            elevatorComponent.TeleportElevator(targetFloor, new List<UnityAction>{()=>ToggleFloorButtonColor(false)} );
        }

    }
}


