using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoManual.JumpScare
{
    [CreateAssetMenu(fileName = "Jump Scare DataBase", menuName = "JumpScare/JumpScareDataBase")]
    public class JumpScareDataBaseScriptable : ScriptableObject
    {
        public List<JumpScareScriptable> jumpScareDataBase = new List<JumpScareScriptable>();

        public JumpScareScriptable GetJumpScareDataToId(int jumpScareId)
        {
            foreach (var jumpScare in jumpScareDataBase)
            {
                if (jumpScare.jumpScareId == jumpScareId) return jumpScare;
            }
          
            return null;
        }
    }  
}


