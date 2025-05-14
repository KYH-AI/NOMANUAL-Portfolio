using System.Collections;
using System.Collections.Generic;
using HFPS.Player;
using UnityEngine;

namespace NoManual.StateMachine
{

    public enum PlayerSpecialState
    {
        None = -1,
        Idle = 0,
        HoldBreath = 1,
        BlinkEye = 2,
        Exhausted = 3,
    }

    public enum PlayerMovementState
    {
        Idle = 0,
        Walk = 1,
        CrouchWalk = 2,
        Run = 3,
    }
    
    public abstract class BaseState
    {
        public abstract void OnEnterState();
        public abstract void OnUpdateState();
        public abstract void OnFixedUpdateState();
        public abstract void OnExitState();
    }
}


