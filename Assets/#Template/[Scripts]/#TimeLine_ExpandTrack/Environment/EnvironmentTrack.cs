using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// �����ɫ��ǳ��ɫ����Ӧ������Colorģʽ�����ɫ����
[TrackColor(0.4f, 0.7f, 0.9f)]
// �����֧���Զ����EnvironmentClipƬ��
[TrackClipType(typeof(EnvironmentClip))]
// ������ض����󣨻�����Ϊȫ�����ã�RenderSettingsֱ�ӷ��ʣ�
public class EnvironmentTrack : TrackAsset
{
    /// <summary>
    /// �������������������Ƭ�ε���ɫ����߼���
    /// </summary>
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<EnvironmentMixerBehaviour>.Create(graph, inputCount);
    }

    /// <summary>
    /// �ռ�Timeline���¼�����ԣ�����ԭ�ṹ��ȷ���༭��Ԥ��������
    /// </summary>
    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        base.GatherProperties(director, driver);
    }
}