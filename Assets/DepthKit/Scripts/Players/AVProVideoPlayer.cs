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
using System;
using System.Collections;

namespace DepthKit
{
    /// <summary>
    /// Implmentation of the DepthKit player with an AVProVideo-based backend </summary>
#if (!DK_USING_AVPRO)
    public class AVProVideoPlayer // : ClipPlayer
    {
#else
    public class AVProVideoPlayer : ClipPlayer
    {
        [SerializeField, HideInInspector]
        /// <summary>
        /// Reference to the AVProVideo Component </summary>
        protected RenderHeads.Media.AVProVideo.MediaPlayer _mediaPlayer;

        /// <param name="moviePath">Path to the movie. Format here varies based on location.</param>
        /// <param name="fileLocation">What file or folder the given path is relative to.</param>
        public override bool SetValues(PlayerValues values)
        {
            _mediaPlayer = gameObject.GetComponent<RenderHeads.Media.AVProVideo.MediaPlayer>();

            if (_mediaPlayer == null)
            {
                // no media component already added to this component, try adding a MediaPlayer component
                try
                {
                    _mediaPlayer = gameObject.AddComponent<RenderHeads.Media.AVProVideo.MediaPlayer>();
                }
                catch (Exception e)
                {
                    Debug.LogError("AVProVideo not found in project: " + e.ToString());
                    return false;
                }
            }

            //disable autoplay and autoload, which are on by default
            _mediaPlayer.m_AutoStart = false;
            _mediaPlayer.m_AutoOpen = false;

            //relies on exact parity between enum values in AVProVideo and Clip
            _mediaPlayer.m_VideoLocation = (RenderHeads.Media.AVProVideo.MediaPlayer.FileLocation)((int)values.location);

            if (values.moviePath != "")
            {
                //cache the movie path reference
                _mediaPlayer.m_VideoPath = values.moviePath;
                return true;
            }

            return false;
        }

        public override bool IsPlayerCreated()
        {
            return _mediaPlayer != null;
        }

        public override IEnumerator Load()
        {
            //start the loading operation
            _mediaPlayer.OpenVideoFromFile(_mediaPlayer.m_VideoLocation, _mediaPlayer.m_VideoPath, false);
            Events.OnClipLoadingStarted();

            //while the video is loading you can't play it
            while (!_mediaPlayer.Control.CanPlay())
            {
                VideoLoaded = false;
                yield return null;
            }
            VideoLoaded = true;
            Events.OnClipLoadingFinished();
            yield return null;
        }

        public override void StartVideoLoad()
        {
            StartCoroutine(Load());
        }

        public override IEnumerator LoadAndPlay()
        {
            StartVideoLoad();
            while (!VideoLoaded)
            {
                yield return null;
            }
            Play();
            yield return null;
        }
        public override void Play()
        {
            if(VideoLoaded)
            {
                _mediaPlayer.Control.Play();
                Events.OnClipPlaybackStarted();
            }
        }
        public override void Pause()
        {
            _mediaPlayer.Control.Pause();
            Events.OnClipPlaybackPaused();
        }
        public override void Stop()
        {
            _mediaPlayer.Control.Stop();
            Events.OnClipPlaybackStopped();
        }

        public override void SetLoop(bool loopStatus)
        {
            if (_mediaPlayer.Control != null)
            {
                _mediaPlayer.Control.SetLooping(loopStatus);
                return;
            }

            _mediaPlayer.m_Loop = loopStatus;
        }

        public override void SetVolume(float volume)
        {
            if (_mediaPlayer.Control != null)
            {
                _mediaPlayer.Control.SetVolume(volume);
                return;
            }

            _mediaPlayer.m_Volume = volume;
        }

        public override void SeekTo(float time)
        {
            //convert to ms
            time = time * 1000;

            //map to the range of the Clip
            time = time % _mediaPlayer.Info.GetDurationMs();

            //seek to the proper section
            _mediaPlayer.Control.Seek(time);
        }

        public override double GetCurrentTime()
        {
            return _mediaPlayer.Control.GetCurrentTimeMs() / 1000;
        }

        public override int GetCurrentFrame()
        {
            if (_mediaPlayer != null && _mediaPlayer.TextureProducer != null)
            {
                return _mediaPlayer.TextureProducer.GetTextureFrameCount();
            }
            return -1;
        }

        public override double GetDuration()
        {
            return _mediaPlayer.Info.GetDurationMs() / 1000;
        }
        public override Texture GetTexture()
        {
            if (_mediaPlayer != null && _mediaPlayer.TextureProducer != null)
            {
                return _mediaPlayer.TextureProducer.GetTexture ();
            }
            return null;
        }
        public override bool IsTextureFlipped ()
        {
            //#if (UNITY_ANDROID && !UNITY_EDITOR)
            //            return false;
            //#endif
            //            return true;
            if (_mediaPlayer != null && _mediaPlayer.TextureProducer != null)
            {
                return _mediaPlayer.TextureProducer.RequiresVerticalFlip();
            }
            return false;
        }

        public override bool IsExternalTexture()
        {
#if UNITY_STANDALONE_WIN
            return _mediaPlayer.PlatformOptionsWindows.useHardwareDecoding && _mediaPlayer.PlatformOptionsWindows.videoApi == RenderHeads.Media.AVProVideo.Windows.VideoApi.MediaFoundation;
#else
            return false;
#endif
        }

        public override bool IsPlaying()
        {
            return _mediaPlayer.Control.IsPlaying();
        }

        public override void RemoveComponents()
        {
            if(!Application.isPlaying)
            {
                DestroyImmediate(_mediaPlayer, true);
                DestroyImmediate(this, true);
            }
            else
            {
                Destroy(_mediaPlayer);
                Destroy(this);
            }
        }

        public override AvailablePlayerType GetPlayerType()
        {
            return AvailablePlayerType.AVProVideo;
        }

        public RenderHeads.Media.AVProVideo.MediaPlayer GetPlayerBackend()
        {
            return _mediaPlayer;
        }
#endif
    }
}