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
using System.Collections.Generic;

namespace DepthKit
{
    [ExecuteInEditMode]
    public class HologramRenderer : ClipRenderer
    {
        public struct ShaderBlendMode
        {
            public Shader s;
            public HologramRenderer.BlendMode m;
            public ShaderBlendMode(Shader s, HologramRenderer.BlendMode m)
            {
                this.s = s;
                this.m = m;
            }
        };

        public Dictionary<ShaderBlendMode, Material> _materials;

        private bool _materialsDirty;
        private bool _layersDirty;
        private Bounds _bounds;

        public HologramLayer[] _layers;


        public override RenderType GetRenderType()
        {
            return RenderType.Hologram;
        }

        public void Start()
        {
            _materialsDirty = true;
            _layersDirty = true;
			if(gameObject.GetComponent<HologramRendererFillLayer>() == null)
	            gameObject.AddComponent<HologramRendererFillLayer>();
        }

        public void OnEnable()
        {
            _materialsDirty = true;
            _layersDirty = true;
        }

        public void SetMaterialsDirty()
        {
            _materialDirty = true;
        }

        public void SetLayersDirty()
        {
            _layersDirty = true;
            _materialDirty = true;

            // if we are in editor and add a child component, we want to update straight away not when we play.
            Update();
        }

        void Update()
        {
            if (_layersDirty)
            {
                _layers = GetComponentsInChildren<HologramLayer>();
                if (_layers == null)
                {
                    Debug.LogError("No Layers Found");
                    return;
                }

                _layersDirty = false;
                _materialsDirty = true;
            }

            //build materials if shaders are not set or have changed
            if (_materialsDirty)
            {
                BuildMaterials();
                _materialsDirty = false;
            }

            //draw the layers
            Matrix4x4 transformmat = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            Material m;

            if (_layers != null)
            {
                foreach (HologramLayer layer in _layers)
                {
                    if (layer == null)
                    {
                        continue;
                    }

                    ShaderBlendMode materialKey = new ShaderBlendMode(layer.Shader, layer.BlendMode);
                    if (_materials.ContainsKey(materialKey))
                    {
                        m = _materials[materialKey];
                        SetMaterialProperties(m);
                        layer.Draw(transformmat, m);
                    }
                    else
                    {
                        Debug.LogError("Blending Mode / Shader combo not found in Materials: " + layer.Shader + " " + layer.BlendMode);
                        _materialsDirty = true;
                    }
                }
            }
        }

        protected void BuildMaterials()
        {
            if(_metadata != null)
                _bounds = new Bounds(_metadata.boundsCenter, _metadata.boundsSize);

            if (_layers != null)
            {
                foreach (HologramLayer layer in _layers)
                {
                    if (layer != null)
                    {
                        layer.MeshBounds = _bounds;
                    }
                }
            }


            //Create materials for each blending mode
            //Blending modes cannot be changed by MaterialPropertyBlocks,
            //so we need to have a separate material to pass to each layer based on its setting

            _materials = new Dictionary<ShaderBlendMode, Material>();
            HashSet<Shader> layerShaders = new HashSet<Shader>();
            foreach (HologramLayer layer in _layers)
            {
                if (layer != null && layer.Shader != null)
                {
                    layerShaders.Add(layer.Shader);
                }
            }

            foreach (BlendMode mode in BlendMode.GetValues(typeof(BlendMode)))
            {
                foreach (Shader shader in layerShaders)
                {
                    ShaderBlendMode sbm = new ShaderBlendMode(shader, mode);

                    Material mat = new Material(shader);
                    mat.SetInt("_SrcMode", (int)GetSrcMode(mode));
                    mat.SetInt("_DstMode", (int)GetDstMode(mode));
                    mat.SetInt("_BlendEnum", (int)mode);

                    _materials.Add(sbm, mat);
                }
            }
        }

        protected override void SetMaterialProperties(Material material)
        {
            base.SetMaterialProperties(material);
        }

        public void OnValidate()
        {
            if (_layers != null)
            {
                foreach (HologramLayer layer in _layers)
                {
                    if (layer != null)
                    {
                        layer.OnValidate();
                    }
                }
            }
        }

        void OnDrawGizmos()
        {
        }

        public override void RemoveComponents()
        {
            if (_layers != null)
            {
                foreach (HologramLayer layer in _layers)
                {
                    if (layer != null)
                    {
                        if (!Application.isPlaying)
                        {
                            DestroyImmediate(layer, false);
                        }
                        else
                        {
                            Destroy(layer);
                        }
                    }
                }
            }

            if (!Application.isPlaying)
            {
                DestroyImmediate(this, false);
            }
            else
            {
                Destroy(this);
            }
        }

        //BLENDING MODES
        public enum BlendMode
        {
            Alpha = 0,  // Premultiplied transparency Blend SrcAlpha OneMinusSrcAlpha 
            Add = 1,    // Additive Blend One One 
            Multiply = 2,   // Multiplicative Blend DstColor Zero
            Screen = 3  // Soft Additive Blend One OneMinusDstColor  
        }

        //Fetch appropriate Src/Dst modes for blending modes above
        private static UnityEngine.Rendering.BlendMode GetSrcMode(BlendMode mode)
        {
            switch (mode)
            {
                case BlendMode.Alpha:
                    return UnityEngine.Rendering.BlendMode.SrcAlpha;

                case BlendMode.Add:
                    return UnityEngine.Rendering.BlendMode.One;

                case BlendMode.Multiply:
                    return UnityEngine.Rendering.BlendMode.DstColor;

                case BlendMode.Screen:
                    return UnityEngine.Rendering.BlendMode.One;

                default:
                    return UnityEngine.Rendering.BlendMode.SrcAlpha;
            }
        }

        private static UnityEngine.Rendering.BlendMode GetDstMode(BlendMode mode)
        {
            switch (mode)
            {
                case BlendMode.Alpha:
                    return UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;

                case BlendMode.Add:
                    return UnityEngine.Rendering.BlendMode.One;

                case BlendMode.Multiply:
                    return UnityEngine.Rendering.BlendMode.Zero;

                case BlendMode.Screen:
                    return UnityEngine.Rendering.BlendMode.OneMinusSrcColor;

                default:
                    return UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
            }
        }
    }
}