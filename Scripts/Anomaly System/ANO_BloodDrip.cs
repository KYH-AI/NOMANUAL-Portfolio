using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ANO_WallDrip : MonoBehaviour
{
    public GameObject anoPrefab1;
    public GameObject anoPrefab2;
    public Transform anoPos;

    private bool anoStarted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !anoStarted)
        {
            StartAno();
            anoStarted = true;
        }
    }


    private void StartAno()
    {
        if (anoPrefab1 != null && anoPos != null)
        {
            var position = anoPos.position;
            GameObject wallDrip = Instantiate(anoPrefab1, position, anoPos.rotation);
            GameObject wallDrip2 = Instantiate(anoPrefab2, position, anoPrefab2.transform.rotation);
            wallDrip.transform.SetParent(anoPos);
            wallDrip2.transform.SetParent(anoPos);
        }
    }
}
