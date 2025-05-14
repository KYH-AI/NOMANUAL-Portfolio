using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class MenuContext : MonoBehaviour
{
    public abstract void Initialize(UnityAction onContextEvent, UnityAction offContextEvent);
    public abstract void OnContext(UnityAction buttonEvent);
    public abstract void OffContext(UnityAction buttonEvent);
}
