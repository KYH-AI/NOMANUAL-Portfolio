using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using NoManual.Creature;
using UnityEngine;

public class SharedCreatureAudio : SharedVariable<CreatureAudioHandler>
{
    public static implicit operator SharedCreatureAudio(CreatureAudioHandler value) { return new SharedCreatureAudio { mValue = value }; }
}
