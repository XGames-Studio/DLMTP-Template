using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class MotionBlurBehaviour : PlayableBehaviour
{
    // �������������� enableMotionMotionBlur ��Ϊ enableMotionBlur
    [Tooltip("�Ƿ������˶�ģ��Ч��")]
    public bool enableMotionBlur = true; // �˴�ԭƴдΪ enableMotionMotionBlur���������ô���

    [FoldoutGroup("ģ��ǿ��")]
    [Tooltip("���ŽǶȣ�0~360�ȣ���ֵԽ��ģ��Խǿ"), Range(0f, 360f)]
    public float shutterAngle = 270f;

    [FoldoutGroup("ģ������")]
    [Tooltip("����������4~32����ֵԽ��ģ��Խƽ���������������ӣ�"), Range(4, 32)]
    public int sampleCount = 8;
}