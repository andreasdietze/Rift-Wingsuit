Shader "Rift-Suit/Water/Quelle_2" {
	Properties {
		_SunPositionX("Sonne: X-Position", Float) = 3200
		_SunPositionZ("Sonne: Z-Position", Float) = 3200
		_WaterSpeed ("Wassergeschwindigkeit", Range (0.01, 1)) = 1
		_SpecColor ("Lichtfarbe", Color) = (0.5,0.5,0.5,1)
		_Shininess ("Lichthelligkeit", Range (0.01, 0.95)) = 0.078125
		_DepthColor ("Wasserfarbe (nah)", Color) = (0.01,0.02,0.02,0.8)
		_ReflectColor ("Wasserfarbe (fern)", Color) = (1,1,1,0.5)
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_WaterReflex("Wasser-Reflexionsstärke", Float) = 2.0
		_WaterLumi("Wasser Helligkeit", Range(0.01, 2)) = 1.0
		_Opaque("Sichtdurchlässigkeit", Range(0.00,1.00)) = 0.95
	}

	SubShader 
	{	
		
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True"}
		LOD 100
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha 
		Offset -1, -1

		GrabPass 
		{ 
			
		}
		
		CGPROGRAM

		#pragma surface surf BlinnPhong alpha
		#pragma target 3.0

		sampler2D _BumpMap;
		float4 _GrabTexture_TexelSize;

		float _SunPositionX;
		float _SunPositionZ;
		float _WaterSpeed;
		float4 _ReflectColor;
		float4 _DepthColor;
		float _Shininess;
		float _WaterReflex;
		float _WaterLumi;
		float _Rnd = 0.50;
		float _Opaque;

		struct Input {
			float2 uv_BumpMap;
			float4 screenPos;
			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{			
			//Specular
			o.Gloss = _SpecColor.a;
			o.Specular = (1.0 - _Shininess);
			
			//Calc the normal based on the water speed + bump map
			float waterSpeed = _WaterSpeed * 175;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap - waterSpeed * _Time.y * _GrabTexture_TexelSize.xy ));
			o.Normal += UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap - (waterSpeed*3) * _Time.y * _GrabTexture_TexelSize.xy));
			o.Normal *= 0.5;
			
			//Seethrough
			o.Alpha = _Opaque;

			//Sonnenposition
			IN.screenPos.xy = (_SunPositionX, _SunPositionZ);	
			
			half4 reflectionCol = (0.75,0.75,1,1) * _ReflectColor;
			
			float4 depthCol = _DepthColor;
			
			//Fresnel realisation
			half fresnel = saturate( 1.0 - dot(o.Normal, normalize(IN.viewDir)) );
			fresnel = pow(fresnel, _WaterReflex);
			fresnel =  _Rnd + (1.0 - _Rnd) * fresnel;
			
			//Calculate emision color based on the init-color + fresnel
			half4 emissionCol = reflectionCol * fresnel + half4(depthCol.xyz, 1.0) * (1.0 - fresnel);	
			o.Emission = emissionCol.xyz * 0.8;
			
			//Reflection 
			o.Albedo = emissionCol * _WaterLumi;
		}
		ENDCG
	}
}