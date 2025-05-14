using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NoManual.Creature
{
    [CreateAssetMenu(fileName = "CreatureAnimatorStateSO",menuName = "Creature/AnimatorStateSO")]
    public class CreatureAnimatorStateSO : ScriptableObject
    {
        [SerializeField] private string[] AnimatorStateName;
        
        public string GetAnimatorStateName(string target)
        {
            return AnimatorStateName.FirstOrDefault(value => value.Equals(target));
        }
    }

}

