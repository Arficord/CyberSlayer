using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderTriggerController : MonoBehaviour
{
    public event Action<Transform> onTriggerEnter = delegate { };
    public event Action onTriggerExit = delegate { };
    public void OnTriggerEnter2D(Collider2D other)
    {
        onTriggerEnter.Invoke(other.transform);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        onTriggerExit.Invoke();
    }
}

[Serializable]
public class TransformEvent:UnityEvent<Transform>{ }
