using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour를 상속받지 않는 스크립트에서 코루틴이 필요한 경우 사용
/// </summary>
public class CoroutineManager : MonoBehaviour
{
    public Coroutine StartCoroutineProcess(IEnumerator process)
    {
        if (process == null)
        {
            Debug.LogError("Coroutine process is null. Cannot start null coroutine.");
            return null;
        }
        
        return StartCoroutine(process);
    }

    public void StopCoroutineProcess(Coroutine process)
    {
        if (process != null)
        {
            StopCoroutine(process);
        }
        else
        {
            Debug.LogWarning("Coroutine process reference is null. Cannot stop null coroutine.");
        }
    }
}
