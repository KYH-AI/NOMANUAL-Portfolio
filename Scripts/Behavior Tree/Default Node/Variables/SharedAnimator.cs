using BehaviorDesigner.Runtime;
using UnityEngine;

[System.Serializable]
public class SharedAnimator : SharedVariable<Animator>
{
    public static explicit operator SharedAnimator(Animator value) { return new SharedAnimator { mValue = value }; }
}
