using NoManual.ANO;
using UnityEngine;

public class BadEnding_EightPillars : ANO_Component
{

    [SerializeField] private AudioSource anoAmbience;
    [SerializeField] private AudioSource whisperingSfx;
    [SerializeField] private AudioSource horrorSfx;
    [SerializeField] private Collider anoStart;
    [SerializeField] private Collider anoEnd;

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            anoStart.enabled = false;
            // 메인 BGM 종료
            NoManual.Managers.NoManualHotelManager.Instance.AudioManager.StopBGM();
            // ANO 8 공간 BGM Fade In
            NoManual.Managers.NoManualHotelManager.Instance.AudioManager.PlayBGM_Fade(anoAmbience, true, 1f, 5f);
            whisperingSfx.Play();
            horrorSfx.Play();
            
            // 정신력을 50으로 강제로 설정
            HFPS.Player.PlayerController.Instance.SetMentality(50);
        }

        if (anoTriggerZone == anoEnd)
        {
            anoEnd.enabled = false;
            anoAmbience.Stop();
            whisperingSfx.Stop();
            horrorSfx.Stop();
            
            // 정신력을 0으로 강제로 설정
            HFPS.Player.PlayerController.Instance.SetMentality(0);
            // 플레이어 사망 처리
            HFPS.Player.PlayerController.Instance.SelfDead();
            // 3초 딜레이 후 엔딩 씬으로 이동
            NoManual.Managers.NoManualHotelManager.Instance.OpenScene(GameManager.SceneName.Demo_Ending, 3f);
        }
    }
}
