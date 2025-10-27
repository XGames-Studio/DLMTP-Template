using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class ColorGradingClip : PlayableAsset, ITimelineClipAsset
{
    public ColorGradingBehaviour template = new ColorGradingBehaviour();

    public ClipCaps clipCaps => ClipCaps.Blending;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ColorGradingBehaviour>.Create(graph, template);
        // 确保片段权重模式正确（线性混合）
        playable.SetPropagateSetTime(true);
        return playable;
    }
}