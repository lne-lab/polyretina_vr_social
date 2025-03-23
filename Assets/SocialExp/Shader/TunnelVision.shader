Shader "Hidden/TunnelVision"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GazePosition ("Gaze Position", Vector) = (0.5, 0.5, 0, 0)
        _FOV ("Field of View", Float) = 20.0
    }
    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            // Declare the properties
            uniform sampler2D _MainTex;
            uniform float4 _GazePosition;
            uniform float _FOV;

            float4 frag(v2f_img i) : SV_Target
            {
                // Subtract the gaze position from the UV coordinates to center the effect
                float2 uv = i.uv - _GazePosition.xy;
                float dist = length(uv);
                float circleRadius = tan(radians(_FOV) / 4.0);
                
                // If within the FOV circle, render the texture, otherwise black out
                if (dist < circleRadius)
                    return tex2D(_MainTex, i.uv);
                else
                    return float4(0, 0, 0, 1); // Black out the area outside the FOV
            }
            ENDCG
        }
    }
    FallBack Off
}
