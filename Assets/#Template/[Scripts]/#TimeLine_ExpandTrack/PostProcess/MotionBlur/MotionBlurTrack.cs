using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Rendering.PostProcessing;

// �����ɫ������ɫ�����ϡ��˶�ģ�������Ӿ�������
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