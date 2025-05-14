using System;
using System.Collections;
using UnityEngine;

public class RootB_1 : ScenarioReceiverBase
{
    [SerializeField] private GameObject oil;
    [SerializeField] private AudioSource oilSfx;
    [SerializeField] private Collider sceneStart;

    private bool isOilActive = false;

    private bool isPlayerIn = false;
    
    private void Start()
    {
        oil.SetActive(false);
    }

    protected override void ScenarioLogic(ScenarioTriggerSender sender)
    {
        isPlayerIn = true;
    }

    public override void OnScenarioExit(ScenarioTriggerSender sender)
    {
        isPlayerIn = false;
    }

    private IEnumerator ActiveOil()
    {
        oilSfx.Play();
        yield return new WaitForSeconds(1f);
        oil.SetActive(true);
        isPlayerIn = true;
        isOilActive = true;
        
        yield return null;
    }

    private void Update()
    {
        if (isPlayerIn && PlayerAPI.GetBlinkEyeState())
        {
            StartCoroutine(ActiveOil());
        }
        
        if (isOilActive && oil.activeInHierarchy == false)
        {
            oilSfx.Stop();
        }
    }
}
