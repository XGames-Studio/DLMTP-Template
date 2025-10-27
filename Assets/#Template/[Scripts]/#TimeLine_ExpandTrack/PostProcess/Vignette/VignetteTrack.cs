using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Rendering.PostProcessing;

// 轨道颜色：深棕红色（贴合“暗角”视觉属性）
[TrackColor(0.45f, 0.25f, 0.2f)]
// 轨道仅支持VignetteClip片段
[TrackClipType(typeof(VignetteClip))]
// 轨道需绑定“带PostProcessVolume组件的GameObject”
[TrackBindingType(typeof(PostProcessVolume))]
public class VignetteTrack : TrackAsset
{
    /// <summary>
    /// 创建轨道混合器（核心：处理多片段Vignette参数混合）
    /// </summary>
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        // 实例化Vignette混合器，传入片段数量（inputCount）
        return ScriptPlayable<VignetteMixerBehaviour>.Create(graph, inputCount);
    }

    /// <summary>
    /// 收集Timeline需记录的属性（保持与Bloom脚本结构一致，确保编辑器预览正常）
    /// </summary>
    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        base.GatherProperties(director, driver);
    }
}