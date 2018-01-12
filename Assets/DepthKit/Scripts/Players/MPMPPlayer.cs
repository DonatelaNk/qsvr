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
    /// Implementation of the DepthKit player with an MPMP-based backend </summary>
#if (!DK_USING_MPMP)
    public class MPMPPlayer // : ClipPlayer
    {
#else
    public class MPMPPlayer : ClipPlayer
    {
        // the MPMP player component 
        [SerializeField, HideInInspector]
        /// <summary>
        /// Reference to the MPMP component attached to this script. </summary>
        protected monoflow.MPMP _mediaPlayer;

        // [SerializeField, HideInInspector]
        /// <summary>
        /// The path to the movie for MPMP to play </summary>
        // protected string _moviePath;
        public override bool SetValues(ClipPlayer.PlayerValues values)
        {
            _mediaPlayer = gameObject.GetComponent<monoflow.MPMP>();

            if (_mediaPlayer == null)
            {
                // no media component already added to this component, try adding a MediaPlayer component
                try
                {
                    _mediaPlayer = gameObject.AddComponent<monoflow.MPMP>();
                }

                catch (Exception e)
                {
                    Debug.LogError("MPMP not found in project: " + e.ToString());
                    return false;
                }
            }

            if (values.moviePath != "")
            {
                //set the path of the movie based on passed in params
                _mediaPlayer.videoPath = values.moviePath;
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
            _mediaPlayer.Load(_mediaPlayer.videoPath);
            Events.OnClipLoadingStarted();

            //while the video is loading you can't play it
            while (_mediaPlayer.IsLoading())
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
            if (VideoLoaded)
            {
                _mediaPlayer.Play();
                Events.OnClipPlaybackStarted();
            }
        }

        public override void Pause()
        {
            _mediaPlayer.Pause();
            Events.OnClipPlaybackPaused();
        }
        public override void Stop()
        {
            _mediaPlayer.Stop();
            Events.OnClipPlaybackStopped();
        }

        public override void SetLoop(bool loopStatus)
        {
            _mediaPlayer.looping = loopStatus;
        }

        public override void SetVolume(float volume)
        {
            _mediaPlayer.volume = volume;
        }
        public override void SeekTo(float time)
        {
            time = time % (float)_mediaPlayer.GetDuration();
            _mediaPlayer.SeekTo((float)time, true);
        }

        public override double GetCurrentTime()
        {
            return _mediaPlayer.GetCurrentPosition();
        }
        public override int GetCurrentFrame()
        {
            //No way to determine the frame # with MPMP
            return -1;
        }
        public override double GetDuration()
        {
            return _mediaPlayer.GetDuration();
        }

        public override Texture GetTexture()
        {
            return _mediaPlayer.GetVideoTexture();
        }

        public override bool IsTextureFlipped()
        {
#if (UNITY_ANDROID && !UNITY_EDITOR)
            return false;
#endif
            return true;
        }

        public override bool IsExternalTexture()
        {
            return false;
        }

        public override bool IsPlaying()
        {
            return _mediaPlayer.IsPlaying();
        }

        public override void RemoveComponents()
        {
            if (!Application.isPlaying)
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
            return AvailablePlayerType.MPMP;
        }

        public monoflow.MPMP GetPlayerBackend()
        {
            return _mediaPlayer;
        }
#endif
    }
}
