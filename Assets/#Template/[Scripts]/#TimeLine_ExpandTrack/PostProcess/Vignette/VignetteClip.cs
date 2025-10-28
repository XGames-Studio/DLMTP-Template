using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class VignetteClip : PlayableAsset, ITimelineClipAsset
{
    public VignetteBehaviour template = new VignetteBehaviour();
    public ClipCaps clipCaps => ClipCaps.Blending;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<VignetteBehaviour>.Create(graph, template);
        playable.SetPropagateSetTime(true);
        return playable;
    }
}