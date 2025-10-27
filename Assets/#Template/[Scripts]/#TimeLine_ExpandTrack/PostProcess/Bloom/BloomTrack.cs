using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Rendering.PostProcessing;

// �����ɫ������ɫ������ Bloom �ķ������ԣ�
[TrackColor(0.9f, 0.9f, 0.3f)]
[TrackClipType(typeof(BloomClip))]
[TrackBindingType(typeof(PostProcessVolume))]
public class BloomTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<BloomMixerBehaviour>.Create(graph, inputCount);
    }

    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        base.GatherProperties(director, driver);
    }
}