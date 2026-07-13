// Based on Unity's built-in Mobile/Bumped Specular shader.
// Adds a Main Color tint and preserves Gamma-style tint multiplication
// when the project uses Linear color space.

Shader "Custom/Mobile/Bumped Specular (Gamma Tint)" {
Properties {
    [MainColor] _Color ("Main Color", Color) = (1,1,1,1)
    [PowerSlider(5.0)] _Shininess ("Shininess", Range (0.03, 1)) = 0.078125
    [MainTexture] _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
    [NoScaleOffset] _BumpMap ("Normalmap", 2D) = "bump" {}
}
SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 250

CGPROGRAM
#pragma surface surf MobileBlinnPhong exclude_path:prepass nolightmap noforwardadd halfasview interpolateview

#include "UnityCG.cginc"

inline fixed4 LightingMobileBlinnPhong (SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
{
    fixed diff = max (0, dot (s.Normal, lightDir));
    fixed nh = max (0, dot (s.Normal, halfDir));
    fixed spec = pow (nh, s.Specular * 128) * s.Gloss;

    fixed4 c;
    c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
    UNITY_OPAQUE_ALPHA(c.a);
    return c;
}

sampler2D _MainTex;
sampler2D _BumpMap;
fixed4 _Color;
half _Shininess;

struct Input {
    float2 uv_MainTex;
};

inline half3 MultiplyMainColorGammaStyle(half3 textureColor, half3 mainColor)
{
#ifdef UNITY_COLORSPACE_GAMMA
    return textureColor * mainColor;
#else
    // Unity supplies both inputs in Linear space here. Convert them to Gamma,
    // multiply as a Gamma-space material would, then return to Linear space
    // before lighting and framebuffer output.
    half3 textureGamma = LinearToGammaSpace(textureColor);
    half3 colorGamma = LinearToGammaSpace(mainColor);
    return GammaToLinearSpace(textureGamma * colorGamma);
#endif
}

void surf (Input IN, inout SurfaceOutput o)
{
    fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
    o.Albedo = MultiplyMainColorGammaStyle(tex.rgb, _Color.rgb);
    o.Gloss = tex.a;
    o.Alpha = tex.a;
    o.Specular = _Shininess;
    o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
}
ENDCG
}

FallBack "Mobile/VertexLit"
}
