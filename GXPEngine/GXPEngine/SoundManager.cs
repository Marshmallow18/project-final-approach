﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace GXPEngine
{
    public class SoundManager : GameObject
    {
        private static SoundManager _instance;

        private static object _lock = new object();

        private Sound[] _musics = new Sound[]
        {
            new Sound("data/Theme music.ogg", true, true),
            new Sound("data/Game over.mp3", true, true),
            new Sound("data/in-game.ogg", true, true),
            // new Sound("data/Music2.ogg", true, true),
            // new Sound("data/Music3.ogg", true, true),
        };

        private SoundChannel _currentMusicChannel;

        private Sound[] _fxs = new Sound[]
        {
            new Sound("data/Airplane Engine.wav", true, false), //0
            new Sound("data/Drone flying.wav", true, false), //1
            new Sound("data/BirdFlap.wav", false, false), //2
            new Sound("data/Sniper Shot.wav", false, false), //3
            new Sound("data/Rifle Reload.wav", false, false), //4
            new Sound("data/PizzaDeliveryPLIIIINNGGG.wav", false, false), //5
            new Sound("data/Bird Hit.wav", false, false), //6
            new Sound("data/Drone Lock.wav", false, false), //7
            new Sound("data/whirlwind.wav", true, false), //8
            new Sound("data/Stork Sucked by Tornado.wav", false, false), //9
        };

        private float[] _fxVolumes = new float[]
        {
            -1, //0
            -1, //1
            -1, //2
            -1, //3
            -1, //4
            0.10f, //5
            -1, //6
            0.05f, //7 Drone Detect Player
            -1, //8 tornado
            0.20f, //9
        };

        private Sound[] _explosionFxs = new Sound[]
        {
            //new Sound("data/110115__ryansnook__small-explosion.ogg", false, false),
        };

        public Sound[] _hudFxs = new Sound[]
        {
            // new Sound("data/171697__nenadsimic__menu-selection-click.wav", false, false), //0
            // new Sound("data/150222__pumodi__menu-select.wav", false, false), //1
            // new Sound("data/beep1.wav", false, false), //2
            // new Sound("data/beep2 long.wav", false, false), //3
            // new Sound("data/totya__children-yeah-long-and-laugh.ogg", false, true), //4
            // new Sound("data/kids-boo.ogg", false, true), //5
        };

        private Dictionary<Type, SoundChannel[]> _fxChannelsMap;

        private SoundChannel[] _fxChannels;
        private SoundChannel[] _fxExplosionChannels;
        private SoundChannel[] _fxHudChannels;
        private bool _isSoundEnabled = true;

        private SoundManager()
        {
            _fxChannelsMap = new Dictionary<Type, SoundChannel[]>();

            _fxChannels = new SoundChannel[_fxs.Length];
            _fxExplosionChannels = new SoundChannel[_explosionFxs.Length];
            _fxHudChannels = new SoundChannel[_hudFxs.Length];
        }

        public void PlayFx(int soundId)
        {
            soundId = soundId % _fxs.Length;

            if (_fxChannels[soundId] != null && _fxChannels[soundId].IsPlaying)
            {
                _fxChannels[soundId].Stop();
            }

            _fxChannels[soundId] = _fxs[soundId].Play(true);

            if (_fxVolumes[soundId] > 0)
            {
                _fxChannels[soundId].Volume = _fxVolumes[soundId];
            }

            _fxChannels[soundId].IsPaused = false;
        }

        public bool IsFxPlaying(Type type, int soundId)
        {
            var channel = _fxChannelsMap[type][soundId];

            return channel.IsPlaying;
        }

        // public void SetEngineFxPitch(int index, float pitch)
        // {
        //     if (_fxChannelsMap[index] == null)
        //     {
        //         PlayFx(index);
        //     }
        //
        //     _fxChannelsMap[index].Frequency = pitch;
        // }

        public SoundChannel SetFxVolume(uint soundId, float vol)
        {
            if (_isSoundEnabled == false) return null;

            if (_fxChannels[soundId] == null || _fxChannels[soundId].IsPlaying == false)
            {
                _fxChannels[soundId] = _fxs[soundId].Play(true, 0, vol);
            }

            if (_fxChannels[soundId].IsPaused)
            {
                _fxChannels[soundId].Volume = vol;
                _fxChannels[soundId].IsPaused = false;
            }
            else if (_fxChannels[soundId].IsPlaying)
            {
                _fxChannels[soundId].Volume = vol;
            }

            return _fxChannels[soundId];
        }

        public void PlayExplosionFx(int index = -1)
        {
            if (index == -1)
            {
                index = MRandom.Range(0, _explosionFxs.Length);
            }
            else
            {
                index = index % _explosionFxs.Length;
            }

            if (_fxExplosionChannels[index] != null && _fxExplosionChannels[index].IsPlaying)
                _fxExplosionChannels[index].Stop();

            _fxExplosionChannels[index] = _explosionFxs[index].Play();
        }

        public void PlayHudFx(int index = -1)
        {
            if (index == -1)
            {
                index = MRandom.Range(0, _hudFxs.Length);
            }
            else
            {
                index = index % _hudFxs.Length;
            }

            _fxHudChannels[index] = _hudFxs[index].Play();
        }

        public void PlayMusic(int index, float vol = 0.07f)
        {
            if (_currentMusicChannel != null && _currentMusicChannel.IsPlaying)
            {
                _currentMusicChannel.Stop();
            }

            index = Mathf.Abs(index % _musics.Length);

            _currentMusicChannel = _musics[index].Play(false, 0, vol);
        }

        public void StopMusic()
        {
            if (_currentMusicChannel != null && _currentMusicChannel.IsPlaying)
            {
                _currentMusicChannel.Stop();
            }
        }

        public void SetCurrentMusicVolume(float vol)
        {
            _currentMusicChannel.Volume = vol;
        }

        public void FadeInMusic(int musicId, float vol, int duration = 500)
        {
            CoroutineManager.StartCoroutine(FadeInMusicRoutine(musicId, vol, duration), this);
        }

        private IEnumerator FadeInMusicRoutine(int musicId, float vol, int duration)
        {
            int time = 0;
            float currentVolume = 0;
            float fadeInSpeed = vol / duration;

            PlayMusic(1, 0);
            
            while (time < duration)
            {
                currentVolume += fadeInSpeed * Time.deltaTime;
                SetCurrentMusicVolume(currentVolume);
                yield return null;

                time += Time.deltaTime;
            }

            SetCurrentMusicVolume(vol);
        }

        public void FadeOutCurrentMusic(int duration = 500)
        {
            CoroutineManager.StartCoroutine(FadeOutMusicRoutine(duration), this);
        }

        public void StopAllSounds()
        {
            for (int i = 0; i < _fxChannels.Length; i++)
            {
                if (_fxChannels[i] != null)
                    _fxChannels[i].Stop();
            }

            // foreach (var kv in _fxChannelsMap)
            // {
            //     if (kv.Value == null) continue;
            //     
            //     for (int i = 0; i < kv.Value.Length; i++)
            //     {
            //         kv.Value[i].Stop();
            //     }
            // }
        }

        private IEnumerator FadeOutMusicRoutine(int duration)
        {
            int time = 0;
            float currentVolume = _currentMusicChannel.Volume;
            float faeOutSpeed = _currentMusicChannel.Volume / duration;
            while (time < duration)
            {
                currentVolume -= faeOutSpeed * Time.deltaTime;
                SetCurrentMusicVolume(currentVolume);
                yield return null;

                time += Time.deltaTime;
            }

            SetCurrentMusicVolume(0);
            StopMusic();
        }

        public static SoundManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SoundManager();
                    }

                    return _instance;
                }
            }
        }

        void Update()
        {
            // foreach (var kv in _fxChannelsMap)
            // {
            //     for (int i = 0; i < kv.Value.Length; i++)
            //     {
            //         if (kv.Value[i].IsPlaying == false) continue;
            //         
            //         Console.WriteLine($"{this}: {kv.Key.name} | {kv.Value[i].IsPlaying} | {kv.Value[i].IsPaused} | vol: {kv.Value[i].Volume}");
            //     }
            // }
        }

        public Sound[] Fxs
        {
            get { return _fxs; }
        }

        public bool IsSoundEnabled
        {
            get => _isSoundEnabled;
            set => _isSoundEnabled = value;
        }

        public void DisableAllSounds()
        {
            _isSoundEnabled = false;
            StopAllSounds();
        }
    }
}