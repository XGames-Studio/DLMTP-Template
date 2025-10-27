using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class AmbientOcclusionClip : PlayableAsset, ITimelineClipAsset
{
    public AmbientOcclusionBehaviour template = new AmbientOcclusionBehaviour();

    public ClipCaps clipCaps => ClipCaps.Blending;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<AmbientOcclusionBehaviour>.Create(graph, template);
        playable.SetPropagateSetTime(true);
        return playable;
    }
}