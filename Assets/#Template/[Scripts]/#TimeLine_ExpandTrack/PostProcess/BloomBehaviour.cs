using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
public class BloomBehaviour : PlayableBehaviour
{
    // 基础Bloom参数（参数名与Post Process V2完全匹配）
    [Tooltip("是否启用Bloom效果")]
    public bool enableBloom = true;

    [Tooltip("Bloom发光强度（核心参数）"), Range(0f, 20f)]
    public float intensity = 1f;

    [Tooltip("触发Bloom的亮度阈值（超过此值的像素才会发光）"), Range(0f, 10f)]
    public float threshold = 1f;

    [Tooltip("阈值过渡柔和度（0=硬边缘，1=极柔和）"), Range(0f, 1f)]
    public float softKnee = 0.5f;

    [Tooltip("Bloom扩散半径（官方参数名：diffusion）"), Range(0f, 10f)]
    public float diffusion = 4f; // 关键修改：radius → diffusion

    [Tooltip("Bloom颜色滤镜（默认白色=原色发光）")]
    public Color bloomColor = Color.white;

    // 镜头污渍参数（保留，与官方参数匹配）
    [Tooltip("镜头污渍纹理（模拟镜头灰尘的发光效果）")]
    public Texture dirtTexture;

    [Tooltip("镜头污渍强度（仅当污渍纹理不为空时生效）"), Range(0f, 2f)]
    [ShowIf("HasDirtTexture")]
    public float dirtIntensity = 0.5f;

    // 条件显示判断（Odin用）
    private bool HasDirtTexture => dirtTexture != null;
}