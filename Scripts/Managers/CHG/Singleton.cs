using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singletion<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }
    private static void AddInstace()
    {
        var component = FindObjectOfType<T>();
        if (component == null)
        {
            GameObject obj = new GameObject(typeof(T).Name);
            instance = obj.AddComponent<T>();
        }
        else
        {
            instance = component.GetComponent<T>();
        }
    }
    protected virtual void Awake()
    {
        if (instance == null)
        {
            AddInstace();
        }
        else
        {
            Destroy(this.gameObject);
        }
        Init();
    }
    protected virtual void Init() { }
}