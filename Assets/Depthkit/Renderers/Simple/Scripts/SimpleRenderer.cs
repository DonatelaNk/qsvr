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
    /// The Simple Renderer provides the most basic rendering system for DepthKit clips
    /// </summary>
    /// <remarks>
    /// This class provides methods that are implemented in child classes to allow
    /// a way for clip to be rendered in different ways
    /// </remarks>
    public class SimpleRenderer : ClipRenderer
    {
        /// <summary>
        /// Mesh Density sets the fidelity level of the current mesh. Higher means more polygons
        /// <summary>
        [SerializeField]
        protected MeshDensity _meshDensity = MeshDensity.Medium;
        // use the public getter/setter only when we need to mark the mesh dirty
        public MeshDensity Density
        {
            get { return _meshDensity; }
            set
            {
                _geometryDirty = true;
                _meshDensity = value;
            }
        }

        //track specific x/y density of mesh
        private int _vertices = 128;

        [SerializeField]
        protected Shader _shader;
        public Shader Shader
        {
            get { return _shader; }
            set
            {
                _materialDirty = true;
                _shader = value;
            }
        }

        protected Material _material;
        protected Mesh _mesh;

        public override RenderType GetRenderType()
        {
            return RenderType.Simple;
        }

        void Start()
        {
            _mesh = new Mesh();
            BuildMesh();
            BuildMaterial();

            //need this so it draws on first load
            DrawMesh();
        }

        // Update is called once per frame
        void Update()
        {

            if (_geometryDirty)
            {
                BuildMesh();
                _geometryDirty = false;
            }

            if (_materialDirty)
            {
                BuildMaterial();
                _materialDirty = false;
            }

            DrawMesh();
        }

        protected void DrawMesh()
        {

            if (_material != null && _metadata != null)
            {
                SetMaterialProperties(_material);

                //drawing mesh
                Matrix4x4 transformmat = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
                Graphics.DrawMesh(_mesh, transformmat, _material, 0);
            }
        }

        protected void BuildMesh()
        {
            if (_meshDensity == MeshDensity.High)
            {
                _vertices = 255;
            }
            else if (_meshDensity == MeshDensity.Medium)
            {
                _vertices = 128;
            }
            else
            {
                _vertices = 64;
            }

            //////////////////////////////////////////////
            /// Build mesh
            //////////////////////////////////////////////

            _mesh.Clear();

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
            if (_metadata != null)
            {
                _mesh.bounds = new Bounds(_metadata.boundsCenter, _metadata.boundsSize);
            }
        }

        protected void BuildMaterial()
        {

#if UNITY_EDITOR
            //Auto-populate shaders if they are set to null in the Editor
            if (_shader == null)
            {
                _shader = Shader.Find("DepthKit/Simple");
            }
#endif

            if (_shader != null)
            {
                _material = new Material(_shader);
            }
            else
            {
                _material = null;
            }
        }

        protected override void SetMaterialProperties(Material material)
        {
            base.SetMaterialProperties(material);
            material.SetInt("_MeshDensity", _vertices);
        }

        /// <summary>
        /// Cleans the scene of all scripts and game objects generated by this renderer
        /// <summary>
        public override void RemoveComponents()
        {
            if (!Application.isPlaying)
            {
                DestroyImmediate(this, true);
            }
            else
            {
                Destroy(this);
            }
        }
    }
} //END namespace DepthKit