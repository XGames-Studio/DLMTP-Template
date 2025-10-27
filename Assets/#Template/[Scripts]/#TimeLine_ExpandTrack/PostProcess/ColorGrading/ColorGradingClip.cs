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
        // ȷ��Ƭ��Ȩ��ģʽ��ȷ�����Ի�ϣ�
        playable.SetPropagateSetTime(true);
        return playable;
    }
}