using System.Collections;
using System.Collections.Generic;
using NoManual.Managers;
using UnityEngine;

public class RootA_7 : MonoBehaviour
{
    public void EndingScene()
    {
        // 독백 씬에서 세이브 데이터 일차 판단 후 로딩 씬 진행
        HotelManager.Instance.ExitHotel(true, false, GameManager.SceneName.Monologue);
    }
}
