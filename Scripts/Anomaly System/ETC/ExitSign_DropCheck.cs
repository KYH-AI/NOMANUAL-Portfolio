using System;
using System.Collections;
using System.Collections.Generic;
using HFPS.Player;
using UnityEngine;

public class ExitSign_DropCheck : MonoBehaviour
{   
    // exit sign 떨어지는 소리 1번만 실행하기 위한 변수
    private bool isSfxPlayed;
    
    [SerializeField] private AudioSource exitSignDropSfx;

    private void Start()
    {
        isSfxPlayed = false;
    }

    // 땅과 충돌 여부 검사. collider
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            if (!isSfxPlayed)
            {
                exitSignDropSfx.Play();
                isSfxPlayed = true;
            }
        }
    }
}
