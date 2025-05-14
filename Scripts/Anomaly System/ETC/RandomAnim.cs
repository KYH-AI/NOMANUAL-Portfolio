using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnim : MonoBehaviour
{
    private Animator anim;
    private float randomOffset;

    void Start()
    {
        anim = GetComponent<Animator>();
        randomOffset = Random.Range(0f, 1f);
    }



}

