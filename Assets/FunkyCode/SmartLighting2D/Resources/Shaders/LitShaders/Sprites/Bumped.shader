﻿Shader "Light2D/Sprites/Bumped"
{
	Properties
	{
		[HideInInspector] _MainTex ("Sprite Texture", 2D) = "white" {}
        _NormTex ("Normal Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_Lit ("Lit", Range(0,1)) = 1

        _NormalSize ("Normal Range", Range(0.001, 0.2)) = 1
        _NormalDepth("Normal Depth", Range(0.5, 5)) = 1
		// _LinearColor ("LinearColor", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass {

		CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
				float4 color    : COLOR;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float2 texcoord  : TEXCOORD1;
				fixed4 color    : COLOR;
                float2 worldPos : TEXCOORD0;
			};
			
			fixed4 _Color;
			float _Lit;
	
			sampler2D _MainTex;
            sampler2D _NormTex;

			// float _LinearColor;
			
			// Cam 1
			sampler2D _Cam1_Texture;
            vector _Cam1_Rect;
            float _Cam1_Rotation;

			// Cam 2
			sampler2D _Cam2_Texture;
			vector _Cam2_Rect;
            float _Cam2_Rotation;


            float _NormalSize;
            float _NormalDepth;

			v2f vert(appdata_t IN)
			{
				v2f OUT;

				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
                
                OUT.worldPos = mul (unity_ObjectToWorld, IN.vertex);

				return OUT;
			}

            bool InCamera (float2 pos, float2 rectPos, float2 rectSize) {
				rectPos -= rectSize / 2;
                return (pos.x < rectPos.x || pos.x > rectPos.x + rectSize.x || pos.y < rectPos.y || pos.y > rectPos.y + rectSize.y) == false;
            }

			float2 TransformToCamera(float2 pos, float rotation) {
				float c = cos(-rotation);
				float s = sin(-rotation);
		
				float x = pos.x;
				float y = pos.y;

				pos.x = x * c - y * s;
				pos.y = x * s + y * c;

                return(pos);
            }

			fixed4 LightColor(float id, float2 texcoord) {
				if (id < 0.5f) {
					return(tex2D (_Cam1_Texture, texcoord));
				} else if (id < 1.5f) {
					return(tex2D (_Cam2_Texture, texcoord));
				}
				
				return(fixed4(0, 0, 0, 0));
			}

			fixed4 LightColorBump(float id, float2 texcoord) {
				if (id < 0.5f) {
					fixed4 color = tex2D (_Cam1_Texture, texcoord);
					return((color.r + color.g + color.b) / 3);
				} else if (id < 1.5f) {
					return(tex2D (_Cam2_Texture, texcoord));
				}
				
				return(fixed4(0, 0, 0, 0));
			}

			fixed4 InputColor(v2f IN) {
				return(tex2D (_MainTex, IN.texcoord) * IN.color);
			}
			

			fixed4 OutputColor(v2f IN, float2 posInCamera, float2 cameraSize, float id) {
				float2 texcoord = (posInCamera + cameraSize / 2) / cameraSize;
			
				fixed4 lightPixel = LightColor(id, texcoord);

                float delta = _NormalSize;

                fixed lightPixelLeft = LightColorBump(0, texcoord + float2(-delta, 0));
                fixed lightPixelRight = LightColorBump(0, texcoord + float2(delta, 0));

                fixed lightPixelUp = LightColorBump(0, texcoord + float2(0, delta));
                fixed lightPixelDown = LightColorBump(0, texcoord + float2(0, -delta));
                
                fixed lightHorizontal = (lightPixelRight - lightPixelLeft) * _NormalDepth;
                fixed lightVertical = (lightPixelUp - lightPixelDown) * _NormalDepth;




                float3 normalDirection = (tex2D(_NormTex, IN.texcoord).xyz - 0.5f) * 2.0f;
                normalDirection = float3(mul(float4(normalDirection.xyz, 1.0f), unity_WorldToObject).xyz);
                normalDirection.z *= -1;
                normalDirection = normalize(normalDirection);






                float3 lightDirection = normalize(float3(lightHorizontal, lightVertical, -1));


                float normalDotLight = dot(normalDirection, lightDirection);


                
              	lightPixel.r *= normalDotLight;
              	lightPixel.g *= normalDotLight;
               	lightPixel.b *= normalDotLight;

				lightPixel = lerp(lightPixel, fixed4(1, 1, 1, 1), 1 - _Lit);

                


				fixed4 spritePixel = InputColor(IN);

                spritePixel = tex2D(_MainTex, IN.texcoord);
           



				spritePixel = spritePixel * lightPixel;

				spritePixel.rgb *= spritePixel.a; 
				
				//spritePixel.rgb =  pow(spritePixel.rgb, 1/2.2);
				
				return spritePixel;
			}
        
			fixed4 frag(v2f IN) : SV_Target
			{
                if (_Cam1_Rect.z > 0) {
					float2 camera_1_Size = float2(_Cam1_Rect.z, _Cam1_Rect.w);
					float2 posInCamera1 = TransformToCamera(IN.worldPos - float2(_Cam1_Rect.x, _Cam1_Rect.y),  _Cam1_Rotation);

					if (InCamera(posInCamera1, float2(0, 0), camera_1_Size)) {
                    	return OutputColor(IN, posInCamera1, camera_1_Size, 0);
					}
				}

				if (_Cam2_Rect.z > 0) {
					float2 camera_2_Size = float2(_Cam2_Rect.z, _Cam2_Rect.w);
					float2 posInCamera2 = TransformToCamera(IN.worldPos - float2(_Cam2_Rect.x, _Cam2_Rect.y),  _Cam2_Rotation);
					
					if (InCamera(posInCamera2, float2(0, 0), camera_2_Size)) {
						return OutputColor(IN, posInCamera2, camera_2_Size, 1);
					}
                }

				fixed4 spritePixel = InputColor(IN);
				spritePixel.rgb *= spritePixel.a; 
				return spritePixel;
			}

		    ENDCG
		}
	}
}