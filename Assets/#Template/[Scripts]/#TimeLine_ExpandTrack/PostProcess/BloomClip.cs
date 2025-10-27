using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class BloomClip : PlayableAsset, ITimelineClipAsset
{
    // Ƭ�ε�Bloom����ģ�壨�༭ʱ�����Ĳ�����������
    public BloomBehaviour template = new BloomBehaviour();

    /// <summary>
    /// Ƭ��֧�ֵĹ��ܣ����뿪��Blending���ܻ�϶�Ƭ�Σ�
    /// </summary>
    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    /// <summary>
    /// ����Ƭ�ζ�Ӧ��Playable����ģ�����ʵ����������ʱ��
    /// </summary>
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        // ʵ����Bloom��Ϊ��������ģ�����
        var playable = ScriptPlayable<BloomBehaviour>.Create(graph, template);
        return playable;
    }
}