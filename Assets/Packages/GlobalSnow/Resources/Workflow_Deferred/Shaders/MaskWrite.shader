Shader "GlobalSnow/DeferredMaskWrite" {

SubShader {

	Tags { "DisableBatching" = "True" }

	CGINCLUDE
	#include "UnityCG.cginc"
    #include "GlobalSnowDeferredOptions.cginc"
    
    struct appdata {
    	float4 vertex : POSITION;
        float4 color  : COLOR;
    };

	struct v2f {
	    float4 pos : SV_POSITION;
	};
	
	v2f vert(appdata v) {
    	v2f o;
        APPLY_VERTEX_MODIFIER(v)
    	o.pos = UnityObjectToClipPos(v.vertex);
		return o;
	}	
	
	fixed4 frag(v2f i): SV_Target {
		return fixed4(0,0,0,0);
	}	
	
	
	ENDCG

	Pass { 
       	Fog { Mode Off }
       	ColorMask 0
		CGPROGRAM
		#pragma target 3.0
   		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma vertex vert
		#pragma fragment frag
		ENDCG
	}
	
}

Fallback Off
}
