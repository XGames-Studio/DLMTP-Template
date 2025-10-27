using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class VignetteClip : PlayableAsset, ITimelineClipAsset
{
    // 片段的Vignette参数模板（编辑时调整的参数存储于此）
    public VignetteBehaviour template = new VignetteBehaviour();

    /// <summary>
    /// 片段支持的功能（必须开启Blending才能实现多暗角片段平滑过渡）
    /// </summary>
    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    /// <summary>
    /// 创建片段对应的运行时实例（将模板参数传入PlayableGraph）
    /// </summary>
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<VignetteBehaviour>.Create(graph, template);
        return playable;
    }
}