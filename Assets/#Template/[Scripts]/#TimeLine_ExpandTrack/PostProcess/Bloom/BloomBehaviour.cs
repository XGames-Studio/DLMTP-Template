using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class BloomBehaviour : PlayableBehaviour
{
    [Tooltip("�Ƿ����� Bloom Ч��")]
    public bool enableBloom = true;

    [FoldoutGroup("ǿ������ֵ")]
    [Tooltip("����ǿ�ȣ�0~10����ֵԽ��Ч��Խ����"), Range(0f, 10f)]
    public float intensity = 0.5f; // 3.5.1 Ĭ��ֵ

    [FoldoutGroup("ǿ������ֵ")]
    [Tooltip("������ֵ��0~10����������ֵ������Żᷢ��"), Range(0f, 10f)]
    public float threshold = 1f; // 3.5.1 Ĭ��ֵ

    [FoldoutGroup("��������ɢ")]
    [Tooltip("����ɣ�0~1����ֵԽ����ֵ��Ե����Խ���"), Range(0f, 1f)] // �û�ָ����Χ
    public float softKnee = 0.5f; // 3.5.1 Ĭ��ֵ

    [FoldoutGroup("��������ɢ")]
    [Tooltip("��ɢ�ȣ�1~10����ֵԽ�󣬷�����ɢ��ΧԽ��"), Range(1f, 10f)] // �û�ָ����Χ
    public float diffusion = 7f; // 3.5.1 Ĭ��ֵ
}