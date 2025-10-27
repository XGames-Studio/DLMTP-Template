using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class EnvironmentBehaviour : PlayableBehaviour
{
    /// <summary>
    /// ��������ɫ����Ӧ Environment Lighting �� Source=Color ģʽ����ɫֵ��
    /// [ColorUsage(false, true)]������ HDR ʰȡ������֧�� HDR ��ֵ�����ϻ��������ã�
    /// </summary>
    [Tooltip("��������ɫ��Source=Colorģʽ�µ�ȫ�ֻ�����ɫ��")]
    [ColorUsage(false, true)]
    public Color Ambient_Color = new Color(0.2f, 0.4f, 0.8f); // Ĭ��ǳ��ɫ��ģ�������գ�
}