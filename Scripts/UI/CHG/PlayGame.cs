using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGame : MonoBehaviour
{
    public GameManager.SceneName SceneToGo;
    public List<GameObject> UIToTurnOff;
    public VideoHandler _VideoHandler;
    public DemoMain_GlitchController glitchController;

    private double videoTime = 1;
    public void PlayGameButton()
    {
        if(_VideoHandler != null) videoTime = _VideoHandler.StartPrepareAndReturnTime(VideoName.EndVideo);
        foreach (var ui in UIToTurnOff)
        {
            ui.transform.localScale = new Vector3(0, 0, 0);
        }
        glitchController.CustomMatData();
        StartCoroutine(WairForEndOfVideo());
    }
    
    private IEnumerator WairForEndOfVideo()
    {
        yield return new WaitForSeconds((float)videoTime);
        GameManager.Instance.NextScene = SceneToGo;
        GameManager.Instance.OpenScene(GameManager.SceneName.Loading);
    }
}
