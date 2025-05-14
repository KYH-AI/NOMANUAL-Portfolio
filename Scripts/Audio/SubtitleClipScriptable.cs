using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipName",menuName = "AudioClip/SubtitleClipScriptbale")]
public class SubtitleClipScriptable : AudioClipScriptable
{
    [Header("자막 음성 Key")]
    public LocalizationTable.NPCTableTextKey subtitleEnum;
}
