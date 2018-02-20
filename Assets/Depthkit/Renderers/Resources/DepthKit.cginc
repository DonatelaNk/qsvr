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

// blend modes reflected from user script code
#define BLEND_ALPHA 0
#define BLEND_ADD 1
#define BLEND_MULTIPLY 2
#define BLEND_SCREEN 3

//INCLUDE THIS IN YOUR INPUT STRUCT
#define DEPTHKIT_TEX_COORDS(idx, idx2, idx3) \
	float2 uv_MainTex : TEXCOORD##idx; \
	float2 uv2_MainTex2 : TEXCOORD##idx2; \
	float valid : TEXCOORD##idx3;

//INCLUDE THIS IF YOU ARE NOT USING A SURFACE SHADER
#define DEPTHKIT_TEX_ST float4 _MainTex_ST; \
			float4 _MainTex2_ST;

static const float PI = 3.14159265f;
// float epsilon = .03;
float _Epsilon = .03;
float _DepthSaturationThreshhold = 0.5; //a given pixel whose saturation is less than half will be culled (old default was .5)
float _DepthBrightnessThreshold = 0.5; //a given pixel whose brightness is less than half will be culled (old default was .9)
float _InternalEdgeThreshold = 0.0; 

//PROPERTIES-- feel free to copy this into your shader, but not necessary

//		//Main texture is the combined color and depth video frame
//		_MainTex ("Texture", 2D) = "white" {}
//		_MainTex2 ("Texture", 2D) = "white" {} //we currently set the same texture twice due to a bug in unity to pass multiple texture coordinates
//		//Size of the actual texture being passed in
//		_TextureDimensions ("Texture Dimension", Vector) = (0, 0, 0, 0)
//		//Crop factor that shows where from the original depth frame the texture is sampling
//		_Crop ("Crop", Vector) = (0,0,0,0)
//		//Original depth frame image dimensions
//		_ImageDimensions ("Image Dimensions", Vector) = (0,0,0,0)
//		//Focal length X/Y in terms of pixels from the original depth image (_ImageDimensions)
//		_FocalLength ("Focal Length", Vector) = (0,0,0,0)
//		//Principal Point in terms of pixels from the original depth image (_ImageDimensions)
//		_PrincipalPoint ("Principal Point", Vector) = (0,0,0,0)
//		//Near and Far bounds of depth data range for this frame
//		_NearClip ("Near Clip", Float) = 0.0
//		_FarClip  ("Far Clip", Float) = 0.0
//		//Number of vertices (x/y) in textured mesh
//		_MeshDensity ("Mesh Density", Range(0,255)) = 128

//All DepthKit params
sampler2D _MainTex;
sampler2D _MainTex2;
float4 _MainTex_TexelSize;
float4 _MainTex2_TexelSize;

//float2 _TextureDimensions;
float4 _Crop;
float2 _ImageDimensions;
float2 _FocalLength;
float2 _PrincipalPoint;
float _NearClip;
float _FarClip;
float4x4 _Extrinsics;
float _MeshDensity;
int _TextureFlipped;
int _LinearColorSpace;
int _ExternalTexture;

fixed3 rgb2hsv(fixed3 c)
{
    fixed4 K = fixed4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    fixed4 p = lerp(fixed4(c.bg, K.wz), fixed4(c.gb, K.xy), step(c.b, c.g));
    fixed4 q = lerp(fixed4(p.xyw, c.r), fixed4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    return fixed3(abs(q.z + (q.w - q.y) / (6.0 * d + _Epsilon)), d / (q.x + _Epsilon), q.x);

}

float depthForPoint(float2 texturePoint)
{
    float4 textureSample = float4(texturePoint.x, texturePoint.y, 0.0, 0.0);
    // if Unity is rendering in Linear space and we are using unity video player (which must encode frames in gamma space), we apply inverse gamma (1/2.2) to depth frame to get a linear sample
    fixed4 depthsample = pow(tex2Dlod(_MainTex, textureSample), _LinearColorSpace == 1 && _ExternalTexture == 0 ? 0.4545 : 1.0);
    fixed3 depthsamplehsv = rgb2hsv(depthsample.rgb);

    return depthsamplehsv.g > _DepthSaturationThreshhold && depthsamplehsv.b > _DepthBrightnessThreshold ? depthsamplehsv.r : 0.0;
}

//Used in the vertex shader pass to extract texture coordinates and spatial position
//from the input texture
void dkVertexPass(float4 vertIn, inout float2 colorTexCoord, inout float2 depthTexCoord, inout float4 vertOut, inout float valid)
{
    float2 centerpix = _MainTex_TexelSize.xy * .5;
    float2 textureStep = float2(1.0 / (_MeshDensity - 1.0), 1.0 / (_MeshDensity - 1.0));
    float2 basetex = floor(vertIn.xy * _MainTex_TexelSize.zw) * _MainTex_TexelSize.xy;
    float2 imageCoordinates = _Crop.xy + (basetex * _Crop.zw);

    //flip texture
    if (_TextureFlipped == 1)
    {
        basetex.y = 1.0 - basetex.y;
        depthTexCoord = basetex * float2(1.0, 0.5) + float2(0.0, 0.5) + centerpix;
        colorTexCoord = basetex * float2(1.0, 0.5) + centerpix;
    }
    else
    {
        depthTexCoord = basetex * float2(1.0, 0.5) + centerpix;
        colorTexCoord = basetex * float2(1.0, 0.5) + float2(0.0, 0.5) + centerpix;
    }

    //check neighbors
    float2 neighbors[8] = {
        float2(0.,  textureStep.y),
        float2(textureStep.x,	 		   0.),
        float2(0., -textureStep.y),
        float2(-textureStep.x,			   0.),
        float2(-textureStep.x, -textureStep.y),
        float2(textureStep.x,  textureStep.y),
        float2(textureStep.x, -textureStep.y),
        float2(-textureStep.x,  textureStep.y)
    };

    //texture coords come in as [0.0 - 1.0] for this whole plane
    float depth = depthForPoint(depthTexCoord);

    int i;
    float neighborDepths[8];
    for (i = 0; i < 8; i++)
    {
        neighborDepths[i] = depthForPoint(depthTexCoord + neighbors[i]);
    }

    valid = 1.0;
    int numDudNeighbors = 0;
    //search neighbor verts in order to see if we are near an edge
    //if so, clamp to the surface closest to us
    if (depth < _Epsilon || (1.0 - depth) < _Epsilon)
    {
        float depthDif = 1.0f;
        float nearestDepth = depth;
        for (int i = 0; i < 8; i++)
        {
            float depthNeighbor = neighborDepths[i];
            if (depthNeighbor >= _Epsilon && (1.0 - depthNeighbor) > _Epsilon)
            {
                float thisDif = abs(nearestDepth - depthNeighbor);
                if (thisDif < depthDif)
                {
                    depthDif = thisDif;
                    nearestDepth = depthNeighbor;
                }
            }
            else
            {
                numDudNeighbors++;
            }
        }

        depth = nearestDepth;
        // check validity of final depth
        if (depth < _Epsilon || (1.0 - depth) < _Epsilon)
        {
            valid = 0.0;
        }
        // blob filter
        if (numDudNeighbors > 6)
        {
            valid = 0.0;
        }
    }

    // internal edge filter
    float maxDisparity = 0.0;
    for (i = 0; i < 8; i++)
    {
        float depthNeighbor = neighborDepths[i];
        if (depthNeighbor >= _Epsilon && (1.0 - depthNeighbor) > _Epsilon)
        {
            maxDisparity = max(maxDisparity, abs(depth - depthNeighbor));
        }
    }
    valid *= 1.0 - maxDisparity;

    float z = depth * (_FarClip - _NearClip) + _NearClip;
    vertOut = float4(
        (imageCoordinates.x * _ImageDimensions.x - _PrincipalPoint.x) * z / _FocalLength.x,
        (imageCoordinates.y * _ImageDimensions.y - _PrincipalPoint.y) * z / _FocalLength.y,
        z, vertIn.w);

    vertOut = mul(_Extrinsics, vertOut);
}

void dkFragmentPass(float2 depthTexCoord, float2 colorTexCoord, inout float4 col, inout float valid)
{
    float3 depth = tex2D(_MainTex, depthTexCoord).rgb;
    float3 depthhsv = rgb2hsv(depth);
    // if unity is rendering in linear space and we are using external gamma corrected assets, then encode to gamma space pow(2.2) to match the input assets
    col.rgb = pow(tex2D(_MainTex, colorTexCoord).rgb, _LinearColorSpace == 1 && _ExternalTexture == 1 ? 2.2 : 1.0);

    //attenuate the alpha by the saturation & value. each pix should be fully saturated
    col.a = depthhsv.g * depthhsv.b;
    valid *= (depthhsv.r > _Epsilon && depthhsv.g >_DepthSaturationThreshhold && depthhsv.b > _DepthBrightnessThreshold) ? 1.0 : 0.0;
    valid = valid > _InternalEdgeThreshold ? 1.0 : 0.0;
}
