using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class DepthOfFieldClip : PlayableAsset, ITimelineClipAsset
{
    public DepthOfFieldBehaviour template = new DepthOfFieldBehaviour();

    public ClipCaps clipCaps => ClipCaps.Blending;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DepthOfFieldBehaviour>.Create(graph, template);
        playable.SetPropagateSetTime(true);
        return playable;
    }
}