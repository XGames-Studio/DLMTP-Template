using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Rendering.PostProcessing;

// 轨道颜色：深蓝色（贴合“运动模糊”的视觉关联）
[TrackColor(0.1f, 0.2f, 0.5f)]
[TrackClipType(typeof(MotionBlurClip))]
[TrackBindingType(typeof(PostProcessVolume))]
public class MotionBlurTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<MotionBlurMixerBehaviour>.Create(graph, inputCount);
    }

    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        base.GatherProperties(director, driver);
    }
}