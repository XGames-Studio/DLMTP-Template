using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Rendering.PostProcessing;

// �����ɫ�����غ�ɫ�����ϡ����ǡ��Ӿ����ԣ�
[TrackColor(0.45f, 0.25f, 0.2f)]
// �����֧��VignetteClipƬ��
[TrackClipType(typeof(VignetteClip))]
// �����󶨡���PostProcessVolume�����GameObject��
[TrackBindingType(typeof(PostProcessVolume))]
public class VignetteTrack : TrackAsset
{
    /// <summary>
    /// �����������������ģ������Ƭ��Vignette������ϣ�
    /// </summary>
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        // ʵ����Vignette�����������Ƭ��������inputCount��
        return ScriptPlayable<VignetteMixerBehaviour>.Create(graph, inputCount);
    }

    /// <summary>
    /// �ռ�Timeline���¼�����ԣ�������Bloom�ű��ṹһ�£�ȷ���༭��Ԥ��������
    /// </summary>
    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        base.GatherProperties(director, driver);
    }
}