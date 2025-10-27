using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class BloomClip : PlayableAsset, ITimelineClipAsset
{
    public BloomBehaviour template = new BloomBehaviour();

    public ClipCaps clipCaps => ClipCaps.Blending;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<BloomBehaviour>.Create(graph, template);
        playable.SetPropagateSetTime(true);
        return playable;
    }
}