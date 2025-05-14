using System;
using System.Collections;
using System.Collections.Generic;
using HFPS.Player;
using UnityEngine;

public class TestMentalityZone : MonoBehaviour
{
    public bool isIncreaseMentalityZone;
    public float mentalityValue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = PlayerController.Instance;
            
            if (isIncreaseMentalityZone)
            {
                player.IncreaseMentality(mentalityValue);
            }
            else
            {
                player.DecreaseMentality(mentalityValue);
            }
        }
    }
}
