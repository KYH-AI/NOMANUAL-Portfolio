using System.Collections;
using System.Collections.Generic;
using NoManual.Managers;
using UnityEngine;

public class RootA_7 : MonoBehaviour
{
    public void EndingScene()
    {
        // ���� ������ ���̺� ������ ���� �Ǵ� �� �ε� �� ����
        HotelManager.Instance.ExitHotel(true, false, GameManager.SceneName.Monologue);
    }
}
