using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSpinner : MonoBehaviour
{
    [SerializeField] private float speed;
    private RectTransform _rectTransform;
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        _rectTransform.Rotate(0f, 0f, -speed * Time.deltaTime);
    }
}
