using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
public class BloomBehaviour : PlayableBehaviour
{
    // ����Bloom��������������Post Process V2��ȫƥ�䣩
    [Tooltip("�Ƿ�����BloomЧ��")]
    public bool enableBloom = true;

    [Tooltip("Bloom����ǿ�ȣ����Ĳ�����"), Range(0f, 20f)]
    public float intensity = 1f;

    [Tooltip("����Bloom��������ֵ��������ֵ�����زŻᷢ�⣩"), Range(0f, 10f)]
    public float threshold = 1f;

    [Tooltip("��ֵ������Ͷȣ�0=Ӳ��Ե��1=����ͣ�"), Range(0f, 1f)]
    public float softKnee = 0.5f;

    [Tooltip("Bloom��ɢ�뾶���ٷ���������diffusion��"), Range(0f, 10f)]
    public float diffusion = 4f; // �ؼ��޸ģ�radius �� diffusion

    [Tooltip("Bloom��ɫ�˾���Ĭ�ϰ�ɫ=ԭɫ���⣩")]
    public Color bloomColor = Color.white;

    // ��ͷ���ղ�������������ٷ�����ƥ�䣩
    [Tooltip("��ͷ��������ģ�⾵ͷ�ҳ��ķ���Ч����")]
    public Texture dirtTexture;

    [Tooltip("��ͷ����ǿ�ȣ�������������Ϊ��ʱ��Ч��"), Range(0f, 2f)]
    [ShowIf("HasDirtTexture")]
    public float dirtIntensity = 0.5f;

    // ������ʾ�жϣ�Odin�ã�
    private bool HasDirtTexture => dirtTexture != null;
}