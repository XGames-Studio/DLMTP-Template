using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class VignetteClip : PlayableAsset, ITimelineClipAsset
{
    // Ƭ�ε�Vignette����ģ�壨�༭ʱ�����Ĳ����洢�ڴˣ�
    public VignetteBehaviour template = new VignetteBehaviour();

    /// <summary>
    /// Ƭ��֧�ֵĹ��ܣ����뿪��Blending����ʵ�ֶవ��Ƭ��ƽ�����ɣ�
    /// </summary>
    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    /// <summary>
    /// ����Ƭ�ζ�Ӧ������ʱʵ������ģ���������PlayableGraph��
    /// </summary>
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<VignetteBehaviour>.Create(graph, template);
        return playable;
    }
}