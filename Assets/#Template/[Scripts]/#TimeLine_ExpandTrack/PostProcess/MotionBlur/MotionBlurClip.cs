using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class MotionBlurClip : PlayableAsset, ITimelineClipAsset
{
    public MotionBlurBehaviour template = new MotionBlurBehaviour();

    public ClipCaps clipCaps => ClipCaps.Blending;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<MotionBlurBehaviour>.Create(graph, template);
        playable.SetPropagateSetTime(true);
        return playable;
    }
}