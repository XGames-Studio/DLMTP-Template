using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
public class VignetteBehaviour : PlayableBehaviour
{
    // ========================= Vignette���Ĳ�������Post Process V2�ٷ�������һ�£� =========================
    [Tooltip("�Ƿ����ð���Ч��")]
    public bool enableVignette = true;

    [Tooltip("����ǿ�ȣ�0=�ް��ǣ�1=ȫ��Ļ����"), Range(0f, 1f)]
    public float intensity = 0.5f; // Ĭ���е�ǿ��

    [Tooltip("��������λ�ã���Ļ���꣺0~1��(0.5,0.5)=��Ļ���ģ�")]
    public Vector2 center = new Vector2(0.5f, 0.5f); // Ĭ������

    [Tooltip("���Ǳ�Եƽ���ȣ�0=Ӳ��Ե��1=����ͣ�"), Range(0f, 1f)]
    public float smoothness = 0.2f; // Ĭ����΢ƽ��

    [Tooltip("����Բ��ȣ�0=���ΰ��ǣ�1=Բ�ΰ��ǣ�"), Range(0f, 1f)]
    public float roundness = 0.5f; // Ĭ�ϰ�Բ��

    [Tooltip("������ɫ��ͨ��Ϊ��ɫ/����ɫ��ģ�⾵ͷ���ǣ�")]
    public Color vignetteColor = Color.black; // Ĭ�Ϻ�ɫ

    [Tooltip("�Ƿ�ǿ��Բ�ǰ��ǣ����ȼ�����roundness�����ְ汾�迪��������Ч��")]
    public bool rounded = true; // Ĭ������Բ��
}