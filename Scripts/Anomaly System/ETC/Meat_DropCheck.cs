using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meat_DropCheck : MonoBehaviour
{
    [SerializeField] private AudioSource meatDropSfx;

    private bool isPlayed = false;
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Floor") && !isPlayed)
        {
            meatDropSfx.Play();
            StartCoroutine(StopMovementAfterDelay(1f)); // 1초 후 움직임 멈추기
        }
    }

    private IEnumerator StopMovementAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }
}