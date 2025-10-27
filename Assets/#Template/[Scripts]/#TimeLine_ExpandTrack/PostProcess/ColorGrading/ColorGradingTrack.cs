using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Rendering.PostProcessing;

// ÿ������ֻ���һ�Σ�ɾ���ظ��� [TrackColor] [TrackClipType] [TrackBindingType]
[TrackColor(0.2f, 0.6f, 0.7f)]
[TrackClipType(typeof(ColorGradingClip))]
[TrackBindingType(typeof(PostProcessVolume))]
public class ColorGradingTrack : TrackAsset
{
    // ֻ����һ�� CreateTrackMixer ����
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<ColorGradingMixerBehaviour>.Create(graph, inputCount);
    }

    // ֻ����һ�� GatherProperties ����
    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        base.GatherProperties(director, driver); // �����������߼�
    }
}