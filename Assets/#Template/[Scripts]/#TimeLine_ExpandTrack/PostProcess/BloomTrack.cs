using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Rendering.PostProcessing;

// 轨道颜色：淡紫色（与Bloom视觉关联）
[TrackColor(0.85f, 0.65f, 0.95f)]
// 轨道支持的片段类型
[TrackClipType(typeof(BloomClip))]
// 轨道需要绑定的对象（必须是带PostProcessVolume的GameObject）
[TrackBindingType(typeof(PostProcessVolume))]
public class BloomTrack : TrackAsset
{
    /// <summary>
    /// 创建轨道混合器（核心：处理多片段参数混合）
    /// </summary>
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        // 实例化Bloom混合器，并传入输入数量（多片段数量）
        return ScriptPlayable<BloomMixerBehaviour>.Create(graph, inputCount);
    }

    /// <summary>
    /// 收集需要被Timeline记录的属性（可选，保持与雾效脚本结构一致）
    /// </summary>
    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        base.GatherProperties(director, driver);
    }
}