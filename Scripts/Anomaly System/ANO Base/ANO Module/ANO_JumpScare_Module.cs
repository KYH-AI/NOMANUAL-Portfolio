using UnityEngine;

[System.Serializable]
public class ANO_JumpScare_Module 
{
    [Header("ANO 점프스케어 ID")]  
    [SerializeField, Range(2, 4)] public int jumpScareLevel;
    [Header("ANO 점프스케어 딜레이 값")]  
    [SerializeField, Range(0f, 5f)] public int jumpScareDelay;
}
