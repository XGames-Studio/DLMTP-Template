using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

public class EnvironmentMixerBehaviour : PlayableBehaviour
{
    #region ȫ�ֻ�����Ĭ��ֵ��������Colorģʽ��Ҫ�Ĳ�����
    private Color Default_AmbientColor; // ԭʼ��������ɫ��Source=Colorģʽ��Ӧ��ֵ��
    #endregion

    private bool m_FirstFrameHappened; // ����Ƿ�Ϊ��һ֡������ʼ��һ��Ĭ��ֵ��

    /// <summary>
    /// ÿִ֡�У���϶�Ƭ����ɫ��������Ӧ�õ�ȫ�ֻ�����
    /// </summary>
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // ��һ֡������ԭʼ��������ɫ�������޸ĺ��޷��ָ���
        if (!m_FirstFrameHappened)
        {
            Default_AmbientColor = RenderSettings.ambientSkyColor; // Colorģʽ�»�����洢��ambientSkyColor
            m_FirstFrameHappened = true;
        }

        int inputCount = playable.GetInputCount(); // ��ǰ����ϵ�Ƭ������
        Color blended_AmbientColor = Color.clear; // ��Ϻ�Ļ�������ɫ
        float totalWeight = 0f; // ����Ƭ�ε���Ȩ�أ����ڹ�һ�����ֵ��

        // ��������Ƭ�Σ���Ȩ�ػ�ϻ�������ɫ
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i); // ��ǰƬ�ε�Ȩ�أ�0~1��
            if (Mathf.Approximately(inputWeight, 0f))
                continue; // Ȩ��Ϊ0��Ƭ����������Ӱ�죩

            // ��ȡ��ǰƬ�εĻ��������
            ScriptPlayable<EnvironmentBehaviour> inputPlayable =
                (ScriptPlayable<EnvironmentBehaviour>)playable.GetInput(i);
            EnvironmentBehaviour input = inputPlayable.GetBehaviour();

            // ��Ȩ���ۼ���ɫ��Ȩ��Խ�ߣ��Ի�Ͻ��Ӱ��Խ��
            blended_AmbientColor += input.Ambient_Color * inputWeight;
            totalWeight += inputWeight;
        }

        // Ӧ�û�Ϻ����ɫ��ȫ�ֻ����⣨������Ƭ�Ρ��͡���Ƭ�Ρ����������
        if (totalWeight > 0f)
        {
            // ��Ƭ�Σ���һ�������ɫ��������Ȩ��>1������ɫ������������ԭʼ��ɫ����
            blended_AmbientColor /= totalWeight;
            RenderSettings.ambientSkyColor = blended_AmbientColor + Default_AmbientColor * (1f - totalWeight);
        }
        else
        {
            // ��Ƭ�Σ��ָ�ԭʼ��������ɫ
            RenderSettings.ambientSkyColor = Default_AmbientColor;
        }

        // ǿ������AmbientModeΪFlat��ȷ����Source=Colorģʽƥ�䣬����ģʽ�����ݣ�
        if (RenderSettings.ambientMode != AmbientMode.Flat)
        {
            RenderSettings.ambientMode = AmbientMode.Flat;
        }
    }

    /// <summary>
    /// ���������ʱ���ָ�ԭʼ��������ɫ���˳�����/ Timelineֹͣʱ��Ч��
    /// </summary>
    public override void OnPlayableDestroy(Playable playable)
    {
        m_FirstFrameHappened = false;
        RenderSettings.ambientSkyColor = Default_AmbientColor; // �ָ���ʼ��ɫ
        RenderSettings.ambientMode = AmbientMode.Flat; // ������Source=Colorģʽһ��
    }
}