Shader "FullScreen/FullscreenPass_Stars"
{
    HLSLINCLUDE

    #pragma vertex Vert

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone vulkan metal switch

    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"

    // The PositionInputs struct allow you to retrieve a lot of useful information for your fullScreenShader:
    // struct PositionInputs
    // {
    //     float3 positionWS;  // World space position (could be camera-relative)
    //     float2 positionNDC; // Normalized screen coordinates within the viewport    : [0, 1) (with the half-pixel offset)
    //     uint2  positionSS;  // Screen space pixel coordinates                       : [0, NumPixels)
    //     uint2  tileCoord;   // Screen tile coordinates                              : [0, NumTiles)
    //     float  deviceDepth; // Depth from the depth buffer                          : [0, 1] (typically reversed)
    //     float  linearDepth; // View space Z coordinate                              : [Near, Far]
    // };

    // To sample custom buffers, you have access to these functions:
    // But be careful, on most platforms you can't sample to the bound color buffer. It means that you
    // can't use the SampleCustomColor when the pass color buffer is set to custom (and same for camera the buffer).
    // float4 SampleCustomColor(float2 uv);
    // float4 LoadCustomColor(uint2 pixelCoords);
    // float LoadCustomDepth(uint2 pixelCoords);
    // float SampleCustomDepth(float2 uv);

    // There are also a lot of utility function you can use inside Common.hlsl and Color.hlsl,
    // you can check them out in the source code of the core SRP package.

    float noise(in float seed) {
        //return((sin(seed * 7) + 1) * .5);
        return((sin(seed * 7) * sin(seed * 15.15) + 1) * .5);
    }
    float4 FullScreenPass(Varyings varyings) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(varyings);
        float depth = LoadCameraDepth(varyings.positionCS.xy);
        PositionInputs posInput = GetPositionInput(varyings.positionCS.xy, _ScreenSize.zw, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
        float3 viewDirection = GetWorldSpaceNormalizeViewDir(posInput.positionWS);
        float4 color = float4(0.0, 0.0, 0.0, 0.0);

        // Load the camera color buffer at the mip 0 if we're not at the before rendering injection point
        if (_CustomPassInjectionPoint != CUSTOMPASSINJECTIONPOINT_BEFORE_RENDERING)
            color = float4(CustomPassLoadCameraColor(varyings.positionCS.xy, 0), 1);

        //float _CellSize = 0.1;

        //float2 uv = viewDirection.xy % _CellSize;
        float _StarSize = .05;
        float _StarDepth = 4;
        float2 gridsize = float2(11,23);

        float2 uv = float2(atan(viewDirection.x/viewDirection.z) / 3.14, acos(viewDirection.y) / 3.14);
        uv.y -= .5;
        uv *= gridsize;
        float band = abs(uv.y);
        band = (floor(band+.5)) / gridsize.y;
        uv.x = uv.x * 2 - band;
        uv.y -= .5 ;
        float2 cell = floor(uv);
        uv = abs(uv % 1 * 2) - 1;
        uv += float2(noise(cell.x*cell.y)*.8,noise(cell.x*1.4+cell.y*1.4)*.8);
        float star = max(0,1 - length(uv));
        star = smoothstep(.8, 1, star);
        color.rgb += float3(noise(cell.x*cell.y),noise(cell.x*2+cell.y*2),noise(cell.x * .5 + cell.y*.5))*star;
        //float stars = max(0, noise(viewDirection.x / _StarSize) * noise(viewDirection.y / _StarSize) * noise(viewDirection.z / _StarSize) - (1.0 - 1.0 / _StarDepth)) * _StarDepth ;
        //color.rgb = stars;

        // Add your custom pass code here

        // Fade value allow you to increase the strength of the effect while the camera gets closer to the custom pass volume
        float f = 1 - abs(_FadeValue * 2 - 1);
        return float4(color.rgb + f, color.a);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "Custom Pass 0"

            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            HLSLPROGRAM
                #pragma fragment FullScreenPass
            ENDHLSL
        }
    }
    Fallback Off
}
