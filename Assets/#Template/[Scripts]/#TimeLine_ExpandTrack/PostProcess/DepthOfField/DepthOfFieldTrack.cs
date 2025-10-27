using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Rendering.PostProcessing;

// �����ɫ�����ɫ�����ϡ�������Ӿ�������
[TrackColor(0.3f, 0.3f, 0.3f)]
[TrackClipType(typeof(DepthOfFieldClip))]
[TrackBindingType(typeof(PostProcessVolume))]
public class DepthOfFieldTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<DepthOfFieldMixerBehaviour>.Create(graph, inputCount);
    }

    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        base.GatherProperties(director, driver);
    }
}