using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class AmbientOcclusionBehaviour : PlayableBehaviour
{
    [Tooltip("是否启用环境光遮蔽效果")]
    public bool enableAmbientOcclusion = true;

    [FoldoutGroup("核心效果")]
    [Tooltip("AO强度（0~4）：值越大，缝隙阴影越明显"), Range(0f, 4f)]
    public float intensity = 1f; // 3.5.1 版本明确支持的参数

    [FoldoutGroup("核心效果")]
    [Tooltip("采样半径（0.1~10）：值越大，影响范围越大"), Range(0.1f, 10f)]
    public float radius = 0.3f; // 3.5.1 版本明确支持的参数

    // 移除 darkness 和 blurQuality：3.5.1 版本已不存在这两个参数
}