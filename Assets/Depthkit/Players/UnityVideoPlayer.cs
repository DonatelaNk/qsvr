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
using System;
using System.Collections;

namespace DepthKit
{
    /// <summary>
    /// Implmentation of the DepthKit player with the Unity VideoPlayer-based backend.
    /// </summary>
    public class UnityVideoPlayer : ClipPlayer
    {
        //reference to the MovieTexture passed in through Clip
        [SerializeField, HideInInspector]
        protected UnityEngine.Video.VideoPlayer _mediaPlayer;
        [SerializeField, HideInInspector]
        AudioSource _audioSource;

        /// <param name="clip">VideoClip reference</param>
        public override bool SetValues(PlayerValues values)
        {
            _mediaPlayer = gameObject.GetComponent<UnityEngine.Video.VideoPlayer>();

            if (_mediaPlayer == null)
            {
                // no media component already added to this component, try adding a MediaPlayer component
                try
                {
                    _mediaPlayer = gameObject.AddComponent<UnityEngine.Video.VideoPlayer>();
                }
                catch (Exception e)
                {
                    Debug.LogError("AVProVideo not found in project: " + e.ToString());
                    return false;
                }
            }

            _audioSource = gameObject.GetComponent<AudioSource>();

            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
            if (_mediaPlayer == null)
            {
                _mediaPlayer = gameObject.AddComponent<UnityEngine.Video.VideoPlayer>();
            }

            _mediaPlayer.audioOutputMode = UnityEngine.Video.VideoAudioOutputMode.AudioSource;
            _mediaPlayer.SetTargetAudioSource(0, _audioSource);
            _mediaPlayer.renderMode = UnityEngine.Video.VideoRenderMode.APIOnly;
            _mediaPlayer.clip = values.videoClip;
            _mediaPlayer.prepareCompleted += OnVideoLoadingComplete;
            _mediaPlayer.EnableAudioTrack(0, true);
            return true;
        }

        public override bool IsPlayerCreated()
        {
            return _mediaPlayer != null;
        }

        public override void StartVideoLoad()
        {
            StartCoroutine(Load());
        }

        public override IEnumerator Load()
        {
            Events.OnClipLoadingStarted();
            _mediaPlayer.Prepare();
            yield return null;
        }

        public void OnVideoLoadingComplete(UnityEngine.Video.VideoPlayer player)
        {
            VideoLoaded = true;
            Events.OnClipLoadingFinished();
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
            _mediaPlayer.Play();
            Events.OnClipPlaybackStarted();
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
            _mediaPlayer.isLooping = loopStatus;
        }

        public override void SetVolume(float volume)
        {
            //need to set with audio source instead of trying to do directaudio
            // _mediaPlayer.volume = volume;
        }
        public override void SeekTo(float time)
        {
            double seekFrame =_mediaPlayer.clip.frameRate * time;
            _mediaPlayer.frame = (long)seekFrame;
        }
        public override int GetCurrentFrame()
        {
            //No way to determine the frame # with MPMP
            return (int)_mediaPlayer.frame;
        }
        public override double GetCurrentTime()
        {
            return _mediaPlayer.time;
        }

        public override double GetDuration()
        {
            return _mediaPlayer.clip.length;
        }

        public override Texture GetTexture()
        {
            return _mediaPlayer.texture;
        }
        public override bool IsTextureFlipped ()
        {
            return false;
        }
        public override bool IsExternalTexture()
        {
            return false;
        }

        public override bool IsPlaying()
        {
            return _mediaPlayer.isPlaying;
        }

        public override void RemoveComponents()
        {
            if(!Application.isPlaying)
            {
                DestroyImmediate(_mediaPlayer, true);
                DestroyImmediate(_audioSource, true);
                DestroyImmediate(this, true);
            }
            else
            {
                Destroy(_mediaPlayer);
                Destroy(_audioSource);
                Destroy(this);
            }
        }

        public override AvailablePlayerType GetPlayerType()
        {
            return AvailablePlayerType.UnityVideoPlayer;
        }

        public UnityEngine.Video.VideoPlayer GetPlayerBackend()
        {
            return _mediaPlayer;
        }
    }
}