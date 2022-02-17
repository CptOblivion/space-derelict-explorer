Shader "Unlit/UI_Path"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    _ScrollSpeed("Scroll Speed", Float) = 1
        _SegmentCount("Segment Count", Float) = 1
        _SegmentColor("Segment Color", Color) = (0.0, 0.0, 0.0, 1.0)
        _TimeOffset("Time Offset", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half _ScrollSpeed;
            half _SegmentCount;
            float4 _SegmentColor;
            half _TimeOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //remember to always leave in the default comments
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= i.color;

                //there's probably a better way to write this so negative scrollspeeds just run backwards (instead of producing negative results)
                //I tried using abs() but there's a weird delay for the first cycle or two (I guess because the negative speed has to walk seg down to 0 first before it can behave normally or something?)
                //normally that wouldn't really be an issue after the first second or so after scene load, but in this house counteract _Time with _TimeOffset
                float seg = ((1-i.uv.x + (_Time.y - _TimeOffset) * _ScrollSpeed)  * _SegmentCount) % 1;

                //for some reason, the whole invert, multiply, and invert back thing keeps tripping me up
                _SegmentColor.rgb = 1 - ((1 - _SegmentColor.rgb) * _SegmentColor.a * seg);

                col.rgb *= _SegmentColor.rgb;

                col.rgb *= col.a; //there's probably alread a premultiply tickbox somewhere, but eh
                return col;
            }
            ENDCG
        }
    }
}
