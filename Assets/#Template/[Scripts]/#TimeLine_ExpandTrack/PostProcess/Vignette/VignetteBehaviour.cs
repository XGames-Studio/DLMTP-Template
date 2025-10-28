using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class VignetteBehaviour : PlayableBehaviour
{
    [Tooltip("�Ƿ����ð���Ч��")]
    public bool enableVignette = true; // ��ȷ��bool ����

    [Tooltip("ǿ�ȣ�0~1����ֵԽ�󣬱�ԵԽ��"), Range(0f, 1f)]
    public float intensity = 0f; // ƥ��Դ�룺FloatParameter

    [Tooltip("����λ�ã�x:0~1, y:0~1����(0.5,0.5) Ϊ��Ļ����")]
    public Vector2 center = new Vector2(0.5f, 0.5f); // ƥ��Դ�룺Vector2Parameter

    [Tooltip("��״���ɣ�0~1����0=���Σ�1=Բ�ι���"), Range(0f, 1f)]
    public float roundness = 1f; // ƥ��Դ�룺roundness��FloatParameter��

    [Tooltip("�Ƿ�ǿ��Բ�Σ�true=Բ�Σ�false=����Ļ�����仯��")]
    public bool rounded = false; // ƥ��Դ�룺rounded��BoolParameter��

    [Tooltip("������ɫ")]
    public Color color = Color.black; // ƥ��Դ�룺ColorParameter
}