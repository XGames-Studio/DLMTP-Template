using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Rendering.PostProcessing;

// 轨道颜色：深棕色（贴合暗角的视觉特性）
[TrackColor(0.3f, 0.2f, 0.1f)]
[TrackClipType(typeof(VignetteClip))]
[TrackBindingType(typeof(PostProcessVolume))]
public class VignetteTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<VignetteMixerBehaviour>.Create(graph, inputCount);
    }

    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        base.GatherProperties(director, driver);
    }
}