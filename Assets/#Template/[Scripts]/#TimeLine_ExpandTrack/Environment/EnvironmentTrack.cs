using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// 轨道颜色：浅蓝色（对应环境光Color模式的天空色调）
[TrackColor(0.4f, 0.7f, 0.9f)]
// 轨道仅支持自定义的EnvironmentClip片段
[TrackClipType(typeof(EnvironmentClip))]
// 无需绑定特定对象（环境光为全局设置，RenderSettings直接访问）
public class EnvironmentTrack : TrackAsset
{
    /// <summary>
    /// 创建轨道混合器（处理多片段的颜色混合逻辑）
    /// </summary>
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<EnvironmentMixerBehaviour>.Create(graph, inputCount);
    }

    /// <summary>
    /// 收集Timeline需记录的属性（保持原结构，确保编辑器预览正常）
    /// </summary>
    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        base.GatherProperties(director, driver);
    }
}