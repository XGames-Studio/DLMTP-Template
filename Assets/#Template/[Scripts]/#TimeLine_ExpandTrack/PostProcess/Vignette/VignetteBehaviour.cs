using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
public class VignetteBehaviour : PlayableBehaviour
{
    // ========================= Vignette核心参数（与Post Process V2官方参数名一致） =========================
    [Tooltip("是否启用暗角效果")]
    public bool enableVignette = true;

    [Tooltip("暗角强度（0=无暗角，1=全屏幕暗）"), Range(0f, 1f)]
    public float intensity = 0.5f; // 默认中等强度

    [Tooltip("暗角中心位置（屏幕坐标：0~1，(0.5,0.5)=屏幕中心）")]
    public Vector2 center = new Vector2(0.5f, 0.5f); // 默认中心

    [Tooltip("暗角边缘平滑度（0=硬边缘，1=极柔和）"), Range(0f, 1f)]
    public float smoothness = 0.2f; // 默认轻微平滑

    [Tooltip("暗角圆润度（0=方形暗角，1=圆形暗角）"), Range(0f, 1f)]
    public float roundness = 0.5f; // 默认半圆润

    [Tooltip("暗角颜色（通常为黑色/深棕色，模拟镜头暗角）")]
    public Color vignetteColor = Color.black; // 默认黑色

    [Tooltip("是否强制圆角暗角（优先级高于roundness，部分版本需开启才能生效）")]
    public bool rounded = true; // 默认启用圆角
}