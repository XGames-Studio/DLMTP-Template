using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class DepthOfFieldBehaviour : PlayableBehaviour
{
    [Tooltip("�Ƿ����þ���Ч��")]
    public bool enableDepthOfField = true;

    [FoldoutGroup("���Ĳ���")]
    [Tooltip("���ࣨ0~100�ף������㵽����ľ���"), Range(0f, 100f)]
    public float focusDistance = 10f; // 3.5.1 Ĭ��ֵ

    [FoldoutGroup("���Ĳ���")]
    [Tooltip("��Ȧ��0.1~32������ֵԽС����Խǳ"), Range(0.1f, 32f)]
    public float aperture = 5.6f; // 3.5.1 Ĭ��ֵ

    [FoldoutGroup("��ͷ����")]
    [Tooltip("���೤�ȣ�12~400mm����Ӱ��͸�Ӻ;��Χ"), Range(12f, 400f)]
    public float focalLength = 50f; // 3.5.1 Ĭ��ֵ

    // �Ƴ� maxBlurSize��3.5.1 �汾�иò������Ƴ���Ǩ��
}