using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class EnvironmentClip : PlayableAsset, ITimelineClipAsset
{
    // 片段的环境光参数模板（编辑时调整的Color模式参数存储于此）
    public EnvironmentBehaviour template = new EnvironmentBehaviour();

    /// <summary>
    /// 片段支持的功能（开启Blending才能实现多片段颜色平滑混合）
    /// </summary>
    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    /// <summary>
    /// 创建片段对应的Playable实例（将模板参数传入运行时）
    /// </summary>
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<EnvironmentBehaviour>.Create(graph, template);
        return playable;
    }
}