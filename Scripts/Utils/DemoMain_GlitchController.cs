using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector2 = UnityEngine.Vector2;

public class DemoMain_GlitchController : MonoBehaviour
{

    // 쉐이더와 연결된 머티리얼. 
    // 4.Settings 하위의 ScreenMat과 연결하면 됨
    public Material mat;

    // 노이즈 세기
    public float noiseAmount;

    // 글리치 (화면이 얼마만큼의 빈도로 글리치가 발생하는지) 속도
    public float glitchStrength;

    // 스캔라인 (1이 되야 없어짐)
    public float scanLinesStrength;

    // 
    public Vector2 remapOutMinMax;

    void Start()
    {
        //메인 씬 진입 시 기본 값으로 설정
        if(GameManager.Instance.IsSceneCorrect(GameManager.SceneName.Main)
           ||GameManager.Instance.IsSceneCorrect(GameManager.SceneName.Demo_Ending))
        {
            SetMatOriginShake();
        }
    }
    private void SetMatOriginShake()
    {
        mat.SetFloat("_NoiseAmount", 50f);
        mat.SetFloat("_GlitchStrength", 0.7f);
        mat.SetFloat("_ScanLinesStrength", 0.5f);
        mat.SetVector("RemapOutMinMax",new Vector2(-0.1f, 0.1f));
    }
    public void CustomMatData()
    {
        mat.SetFloat("_NoiseAmount", noiseAmount);
        mat.SetFloat("_GlitchStrength", glitchStrength);
        mat.SetFloat("_ScanLinesStrength", scanLinesStrength);
        mat.SetVector("RemapOutMinMax", remapOutMinMax);
    }

    /* Demo Main에서 End Video가 종료될 때, 
    
    mat의 noise Amount와 glitchStrength는 0으로, 
    scanLinesStrength는 1로 값 변경
    
    
    */
}
