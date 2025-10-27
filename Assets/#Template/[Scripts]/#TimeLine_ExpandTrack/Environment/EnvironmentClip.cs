using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class EnvironmentClip : PlayableAsset, ITimelineClipAsset
{
    // Ƭ�εĻ��������ģ�壨�༭ʱ������Colorģʽ�����洢�ڴˣ�
    public EnvironmentBehaviour template = new EnvironmentBehaviour();

    /// <summary>
    /// Ƭ��֧�ֵĹ��ܣ�����Blending����ʵ�ֶ�Ƭ����ɫƽ����ϣ�
    /// </summary>
    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    /// <summary>
    /// ����Ƭ�ζ�Ӧ��Playableʵ������ģ�������������ʱ��
    /// </summary>
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<EnvironmentBehaviour>.Create(graph, template);
        return playable;
    }
}