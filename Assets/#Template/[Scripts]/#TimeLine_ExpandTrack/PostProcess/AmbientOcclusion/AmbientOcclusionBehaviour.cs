using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class AmbientOcclusionBehaviour : PlayableBehaviour
{
    [Tooltip("�Ƿ����û������ڱ�Ч��")]
    public bool enableAmbientOcclusion = true;

    [FoldoutGroup("����Ч��")]
    [Tooltip("AOǿ�ȣ�0~4����ֵԽ�󣬷�϶��ӰԽ����"), Range(0f, 4f)]
    public float intensity = 1f; // 3.5.1 �汾��ȷ֧�ֵĲ���

    [FoldoutGroup("����Ч��")]
    [Tooltip("�����뾶��0.1~10����ֵԽ��Ӱ�췶ΧԽ��"), Range(0.1f, 10f)]
    public float radius = 0.3f; // 3.5.1 �汾��ȷ֧�ֵĲ���

    // �Ƴ� darkness �� blurQuality��3.5.1 �汾�Ѳ���������������
}