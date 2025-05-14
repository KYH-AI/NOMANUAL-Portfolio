using UnityEngine;
using UnityEngine.Timeline;

[TrackBindingType(typeof(RayFireGroupReceiver))]
[TrackClipType(typeof(TimelineClip))]
[TrackColor(1f, 0.5f, 0.5f)] // 트랙 색상 설정 (선택 사항)
public class RayFireMarkerTrack : MarkerTrack { }
