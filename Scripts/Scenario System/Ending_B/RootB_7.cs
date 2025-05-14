using System;
using System.Collections;
using System.Collections.Generic;
using HFPS.Player;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class RootB_7 : ScenarioReceiverBase
{
    [SerializeField] private AudioSource sfx0;
    [SerializeField] private AudioSource sfx1;
    [SerializeField] private AudioSource sfx2;
    [SerializeField] private Transform teleportPosition;
    [SerializeField] private float delayAfterBlink = 1f;

    private bool isPlayerIn = false;
    
    private void FixedUpdate()
    {
        if (PlayerAPI.GetBlinkEyeState() && isPlayerIn)
        {
            StartCoroutine(TeleportPlayer());
        }
    }

    protected override void ScenarioLogic(ScenarioTriggerSender sender)
    {
        // sfx0 Àç»ý
        sfx0.Play();

        isPlayerIn = true;
    }

    public override void OnScenarioExit(ScenarioTriggerSender sender)
    {
        isPlayerIn = false;
    }

    private IEnumerator TeleportPlayer()
    {
        sfx1.Play();
        
        yield return new WaitForSeconds(delayAfterBlink);

        Transform playerTransform = PlayerController.Instance.transform;
        playerTransform.position = teleportPosition.position;
        playerTransform.rotation = playerTransform.rotation;
        
        sfx1.Stop();
        sfx2.Play();
    }
}
