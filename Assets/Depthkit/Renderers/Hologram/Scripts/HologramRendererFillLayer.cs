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
    /// HologramRendererFillLayer adds a layer to render a transparency enabled DK clip projection.
    /// <summary>
    [System.Serializable]
    public class HologramRendererFillLayer : HologramLayer
    {
        /// <summary>
        /// Mesh Density sets the fidelity level of the current mesh. Higher means more polygons
        /// <summary>
        [SerializeField]
        protected ClipRenderer.MeshDensity _meshDensity = ClipRenderer.MeshDensity.Medium;
        // use the public getter/setter only when we need to mark the mesh dirty
        public ClipRenderer.MeshDensity Density
        {
            get { return _meshDensity; }
            set
            {
                _meshDirty = true;
                _meshDensity = value;
            }
        }
        
        /// <summary>
        /// Opacity if the overall fade of the layer
        /// floating point type to expose to Unity timeline.
        /// <summary>
        [Range(0.0f, 1.0f)]
        public float _opacity = 1.0f;

        /// <summary>
        /// track specific x/y density of mesh
        /// <summary>
        private int _vertices = 128;

        /// <summary>
        /// Submits the mesh for rendering
        /// <summary>
        public override void Draw(Matrix4x4 transform, Material material)
        {
            if (_meshDirty || _mesh == null)
            {
                BuildMesh();
                _meshDirty = false;
            }

            // even if opacity is 0, render to depth incase layers above require depth occlusion
            // TODO: optimisation opportunity for true depth only and disable colour target for BW.
            if (isActiveAndEnabled)
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

                _materialBlock.SetFloat("_MeshDensity", _vertices);
                _materialBlock.SetFloat("_Opacity", _opacity);

                Graphics.DrawMesh(_mesh, transform, material, 0, null, 0, _materialBlock);
            }
        }

        /// <summary>
        /// Rebuilds the mesh with the current settings
        /// <summary>
        protected void BuildMesh()
        {
            if (_meshDensity == ClipRenderer.MeshDensity.High)
            {
                _vertices = 255;
            }
            else if (_meshDensity == ClipRenderer.MeshDensity.Medium)
            {
                _vertices = 128;
            }
            else
            {
                _vertices = 64;
            }


            if (_mesh == null)
            {
                _mesh = new Mesh();
            }
            else
            {
                _mesh.Clear();
            }

            //Currently builds a fully tesselated mesh with Topology Triangles
            //Using Topology points or Lines would result in different styles
            Vector3[] verts = new Vector3[_vertices * _vertices];
            int[] indices = new int[(_vertices - 1) * (_vertices - 1) * 2 * 3];

            int curIndex = 0;
            for (int y = 0; y < _vertices - 1; y++)
            {
                for (int x = 0; x < _vertices - 1; x++)
                {
                    indices[curIndex++] = x + y * _vertices;
                    indices[curIndex++] = x + (y + 1) * _vertices;
                    indices[curIndex++] = (x + 1) + y * _vertices;

                    indices[curIndex++] = (x + 1) + (y) * _vertices;
                    indices[curIndex++] = x + (y + 1) * _vertices;
                    indices[curIndex++] = (x + 1) + (y + 1) * _vertices;
                }
            }

            Vector4 textureStep = new Vector4(1.0f / (_vertices - 1), 1.0f / (_vertices - 1), 0.0f, 0.0f);
            curIndex = 0;
            for (int y = 0; y < _vertices; y++)
            {
                for (int x = 0; x < _vertices; x++)
                {
                    verts[curIndex].x = x * textureStep.x;
                    verts[curIndex].y = y * textureStep.y;
                    verts[curIndex].z = 0;
                    curIndex++;
                }
            }

            _mesh.vertices = verts;
            _mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            _mesh.bounds = _meshBounds;
        }

        /// <summary>
        /// Gets the default shader name for this layer
        /// <summary>
        protected override string DefaultShaderString()
        {
            return "DepthKit/HologramFill";
        }
    }
}