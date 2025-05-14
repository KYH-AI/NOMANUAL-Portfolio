using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANO_PictureDrop : MonoBehaviour
{
    public GameObject defaultPic;
    public GameObject anoPic;
    public GameObject[] pictures;

    // 초기값
    private void init()
    {
        defaultPic.SetActive(false);
        anoPic.SetActive(true);
    }

    // 지금은 Awake지만, 선별 기준에 뽑힐 때 실행되어야 함
    private void Awake()
    {
        init();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject picture in pictures)
            {
                // picture GameObject에 Rigidbody가 없는 경우에만 Rigidbody를 추가합니다.
                if (!picture.GetComponent<Rigidbody>())
                {
                    picture.AddComponent<Rigidbody>();
                }
            }
        }
    }
}