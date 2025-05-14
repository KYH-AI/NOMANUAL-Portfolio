using System;
using UnityEngine;

namespace NoManual.Interaction
{
    public abstract class ElevatorButton : InteractionBase
    {
        protected ElevatorComponent elevatorComponent;
        protected AudioSource _elevatorButtonAudio;
        [SerializeField] private MeshRenderer buttonRenderer;
        private Material _buttonHighLightMaterial;
        private Material _buttonDefaultMaterial;

        private void Awake()
        {
            interactionType = InteractionType.Press;
            _elevatorButtonAudio = GetComponent<AudioSource>();
        }

        public void InitElevatorButton(ElevatorComponent mainElevator, Material buttonHighLightMaterial)
        {
            this.elevatorComponent = mainElevator;
            this._buttonHighLightMaterial = buttonHighLightMaterial;
        }

        public override void Interact()
        {
            InteractElevatorButton();
        }

        protected abstract void InteractElevatorButton();

        protected void ToggleFloorButtonColor(bool highLight)
        {
            if (highLight)
            {
                _buttonDefaultMaterial = buttonRenderer.material;
                buttonRenderer.material = _buttonHighLightMaterial;
            }
            else
            {
                buttonRenderer.material = _buttonDefaultMaterial;
            }
        }
    }
}


