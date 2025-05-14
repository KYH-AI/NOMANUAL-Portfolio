using UnityEngine;

[System.Serializable]
public class ANO_JumpScare_Module 
{
    [Header("ANO �������ɾ� ID")]  
    [SerializeField, Range(2, 4)] public int jumpScareLevel;
    [Header("ANO �������ɾ� ������ ��")]  
    [SerializeField, Range(0f, 5f)] public int jumpScareDelay;
}
