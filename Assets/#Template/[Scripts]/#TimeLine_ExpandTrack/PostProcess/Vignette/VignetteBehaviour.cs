using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class VignetteBehaviour : PlayableBehaviour
{
    [Tooltip("是否启用暗角效果")]
    public bool enableVignette = true; // 正确：bool 类型

    [Tooltip("强度（0~1）：值越大，边缘越暗"), Range(0f, 1f)]
    public float intensity = 0f; // 匹配源码：FloatParameter

    [Tooltip("中心位置（x:0~1, y:0~1）：(0.5,0.5) 为屏幕中心")]
    public Vector2 center = new Vector2(0.5f, 0.5f); // 匹配源码：Vector2Parameter

    [Tooltip("形状过渡（0~1）：0=方形，1=圆形过渡"), Range(0f, 1f)]
    public float roundness = 1f; // 匹配源码：roundness（FloatParameter）

    [Tooltip("是否强制圆形（true=圆形，false=随屏幕比例变化）")]
    public bool rounded = false; // 匹配源码：rounded（BoolParameter）

    [Tooltip("暗角颜色")]
    public Color color = Color.black; // 匹配源码：ColorParameter
}