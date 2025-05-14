using UnityEngine;

public enum BgmEnum
{
    Hospital,
    Hotel,
    ChaseStart,
}

[CreateAssetMenu(fileName = "AudioClipName",menuName = "AudioClip/BgmClipScriptable")]
public class BgmClipScriptable : AudioClipScriptable
{
    [Header("배경음 Key")]
    public BgmEnum bgmEnum;
}
