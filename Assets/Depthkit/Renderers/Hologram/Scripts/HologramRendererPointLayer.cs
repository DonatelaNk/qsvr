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

using UnityEngine;
using System.Collections;

namespace DepthKit
{
    /// <summary>
    /// HologramRendererPointLayer adds a layer of point based effect to the model
    /// <summary>
    [System.Serializable]
    public class HologramRendererPointLayer : HologramLayer
    {
        /// <summary>
        /// Self occlusion property so we do not see occluded points
        /// <summary>
        public bool _selfOcclusion = false;

        /// <summary>
        /// How many columns of points are drawn
        /// floating point type to expose to Unity timeline.
        /// <summary>
        [Range(0, 255)]
        public float _pointColumns = 20;
        private float _prevPointColumns = 20;

        /// <summary>
        /// How many rows of points are drawn
        /// floating point type to expose to Unity timeline.
        /// <summary>
        [Range(0, 255)]
        public float _pointRows = 20;
        private float _prevPointRows = 20;

        /// <summary>
        /// Opacity if the overall fade of the layer
        /// floating point type to expose to Unity timeline.
        /// <summary>
        [Range(0.0f, 1.0f)]
        public float _opacity = 1.0f;

        /// <summary>
        /// Width of the points
        /// floating point type to expose to Unity timeline.
        /// <summary>
        [Range(0.0f, 100.0f)]
        public float _pointWidth = 10.0f;

        /// <summary>
        /// Width of the points
        /// floating point type to expose to Unity timeline.
        /// <summary>
        [Range(0.0f, 100.0f)]
        public float _pointHeight = 10.0f;

        /// <summary>
        /// Line Sprite is a texture that is spread across the line
        /// <summary>
        public Texture2D _pointSprite;

        /// <summary>
        /// Submits the mesh for rendering
        /// <summary>
        public override void Draw(Matrix4x4 transform, Material material)
        {
            if (_meshDirty ||
                _mesh == null ||
                _pointColumns != _prevPointColumns ||
                _pointRows != _prevPointRows)
            {
                BuildMesh();
                _meshDirty = false;
                _prevPointColumns = _pointColumns;
                _prevPointRows = _pointRows;
            }

            if (isActiveAndEnabled && _opacity > 0.0f)
            {
                //Push all the items into material property block
                //We use this technique in order to the share the material between all the different layers
                if (_materialBlock == null)
                {
                    _materialBlock = new MaterialPropertyBlock();
                }
                else
                {
                    _materialBlock.Clear();
                }

                if (_selfOcclusion)
                {
                    material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
                }
                else
                {
                    material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
                }

                if (_pointSprite != null)
                {
                    _materialBlock.SetTexture("_Sprite", _pointSprite);
                    _materialBlock.SetFloat("_UseSprite", 1.0f);
                }
                else
                {
                    _materialBlock.SetFloat("_UseSprite", 0.0f);
                }

                _materialBlock.SetFloat("_Width", _pointWidth/1000.0f);
                _materialBlock.SetFloat("_Height", _pointHeight/1000.0f);
                _materialBlock.SetFloat("_Opacity", _opacity);

                Graphics.DrawMesh(_mesh, transform, material, 0, null, 0, _materialBlock);
            }
        }

        /// <summary>
        /// Rebuilds the mesh with the current settings
        /// <summary>
        void BuildMesh()
        {
            if (_mesh == null)
            {
                _mesh = new Mesh();
            }
            else
            {
                _mesh.Clear();
            }

            ///////// Build Points /////////

            Vector3[] verts = new Vector3[(int)_pointColumns * (int)_pointRows];
            int[] indices = new int[(int)_pointColumns * (int)_pointRows];

            int curIndex = 0;
            Vector2 textureStep = new Vector2(1.0f / (_pointColumns - 1), 1.0f / (_pointRows - 1));

            for (int y = 0; y < (int)_pointRows; y++)
            {
                for (int x = 0; x < (int)_pointColumns; x++)
                {
                    indices[curIndex] = curIndex;

                    verts[curIndex].x = x * textureStep.x;
                    verts[curIndex].y = y * textureStep.y;
                    verts[curIndex].z = 0;

                    curIndex++;
                }
            }

            _mesh.vertices = verts;
            _mesh.SetIndices(indices, MeshTopology.Points, 0);
            _mesh.bounds = _meshBounds;
        }

        /// <summary>
        /// Gets the default shader name for this layer
        /// <summary>
        protected override string DefaultShaderString()
        {
            return "DepthKit/HologramPoints";
        }
    }
}
