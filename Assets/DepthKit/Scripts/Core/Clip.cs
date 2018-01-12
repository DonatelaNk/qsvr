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
using System.Text;
using System.IO;
using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace DepthKit
{

    /// <summary>
    /// Where the video file is located relative to the selection </summary>
    /// <remarks>
    /// This is used for AVProVideo, but only exposed if that option is selected as the backend </remarks>
    public enum FileLocation
    {
        AbsolutePathOrURL,
        RelativeToProjectFolder,
        RelativeToStreamingAssetsFolder,
        RelativeToDataFolder,
        RelativeToPeristentDataFolder,
    }

    /// <summary>
    /// The type of rendering that should be used for DepthKit clip </summary>
    /// <remarks>
    /// Users can extend rendering by placing a new renderer in here. </remarks>
    public enum RenderType
    {
        Simple,
        Hologram
    }

    /// <summary>
    /// A DepthKit clip </summary>
    /// <remarks>
    /// Class that holds DepthKit data and prepares clips for playback in the editor. </remarks>
    [RequireComponent(typeof(BoxCollider))]
    [ExecuteInEditMode]
    public class Clip : MonoBehaviour
    {

        /// <summary>
        ///  What kind of player backend is playing the Clip.</summary>
        [SerializeField]
        protected ClipPlayer _player;
        public ClipPlayer Player
        {
            get
            {
                return _player;
            }

            protected set
            {
                _player = value;
            }
        }

        public PlayerEvents Events
        {
            get
            {
                if (Player != null)
                {
                    return Player.Events;
                }
                Debug.LogError("Unable to access events as player is currently null");
                return null;
            }
        }

        /// <summary>
        ///  What kind of renderer backend is playing the Clip.</summary>
        [SerializeField]
        protected ClipRenderer _renderer;
        public ClipRenderer ClipRenderer
        {
            get
            {
                return _renderer;
            }

            protected set
            {
                _renderer = value;
            }
        }

        /// <summary>
        /// The packing value of a clip used to tweak lasers on a given clip </summary>
        [Range(0.000f,0.03f)]
        public float _depthPackingEpsilon = 0.0005f;
        /// <summary>
        /// Set the saturation threshold for the depth texture </summary>
        [Range(0.0f,1.0f), Tooltip("First pass edge cleanup. Should be as high as possible without clipping the clip internally.")]
        public float _edgeCleanup1 = 0.5f; //_depthSaturationThreshold
        /// <summary>
        /// Set the brightness threshold for the depth texture  </summary>
        [Range(0.0f,1.0f), Tooltip("Second pass edge cleanup. Should be as high as possible without clipping the clip internally.")] 
        public float _edgeCleanup2 = 0.5f; //_depthBrightnessThreshold

        /// <summary>
        /// The bounding box collider</summary>
        [SerializeField]
        protected BoxCollider _collider;

        public static AvailablePlayerType _defaultPlayerType = AvailablePlayerType.UnityVideoPlayer;

        /// <summary>
        /// The type of player, as expressed through the Unity Inspector.</summary>
        public AvailablePlayerType _playerType;

        /// <summary>
        /// The type of renderer, as expressed through the Unity Inspector.</summary>
        public RenderType _renderType = RenderType.Simple;

        /// <summary>
        /// The metadata file that cooresponds to a given clip. This is exported from Visualize.</summary>
        public TextAsset _metaDataFile;

        /// <summary>
        /// Allow metadata to be loaded dynamicly from streaming assets.</summary>
        public bool _dynamicMetadataFile = false;
        public string _metaDataFilePath;

        #region Imported DepthKit Data

        /// <summary>
        /// The poster frame for a DepthKit capture.</summary>
        public Texture2D _poster;

        /// <summary>
        /// Use a VideoClip if in version 5.6 as the default</summary>
        public UnityEngine.Video.VideoClip _videoClip;

        /// <summary>
        /// String path of movie. This field has different requirements depending on what player is being used.</summary>
        public string _moviePath;

        /// <summary>
        /// For AVProVideo, this says where the video is located relative to other aspects of the project.</summary>
        public FileLocation _fileLocation;

        /// <summary> Reference to the metadata object fromed from the imported metadata file</summary>
        [SerializeField]
        private Metadata _metaData;
        #endregion

        /// <summary>Should the player backend be updated</summary>
        public bool _needToResetPlayerType;
        /// <summary>Should the player values be updated</summary>
        public bool _needToRefreshPlayerValues;
        /// <summary>Should the renderer backend be updated</summary>
        public bool _needToResetRenderType;
        /// <summary>Should the renderer backend be updated</summary>
        public bool _needToRefreshMetadata;
        public bool _autoPlay = true;
        public bool _autoLoad = true;
        public float _delaySeconds = 0.0f;
        public bool _videoLoops = false;
        public float _volume = 0.5f;

        /// <summary>True when the Clip Player is sucessfully configured.</summary>
        public bool _playerSetup;
        /// <summary>True when valid metadata is loaded into Metada object.</summary>
        public bool _metaSetup;
        /// <summary>true when the renderer is configured properly.</summary>
        public bool _rendererSetup;

        private bool _autoplayTriggered = false;
        private bool _autoLoadTriggered = false;
        private int _lastFrame = -1;

        /// <summary>Whether or not the clip is fully setup</summary>
        public bool IsSetup
        {
            get
            {
                return _playerSetup && _rendererSetup && _metaSetup;
            }
        }

        void Start()
        {
            _autoplayTriggered = false;
            _autoLoadTriggered = false;

            _needToResetPlayerType = false;
            _needToRefreshPlayerValues = false;
            _needToRefreshMetadata = false;
            _needToResetRenderType = false;

            _collider = GetComponent<BoxCollider>();

            if (_renderer)
            {
                _renderer.Poster = _poster;
                _renderer.Metadata = _metaData;
                _rendererSetup = true;
            }
        }

        void Reset() //native monobehavior call
        {
            _playerType = _defaultPlayerType;
        } 
        /// <summary>      
        /// Configures the player with a TextAsset resource
        /// </summary>
        public bool Setup(ClipPlayer.PlayerValues values, AvailablePlayerType playerType, RenderType renderType, TextAsset metadata)
        {
            Setup(values, playerType, renderType);

            _dynamicMetadataFile = false;
            _metaDataFile = metadata;
            RefreshMetaData();

            return IsSetup;
        }

        public bool Setup(ClipPlayer.PlayerValues values, AvailablePlayerType playerType, RenderType renderType, string metadataPath)
        {
            Setup(values, playerType, renderType);

            _dynamicMetadataFile = true;
            _metaDataFilePath = metadataPath;
            RefreshMetaData();

            return IsSetup;
        }

        public void SwapVideo(string moviePath, string metadataPath)
        {
            _moviePath = moviePath;
            _metaDataFilePath = metadataPath;
            _dynamicMetadataFile = true;
            RefreshMetaData();
            RefreshPlayerValues();
        }

        protected void Setup(ClipPlayer.PlayerValues values, AvailablePlayerType playerType, RenderType renderType)
        {
            //configure the clip with all the inputs
            _moviePath = values.moviePath;
            _fileLocation = values.location;
            _videoClip = values.videoClip;

            _playerType = playerType;
            _renderType = renderType;

            //build the components
            ResetPlayer();
            ResetRenderer();
        }

        void Update()
        {

            //safety checks
            if (_renderer == null)
            {
                ResetRenderer();
            }

            if (_player == null)
            {
                ResetPlayer();
            }

#if UNITY_EDITOR
            if (_needToResetRenderType)
            {
                ResetRenderer();
                _needToResetRenderType = false;
            }

            if (_needToResetPlayerType)
            {
                ResetPlayer();
                _needToResetPlayerType = false;
            }

            if (_needToRefreshPlayerValues || !_player.IsPlayerCreated())
            {
                RefreshPlayerValues();
                _needToRefreshPlayerValues = false;
            }

            if (_needToRefreshMetadata)
            {
                RefreshMetaData();
                _needToRefreshMetadata = false;
            }
#endif

            if (IsSetup)
            {
                //set player params based on selected Clip settings
                Player.SetLoop(_videoLoops);
                Player.SetVolume(_volume);

                int frame = Player.GetCurrentFrame();

                if (frame == -1 || _lastFrame != frame)
                {
                    //Get the texture from the provider!
                    _renderer.Texture = Player.GetTexture();
                    _renderer.TextureIsFlipped = Player.IsTextureFlipped();
                    _renderer.TextureIsExternal = Player.IsExternalTexture();

                    //set the render tweak values
                    _renderer._depthPackingEpsilon = _depthPackingEpsilon;
                    _renderer._depthSaturationThreshold = _edgeCleanup1;
                    _renderer._depthBrightnessThreshold = _edgeCleanup2;
                    _lastFrame = frame;
                }

                //properly start the video for autoplay modes
                if (Application.isPlaying)
                {
                    if (_autoLoad && !_autoLoadTriggered)
                    {
                        _autoLoadTriggered = true;
                        Player.StartVideoLoad();
                    }

                    if (_autoPlay && !_autoplayTriggered && Time.time > _delaySeconds)
                    {
                        //clip will not auto play until the loading action is complete
                        if (Player.VideoLoaded)
                        {
                            Player.Play();
                            _autoplayTriggered = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when the player itself changes </summary>
        public void ResetPlayer()
        {
            //try to set the player variable to any Player type component on this script
            Player = gameObject.GetComponent<ClipPlayer>();

            //Short circuit if we are setting the same player as before
            if (Player != null && Player.GetPlayerType() == _playerType)
            {
                return;
            }

            //destroy the components that player references
            //use a for loop to get around the component potentially shifting in the event of an undo
            ClipPlayer[] attachedPlayers = GetComponents<ClipPlayer>();
            for (int i = 0; i < attachedPlayers.Length; i++)
            {
                attachedPlayers[i].RemoveComponents();
            }
            Player = null;

            //add the new components
            switch (_playerType)
            {
#if DK_USING_MPMP
                case AvailablePlayerType.MPMP:
                    {
                        Player = gameObject.AddComponent<MPMPPlayer>();
                        break;
                    }
#endif
#if DK_USING_AVPRO
                case AvailablePlayerType.AVProVideo:
                    {
                        Player = gameObject.AddComponent<AVProVideoPlayer>();
                        break;
                    }
#endif
                case AvailablePlayerType.UnityVideoPlayer:
                    {
                        Player = gameObject.AddComponent<UnityVideoPlayer>();
                        break;
                    }
            }

            RefreshPlayerValues();
        }

        /// <summary>
        /// Called when player vars are changed but player itself isn't changed </summary>
        public void RefreshPlayerValues()
        {
            ClipPlayer.PlayerValues values = new ClipPlayer.PlayerValues();
            values.moviePath = _moviePath;
            values.location = _fileLocation;
            values.videoClip = _videoClip;
            _playerSetup = Player.SetValues(values);
        }

        public void ResetRenderer()
        {
            _renderer = gameObject.GetComponent<ClipRenderer>();

            if (_renderer != null)
            {
                //ensure the new render type isn't the same as the one we already have
                if (_renderType == _renderer.GetRenderType())
                {
                    return;
                }

                ClipRenderer[] attachedRenderers = GetComponents<ClipRenderer>();
                for (int i = 0; i < attachedRenderers.Length; i++)
                {
                    attachedRenderers[i].RemoveComponents();
                }
            }

            _renderer = null;

            //add the new components
            switch (_renderType)
            {
                case RenderType.Simple:
                    _renderer = gameObject.AddComponent<SimpleRenderer>();
                    break;
                case RenderType.Hologram:
                    _renderer = gameObject.AddComponent<HologramRenderer>();
                    break;
                default:
                    _renderer = gameObject.AddComponent<SimpleRenderer>();
                    break;
            }

            _renderer.Metadata = _metaData;
            _renderer.Poster = _poster;

            _rendererSetup = true;
        }

        void RefreshMetaData()
        {
            string metaDataJson = "";
            _metaSetup = false;
            if (_dynamicMetadataFile && !string.IsNullOrEmpty(_metaDataFilePath))
            {
                //TODO: allow for more than just streaming assets
                metaDataJson = System.IO.File.ReadAllText(Path.Combine(Application.streamingAssetsPath, _metaDataFilePath));
            }
            else if (_metaDataFile != null)
            {
                metaDataJson = _metaDataFile.text;
            }

            //If there is no metadata, bail!
            if (metaDataJson == "")
            {
                return;
            }

            try
            {
                _metaData = Metadata.CreateFromJSON(metaDataJson);
            }
            catch (System.Exception)
            {
                Debug.LogError("Invaid DepthKit Metadata Format. Make sure you are using the proper metadata export from DepthKit Visualize.");
                return;
            }

            if (_collider == null)
            {
                _collider = GetComponent<BoxCollider>();
            }

            _collider.center = _metaData.boundsCenter;
            _collider.size = _metaData.boundsSize;

            if (_renderer != null)
            {
                _renderer.Metadata = _metaData;
                _renderer.Poster = _poster;
            }

            _metaSetup = true;
        }

        void OnDrawGizmos()
        {

            if (Application.isPlaying && _metaData != null)
            {
                Gizmos.color = new Color(.5f, 1.0f, 0, 0.5f);
                Gizmos.DrawWireSphere(
                    transform.localToWorldMatrix * new Vector4(_metaData.boundsCenter.x, _metaData.boundsCenter.y, _metaData.boundsCenter.z, 1.0f),
                    transform.localScale.x * _metaData.boundsSize.x * .5f);
            }
        }

        void OnApplicationQuit()
        {
            if (Player != null)
            {
                Player.Stop();
            }
        }

    }

}