using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class VignetteBehaviour : PlayableBehaviour
{
    [Tooltip("是否启用暗角效果")]
    public bool enableVignette = true;

    [Tooltip("强度(0~1)：值越大，暗角越明显"), Range(0f, 1f)]
    public float intensity = 0f;

    [Tooltip("中心位置(x:0~1, y:0~1)：(0.5,0.5) 为屏幕中心")]
    public Vector2 center = new Vector2(0.5f, 0.5f);

    [Tooltip("圆角程度(0~1)：0=方形，1=纯圆形"), Range(0f, 1f)]
    public float roundness = 1f;

    [Tooltip("是否强制圆形：true=圆形，false=随屏幕比例变化")]
    public bool rounded = false;

    [Tooltip("暗角颜色")]
    public Color color = Color.black;
}