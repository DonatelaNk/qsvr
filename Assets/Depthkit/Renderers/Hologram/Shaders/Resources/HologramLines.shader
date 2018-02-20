/************************************************************************************

DepthKit Unity SDK License v1
Copyright 2016-2018 Simile Inc. All Rights reserved.  

Licensed under the Simile Inc. Software Development Kit License Agreement (the "License"); 
you may not use this SDK except in compliance with the License, 
which is provided at the time of installation or download, 
or which otherwise accompanies this software in either electronic or hard copy form.  

You may obtain a copy of the License at http://www.depthkit.tv/license-agreement-v1

Unless required by applicable law or agreed to in writing, 
the SDK distributed under the License is distributed on an "AS IS" BASIS, 
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
See the License for the specific language governing permissions and limitations under the License. 

************************************************************************************/

Shader "DepthKit/HologramLines"
{
    Properties
    {
        _Sprite 	("Texture", 2D) = "white" {}
        _UseSprite 	("Use Sprite", Float) = 0.0
        _Width 		("Width", Float) = 1.0
        _Opacity 	("Opacity", Range(0,1)) = 1.0

        //NOTE: These are set per material, not per layer, as they cannot be controlled by the MaterialPropertyBlock
        _SrcMode	  ("Blend Src Mode", Float) = 0.0
        _DstMode	  ("Blend Dst Mode", Float) = 0.0
        _ZTest ("Depth Compare Mode", Int) = 0

        _BlendEnum("Blend Enum", Float) = 0.0
    }

    SubShader
    {
        // All hologram shaders are rendered in the transparency pass, with no shadowing
        Tags { "Queue"="Transparent+1" "RenderType"="Transparent" "IgnoreProjector"="True" "ForceNoShadowCasting" ="True" }
        LOD 100

        Blend [_SrcMode] [_DstMode]
        ZWrite Off
        Cull Off
        ZTest [_ZTest]  // self occlusion if ztest is set to LEQUAL
        Offset -128, -1 // large bias to camera to ensure our lines do not fail depth test when co-planar with depth

        Pass
        {
            CGPROGRAM

            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "../../../Resources/DepthKit.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2g
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                DEPTHKIT_TEX_COORDS(1, 2, 3)    // reserve texcoords/interpolants 1-3 for depthkit use
                UNITY_FOG_COORDS(4)	            // if Unity distance fogging enabled
            };

            sampler2D _Sprite;
            float4 _Sprite_ST;

            //Creates the necessary declarations for DK textures
            DEPTHKIT_TEX_ST

            float _UseSprite;
            float _Width;
            float _Opacity;
            float _BlendEnum;

            v2g vert (appdata v)
            {
                v2g o;
                UNITY_INITIALIZE_OUTPUT(v2g,o);

                float2 colorTexCoord;
                float2 depthTexCoord;
                float4 vertOut;

                dkVertexPass(v.vertex, colorTexCoord, depthTexCoord, vertOut, o.valid);

                o.vertex = mul(unity_ObjectToWorld, vertOut);
                o.uv = v.uv;
                o.uv_MainTex   = colorTexCoord;
                o.uv2_MainTex2 = depthTexCoord;

                UNITY_TRANSFER_FOG(o, vertOut);

                return o;
            }

            [maxvertexcount(4)]
            void geom(line v2g input[2], inout TriangleStream<v2g> OutputStream)
            {
                // remove connector lines between line segments in linestrip
                if(input[0].uv.x == 1.0 && input[1].uv.x == 1.0){
                    return;
                }

                v2g output;
                UNITY_INITIALIZE_OUTPUT(v2g, output);

                // get the worldspace direction down the line
                float3 lineLook = input[1].vertex.xyz - input[0].vertex.xyz;

                // output a stretched quad as the line segment, 2 x 2 verts, orientated towards the camera
                for(int i = 0; i < 2; i++)
                {
                    //direction from camera to this object
                    float3 cameraLook = _WorldSpaceCameraPos.xyz - input[i].vertex.xyz;

                    //billboard direction is orthogonal to both line & camera
                    float3 up = normalize(cross(cameraLook, lineLook));

                    output.valid = input[i].valid;

                    float4 vert0 = float4(input[i].vertex.xyz + (up * _Width), 1.0);
                    output.vertex = mul(UNITY_MATRIX_VP, vert0);
                    output.uv = float2(i == 0 ? 1.0 : 0.0, 1.0);
                    output.uv_MainTex   = input[i].uv_MainTex;
                    output.uv2_MainTex2 = input[i].uv2_MainTex2;

                    OutputStream.Append(output);

                    float4 vert1 = float4(input[i].vertex.xyz - (up * _Width), 1.0);
                    output.vertex = mul(UNITY_MATRIX_VP, vert1);
                    output.uv = float2(i == 0 ? 1.0 : 0.0, 0.0);
                    output.uv_MainTex   = input[i].uv_MainTex;
                    output.uv2_MainTex2 = input[i].uv2_MainTex2;

                    OutputStream.Append(output);
                }
            }

            fixed4 frag (v2g i) : SV_Target
            {
                float4 dkColor;
                dkFragmentPass(i.uv2_MainTex2, i.uv_MainTex, dkColor, i.valid);

                clip(i.valid > 0.0 ? 0 : -1);

                // sample the texture
                float4 spriteCol = lerp(fixed4(1.0,1.0,1.0,1.0), tex2D(_Sprite, i.uv), _UseSprite);

                float4 finalCol;
                finalCol.rgb = spriteCol.rgb * dkColor.rgb;
                finalCol.a = 1.0;

                // note; BLEND_MULTIPLY has a known issue with respecting transparency on sprites.
                if (_BlendEnum == BLEND_ALPHA || _BlendEnum == BLEND_MULTIPLY)
                {
                    finalCol.a *= spriteCol.a * _Opacity;
                }
                else if (_BlendEnum == BLEND_ADD || _BlendEnum == BLEND_SCREEN)
                {
                    finalCol.rgb *= spriteCol.a * _Opacity;
                }

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, finalCol);

                return finalCol;
            }
            ENDCG
        }
    }
}
