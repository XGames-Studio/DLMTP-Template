using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Rendering.PostProcessing;

// �����ɫ�����ɫ�ң����� AO �İ����ԣ�
[TrackColor(0.2f, 0.2f, 0.2f)]
[TrackClipType(typeof(AmbientOcclusionClip))]
[TrackBindingType(typeof(PostProcessVolume))]
public class AmbientOcclusionTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<AmbientOcclusionMixerBehaviour>.Create(graph, inputCount);
    }

    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        base.GatherProperties(director, driver);
    }
}