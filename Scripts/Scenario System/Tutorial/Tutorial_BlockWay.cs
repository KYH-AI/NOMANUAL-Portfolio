using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tutorial_BlockWay : MonoBehaviour
{
    [SerializeField] private GameObject[] blockWays;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < blockWays.Length; i++)
            {
                blockWays[i].SetActive(false);
            }
            
            this.gameObject.SetActive(false);
        }
    }
}
