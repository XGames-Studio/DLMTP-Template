using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Rendering.PostProcessing;

// �����ɫ������ɫ����Bloom�Ӿ�������
[TrackColor(0.85f, 0.65f, 0.95f)]
// ���֧�ֵ�Ƭ������
[TrackClipType(typeof(BloomClip))]
// �����Ҫ�󶨵Ķ��󣨱����Ǵ�PostProcessVolume��GameObject��
[TrackBindingType(typeof(PostProcessVolume))]
public class BloomTrack : TrackAsset
{
    /// <summary>
    /// �����������������ģ������Ƭ�β�����ϣ�
    /// </summary>
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        // ʵ����Bloom�������������������������Ƭ��������
        return ScriptPlayable<BloomMixerBehaviour>.Create(graph, inputCount);
    }

    /// <summary>
    /// �ռ���Ҫ��Timeline��¼�����ԣ���ѡ����������Ч�ű��ṹһ�£�
    /// </summary>
    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        base.GatherProperties(director, driver);
    }
}