//http://qiita.com/kaiware007/items/1d9f37bc8c5892d254ac
Shader "Custom/Dissolve" {

    Properties{
        _MainColor("Main Color", Color) = (1,1,1,1) 
        _MainTex("Base (RGB)", 2D) = "white" {}     
        _Mask("Mask To Dissolve", 2D) = "white" {}  
        _CutOff("CutOff Range", Range(0,1)) = 0     
        _Width("Width", Range(0,1)) = 0.001         
        _ColorIntensity("Intensity", Float) = 1     
        _Color("Line Color", Color) = (1,1,1,1)     
        _BumpMap("Normalmap", 2D) = "bump" {}       
    }

    SubShader{

        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        LOD 300

        Pass{
            ZWrite On
            ColorMask 0
        }

        CGPROGRAM
        #pragma target 2.0
        #include "UnityCG.cginc"
        #pragma surface surf Lambert alpha 

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _Mask;
        fixed4 _Color;
        fixed4 _MainColor;
        fixed _CutOff;
        fixed _Width;
        fixed _ColorIntensity;

        struct Input {
            float2 uv_MainTex;
            float2 uv_BumpMap;
        };

        void surf(Input IN, inout SurfaceOutput o) {
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex) * _MainColor;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

            fixed a = tex2D(_Mask, IN.uv_MainTex).r;

            fixed b = smoothstep(_CutOff, _CutOff + _Width, a);
            o.Emission = _Color * b * _ColorIntensity;

            fixed b2 = step(a, _CutOff + _Width * 2.0);
            o.Alpha = b2;
        }
        ENDCG
    }
    Fallback "Diffuse"
}