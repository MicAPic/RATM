Shader "Custom/Matrix Wipe"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_FadeTex("Fade from Texture", 2D) = "white" {}
		_Progress ("Progress", Range(0.0, 1.0)) = 1.0
		_Size ("Size", Vector) = (10.0, 10.0, 0, 0)
		_Smoothness ("Smoothness", float) = 0.5
	}

	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Lighting Off

		Pass
		{
		    CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"
		    
			uniform sampler2D _MainTex;
			uniform sampler2D _FadeTex;
			uniform half _Progress;
			uniform float2 _Size;
			uniform float _Smoothness;
		    
			float rand(float2 uv)
			{
				float2 noise = frac(sin(dot(uv, float2(12.9898, 78.233) * 2.0)) * 43758.5453);
				return abs(noise.x + noise.y) * 0.5;
			}
			
			fixed4 frag(v2f_img i) : COLOR
			{
				float tile = rand(floor(_Size.xy * i.uv));
				float step = smoothstep(0.0,
									 -_Smoothness,
									 tile - _Progress * (1.0 + _Smoothness));
			
				return lerp(tex2D(_MainTex, i.uv), tex2D(_FadeTex, i.uv), step);
			}
			
			ENDCG
		}
	}

	FallBack "Diffuse"
}