Shader "Hidden/UI_Effects"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurWidth ("Smear Iterations", Int) = 8
        _BlurOffset ("Smear Offset", Float) = 1
        _ScanlineCount1 ("Scanline Count 1", Float) = 500
        _ScanlineCount2 ("Scanline Count 2", Float) = 1500
        _ScanlineStrength ("Scanline Strength", Float) = .04
        //TODO: rework _ScanlinePulse implementation to actually make sense
        _ScanlinePulse ("Scanline Effects", Vector) = (.0003, .0002, .0002, .0001) //Strength Base, Strength Flicker, Stretch Base, Stretch Flicker
        _WobbleLineCount ("Wobble Line Count", Float) = 80
        _WobbleFrequencies ("Wobble Wave Frequencies", Vector) = (2, 2.3, 10.76, 0)
        _WobbleMagnitudes ("Wobble Wave Magnitudes", Vector) = (1, 0.4, 0.2, 0)
        _WobbleColors ("Wobble Color Separation", Vector) = (1, .2, .1, 0)
        _FlickerStrength ("Flicker Strength", Float) = 0.02
        _FlickerFrequency ("Flicker Frequency", Float) = 60
        _CursorY ("Cursor Y", Vector) = (0, 0, 0, 0) //cursor y, cursor height, cursor smear peak, cursor smear slope

        
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
        Tags{"Queue" = "Transparent"}
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            int _BlurWidth;
            half _BlurOffset;
            half _ScanlineCount1;
            half _ScanlineCount2;
            half _ScanlineStrength;
            half4 _ScanlinePulse;
            half _WobbleLineCount;
            half3 _WobbleFrequencies;
            half3 _WobbleMagnitudes;
            half3 _WobbleColors;
            half _FlickerStrength;
            half _FlickerFrequency;
            half4 _CursorY;

            fixed4 frag(v2f i) : SV_Target
            {
            float2 uv = i.uv;
            float pix = 1. / _ScreenParams.x;
            _WobbleMagnitudes /= (_WobbleMagnitudes.x * _WobbleMagnitudes.x + _WobbleMagnitudes.y * _WobbleMagnitudes.y + _WobbleMagnitudes.z * _WobbleMagnitudes.z); //I always forget to normalize
            float pulse = sin(_Time.y * _WobbleFrequencies.x) * _WobbleMagnitudes.x + sin(_Time.y * _WobbleFrequencies.y) * _WobbleMagnitudes.y + sin(_Time.y * _WobbleFrequencies.z) * _WobbleMagnitudes.z;

            float scanline = sin(uv.y * _ScanlineCount1 + _Time.w) * (_ScanlinePulse.x + pulse * _ScanlinePulse.y) + sin(uv.y * _ScanlineCount2 + _Time.z);
            uv.x += scanline * (_ScanlinePulse.z + pulse * _ScanlinePulse.w);
            fixed4 col = tex2Dlod(_MainTex, float4(uv, 1, pulse*_ScreenParams.x/1500))*.5;
            col *= (1 - _ScanlineStrength + scanline * _ScanlineStrength);

            pulse = max(pulse, 0) + (sin(_Time.y * _FlickerFrequency) + 1) * _FlickerStrength;
            float3 sAmp = _WobbleColors * pulse;

            float cursorOffset = smoothstep(0, 1, 1 - abs((2 * (uv.y - _CursorY.x)) / _CursorY.y)) * _CursorY.z;

            float sSpeed = 0.8;
            for (float b = 0; b < _BlurWidth; b++) {
                float bStr = 1-(b / _BlurWidth) + cursorOffset;
                float3 bDist = sAmp * b * pix * _BlurOffset;
                float scanline = (sin(uv.y * _WobbleLineCount + _Time.y * sSpeed) + 1);
                float3 blur = bDist * scanline + cursorOffset * bDist;
                float4 colr = tex2Dlod(_MainTex, float4(uv + float2(blur.x, 0), 1, pulse * 2));
                float4 colg = tex2Dlod(_MainTex, float4(uv + float2(blur.y, 0), 1, pulse * 2));
                float4 colb = tex2Dlod(_MainTex, float4(uv + float2(blur.z, 0), 1, pulse * 2));
                col += float4(colr.r, colg.g, colb.b, (colr.a * colr.r + colg.a * colg.g + colb.a * colb.b) / 3) * bStr * 2 / _BlurWidth *(.1 + pulse * .8);
            }

            return col;
            }
            ENDCG
        }
    }
}



