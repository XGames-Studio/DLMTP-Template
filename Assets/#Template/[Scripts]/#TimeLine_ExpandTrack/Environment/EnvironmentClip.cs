using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class EnvironmentClip : PlayableAsset, ITimelineClipAsset
{
    public EnvironmentBehaviour template = new EnvironmentBehaviour();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<EnvironmentBehaviour>.Create (graph, template);
        return playable;
    }
}
