using UnityEngine;
using UnityEngine.Timeline;

[TrackBindingType(typeof(RayFireGroupReceiver))]
[TrackClipType(typeof(TimelineClip))]
[TrackColor(1f, 0.5f, 0.5f)] // Ʈ�� ���� ���� (���� ����)
public class RayFireMarkerTrack : MarkerTrack { }
