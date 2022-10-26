Shader "FX/Mirror Reflection" { 
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _ReflectionTex ("Reflection", 2D) = "white" { TexGen ObjectLinear }
    _ReflectionColor ("Reflection Color", Color) = (1.0, 1.0, 1.0, 1.0)
    _SpecularPower("Specular Power", Range(0.0, 32.0)) = 2.0
}

SubShader {

	CGPROGRAM
	#pragma surface customSurfShader ColoredSpecular

	struct CustomSurfaceOutput {
	    half3 Albedo;
	    half3 Normal;
	    half3 Emission;
	    half Specular;
	    half3 GlossColor;
	    half Alpha;
	};

	float _SpecularPower;
	
	inline half4 LightingColoredSpecular (CustomSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
	{
	  half3 h = normalize (lightDir + viewDir);
	  half diff = max (0, dot (s.Normal, lightDir));
	  float nh = max (0, dot (s.Normal, h));
	  float spec = pow (nh, _SpecularPower);
	  half3 specCol = spec * s.GlossColor;
	 
	  half4 c;
	  c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * specCol) * (atten * 2);
	  c.a = s.Alpha;
	  return c;
	}
	 
	inline half4 LightingColoredSpecular_PrePass (CustomSurfaceOutput s, half4 light)
	{
	    half3 spec = light.a * s.GlossColor;
	   
	    half4 c;
	    c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
	    c.a = s.Alpha + dot(spec,spec) * _SpecColor.a;
	    return c;
	}
	 	
	struct Input {
	  float3 worldPos;
	  float2 uv_MainTex;
	};

	sampler2D _MainTex;
	sampler2D _ReflectionTex;
	float4 _ReflectionColor;
	float4x4 _ProjMatrix;
	  
	void customSurfShader (Input IN, inout CustomSurfaceOutput o) {

		float4 reflUV = mul(_ProjMatrix, float4(IN.worldPos,1));
		float3 reflCol = tex2Dproj(_ReflectionTex, reflUV);
		o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
		o.GlossColor = reflCol.rgb*_ReflectionColor.rgb;
	}
	
	ENDCG
} // SubShader

	Fallback "Specular"
} // "FX/Mirror Reflection" 
