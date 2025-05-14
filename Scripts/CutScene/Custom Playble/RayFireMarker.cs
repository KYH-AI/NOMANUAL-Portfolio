using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class RayFireMarker : Marker, INotification
{
    public PropertyName id { get; }
}
