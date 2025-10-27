using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class BloomClip : PlayableAsset, ITimelineClipAsset
{
    // 片段的Bloom参数模板（编辑时调整的参数会存在这里）
    public BloomBehaviour template = new BloomBehaviour();

    /// <summary>
    /// 片段支持的功能（必须开启Blending才能混合多片段）
    /// </summary>
    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    /// <summary>
    /// 创建片段对应的Playable（将模板参数实例化到运行时）
    /// </summary>
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        // 实例化Bloom行为，并传入模板参数
        var playable = ScriptPlayable<BloomBehaviour>.Create(graph, template);
        return playable;
    }
}