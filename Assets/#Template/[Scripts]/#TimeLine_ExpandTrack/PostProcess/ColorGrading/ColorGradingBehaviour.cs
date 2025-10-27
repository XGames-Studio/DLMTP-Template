using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class ColorGradingBehaviour : PlayableBehaviour
{
    [Tooltip("�Ƿ�������ɫ�ּ�Ч��")]
    public bool enableColorGrading = true;

    [Tooltip("�������ȣ�-100=ȫ�ڣ�0=ԭʼ��100=������"), Range(-100f, 100f)]
    public float brightness = 0f;

    [Tooltip("�Աȶȣ�-100=�ͶԱȣ�0=ԭʼ��100=�߶Աȣ�"), Range(-100f, 100f)]
    public float contrast = 0f;

    // �ؼ������� saturation ��Χ��Ϊ [-100, 100]
    [Tooltip("���Ͷȣ�-100=�ڰף�0=ԭʼ��100=��߱��ͣ�"), Range(-100f, 100f)]
    public float saturation = 0f; // 0 Ϊԭʼ���Ͷȣ�3.5.1 �汾��׼ֵ��

    [Tooltip("ɫ��ƫ�ƣ�-180~180�ȣ�"), Range(-180f, 180f)]
    public float hueShift = 0f;

    [FoldoutGroup("ɫ����ɫ��")]
    [Tooltip("ɫ�£�-100=������0=ԭʼ��100=ů�ƣ�"), Range(-100f, 100f)]
    public float temperature = 0f;

    [FoldoutGroup("ɫ����ɫ��")]
    [Tooltip("ɫ��ƫ�ƣ�-100=ƫ�̣�0=ԭʼ��100=ƫƷ�죩"), Range(-100f, 100f)]
    public float tint = 0f;
}