using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Rendering.PostProcessing;

// 每个特性只标记一次，删除重复的 [TrackColor] [TrackClipType] [TrackBindingType]
[TrackColor(0.2f, 0.6f, 0.7f)]
[TrackClipType(typeof(ColorGradingClip))]
[TrackBindingType(typeof(PostProcessVolume))]
public class ColorGradingTrack : TrackAsset
{
    // 只保留一个 CreateTrackMixer 方法
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<ColorGradingMixerBehaviour>.Create(graph, inputCount);
    }

    // 只保留一个 GatherProperties 方法
    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        base.GatherProperties(director, driver); // 仅保留基础逻辑
    }
}