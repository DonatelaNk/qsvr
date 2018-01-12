/************************************************************************************

DepthKit Unity SDK License v1
Copyright 2016-2017 Depth Kit Inc. All Rights reserved.  

Licensed under the Depth Kit Inc. Software Development Kit License Agreement (the "License"); 
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
    [System.Serializable]
    abstract public class HologramLayer : MonoBehaviour
    {
        protected Mesh _mesh;
        protected bool _meshDirty = true;
        protected MaterialPropertyBlock _materialBlock;

        protected HologramRenderer parentRenderer;

        /// <summary>
        /// Set bounds on the mesh contained within this layer
        /// <summary>
        [SerializeField]
        [HideInInspector]
        protected Bounds _meshBounds;
        // use the public getter/setter only when we need to mark the mesh dirty
        public Bounds MeshBounds
        {
            get { return _meshBounds; }
            set
            {
                _meshBounds = value;
                if (_mesh != null)
                {
                    _mesh.bounds = _meshBounds;
                }
            }
        }

        /// <summary>
        /// Set the shader for this layer. By default this loads built in shader path
        /// implemented by the subclass
        /// <summary>
		[SerializeField]
        protected Shader _shader;
        public Shader Shader
        {
            get { return _shader; }
            set
            {
                _shader = value;
                if (parentRenderer != null)
                {
                    parentRenderer.SetMaterialDirty();
                }
                else
                {
                    Init();
                }
            }
        }

        /// <summary>
        /// Blending Mode for this layer
        /// <summary>
        [SerializeField]
        private HologramRenderer.BlendMode _blendMode;
        public HologramRenderer.BlendMode BlendMode
        {
            get { return _blendMode; }
            set { _blendMode = value; }
        }

        void Awake()
        {
            Init();
        }

        void Start()
        {
            Init();
        }

        private void Reset()
        {
            Init();
        }

        void OnEnable()
        {
            Init();
        }

        protected void Init()
        {
            if (parentRenderer == null)
            {
                parentRenderer = GetComponent<HologramRenderer>();
                if (parentRenderer == null)
                {
                    parentRenderer = gameObject.AddComponent<HologramRenderer>();
                }
            }

            if (this.Shader == null)
            {
                this.Shader = Shader.Find(DefaultShaderString());
            }

            parentRenderer.SetLayersDirty();
        }

        public void OnValidate()
        {
            _meshDirty = true;
            if (this.Shader == null)
            {
                this.Shader = Shader.Find(DefaultShaderString());
            }

            if (parentRenderer != null)
            {
                parentRenderer.SetMaterialDirty();
            }
        }

        void OnDestroy()
        {
            if (parentRenderer != null)
            {
                parentRenderer.SetLayersDirty();
            }
        }

        abstract public void Draw(Matrix4x4 transform, Material material);
        abstract protected string DefaultShaderString();
    }
}