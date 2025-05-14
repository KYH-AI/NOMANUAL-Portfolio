using System.Collections;
using HFPS.Player;
using UnityEngine;
using NoManual.ANO;
using NoManual.Managers;

public class ANO38_DontLook3Time : ANO_Component
{
    [Header("ANO ¼³Á¤")] 
    [SerializeField] private GameObject[] anoObj;
    [SerializeField] private AudioSource anoSfx0;
    [SerializeField] private AudioSource anoSfx1;
    [SerializeField] private AudioSource anoSfx2;
    
    private int anoCount = 0;
    private bool[] hasBeenHit;    

    private void Start()
    {
        hasBeenHit = new bool[anoObj.Length]; 
    }

    private void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            for (int i = 0; i < anoObj.Length; i++)
            {
                if (anoObj[i] != null && hit.collider == anoObj[i].GetComponent<Collider>() && !hasBeenHit[i])
                {
                    anoSfx1.Play();
                    hasBeenHit[i] = true;
                    anoCount++;
                    StartCoroutine(RemoveAnoObjAfterDelay(anoObj[i], 1f));
                }
            }
            
            if (anoCount >= 3)
            {
                anoSfx0.Play();
                anoSfx2.Play();
                PlayerController.Instance.DecreaseMentality(40);
            }
        }
    }

    private IEnumerator RemoveAnoObjAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        anoSfx1.Stop();
        Destroy(obj);
    }
}