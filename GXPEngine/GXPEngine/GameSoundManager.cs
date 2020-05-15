using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GXPEngine
{
    public class GameSoundManager : GameObject
    {
        public static GameSoundManager Instance;

        private BaseLevel _level;

        private Dictionary<string, Sound> _sfxsLevelMap;
        private Dictionary<string, Sound> _sfxsLoopLevelMap;
        private Dictionary<string, Sound> _musicsLevelMap;

        private SoundChannel _currentMusicChannel;
        private SoundChannel _currentFxChannel;
        private SoundChannel _currentLoopFxChannel;

        private uint _currentChanneId = 0;

        private SoundChannel[] _sfxChannels;

        private string _currentMusic = "";
        private string _lastMusic = "";

        //Used to pause and unpause loop sfxs
        private Dictionary<string, SoundChannel> _loopSfxChannelsMap;

        public GameSoundManager(BaseLevel pLevel) : base(false)
        {
            Instance = this;

            _sfxsLevelMap = new Dictionary<string, Sound>();
            _sfxsLoopLevelMap = new Dictionary<string, Sound>();
            _musicsLevelMap = new Dictionary<string, Sound>();

            _loopSfxChannelsMap = new Dictionary<string, SoundChannel>();

            _sfxChannels = new SoundChannel[31];

            _level = pLevel;

            //Load fixed sounds, sounds that its not set in Tiled
            var stepSound = new Sound("data/audios/sfx/steps_loop.wav", true, false);

            string pattern = Settings.Regex_Audio_Music_Pattern.Replace("\\[", "").Replace("\\]", "");
            Regex rx = new Regex(pattern,
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var path = AppDomain.CurrentDomain.BaseDirectory;

            //This code loads all audio and music set in TileObjects (properties with audio_N or music_N name pattern)
            //Its checks, for audio, if has a property audio_loop_N for each audio, to be created with loop
            //Its checks if file exists too
            var allObjects = _level.LevelMap.MapData.ObjectGroups.SelectMany(og => og.Objects);
            foreach (var tiledObject in allObjects)
            {
                var properties = tiledObject.propertyList?.properties.Where(p => rx.IsMatch(p.Name.Trim()));
                if (properties?.Count() > 0)
                {
                    foreach (var prop in properties)
                    {
                        if (prop.Type == "file" && !string.IsNullOrWhiteSpace(prop.Value))
                        {
                            if (File.Exists(Path.Combine(path, prop.Value)))
                            {
                                if (prop.Name.ToLower().StartsWith("audio_"))
                                {
                                    //check if has a loop property
                                    string audioIndexStr = prop.Name.ToLower().Replace("audio_", "");
                                    if (int.TryParse(audioIndexStr, out int audioIndex))
                                    {
                                        string loopPropName = $"audio_loop_{audioIndex}";
                                        bool hasLoop = tiledObject.GetBoolProperty(loopPropName, false);
                                        if (hasLoop && !_sfxsLoopLevelMap.ContainsKey(prop.Value))
                                        {
                                            var sfxLoop = new Sound(prop.Value, true, false);
                                            _sfxsLoopLevelMap.Add(prop.Value, sfxLoop);
                                        }
                                        else if (!_sfxsLevelMap.ContainsKey(prop.Value))
                                        {
                                            var sfx = new Sound(prop.Value, false, false);
                                            _sfxsLevelMap.Add(prop.Value, sfx);
                                        }
                                    }
                                }
                                else if (!_musicsLevelMap.ContainsKey(prop.Value) &&
                                         prop.Name.ToLower().StartsWith("music_"))
                                {
                                    var music = new Sound(prop.Value, true, true);
                                    _musicsLevelMap.Add(prop.Value, music);
                                }

                                Console.WriteLine($"{this} | name: '{prop.Name}' | v: '{prop.Value}'");
                            }
                        }
                    }
                }
            }

            //Load music from map properties
            var startMusicFilename = _level.LevelMap.MapData.GetStringProperty("level_start_music", null);
            if (startMusicFilename != null && File.Exists(Path.Combine(path, startMusicFilename)))
            {
                _musicsLevelMap.Add(startMusicFilename, new Sound(startMusicFilename, true, true));
            }

            //Load aditional sfx

            //Flashback Picked up sound
            LoadSfxByFilename(Settings.History_Pickedup_SFX);

            //Footsteps
            LoadSfxByFilename(Settings.Footstep1_SFX);
            LoadSfxByFilename(Settings.Footstep2_SFX);

            //Cave ambient
            LoadLoopSfxByFilename(Settings.Cave_Background_Ambient_Sound);
            
            //Door open
            LoadSfxByFilename(Settings.Door0_Open_Sound);

            Console.WriteLine();
        }

        public void PlayMusic(string musicKey, float vol = 0.07f)
        {
            if (!_musicsLevelMap.ContainsKey(musicKey))
            {
                return;
            }

            _lastMusic = (string.IsNullOrWhiteSpace(_currentMusic)) ? musicKey : _currentMusic;
            _currentMusic = musicKey;

            if (_currentMusicChannel != null && _currentMusicChannel.IsPlaying)
            {
                _currentMusicChannel.Stop();
            }

            uint nextChannel = (_currentChanneId++) % 31;
            _currentMusicChannel = _musicsLevelMap[musicKey].Play(false, nextChannel, vol);
        }

        public void StopMusic()
        {
            if (_currentMusicChannel != null && _currentMusicChannel.IsPlaying)
            {
                _currentMusicChannel.Stop();
            }
        }

        void StopMusicChannel(SoundChannel channel)
        {
            if (channel != null && channel.IsPlaying)
                channel.Stop();
        }

        public void SetCurrentMusicVolume(float vol)
        {
            if (_currentMusicChannel != null)
                _currentMusicChannel.Volume = vol;
        }

        void SetMusicChannelVolume(SoundChannel channel, float vol)
        {
            if (channel != null)
                channel.Volume = vol;
        }

        public void FadeInMusic(string key, float vol = 1, int duration = 500)
        {
            CoroutineManager.StartCoroutine(FadeInMusicRoutine(key, vol, duration), this);
        }

        private IEnumerator FadeInMusicRoutine(string musicKey, float vol, int duration)
        {
            int time = 0;
            float currentVolume = 0;
            float fadeInSpeed = vol / duration;

            PlayMusic(musicKey, 0);

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
            if (_currentMusicChannel != null)
            {
                var fadeOutChannel = _currentMusicChannel;
                CoroutineManager.StartCoroutine(FadeOutMusicRoutine(fadeOutChannel, duration), this);
            }
        }

        private IEnumerator FadeOutMusicRoutine(SoundChannel fadeOutChannel, int duration)
        {
            int time = 0;
            float currentVolume = fadeOutChannel.Volume;
            float faeOutSpeed = fadeOutChannel.Volume / duration;
            while (time < duration)
            {
                currentVolume -= faeOutSpeed * Time.deltaTime;
                SetMusicChannelVolume(fadeOutChannel, currentVolume);
                yield return null;

                time += Time.deltaTime;
            }

            SetMusicChannelVolume(fadeOutChannel, 0);
            StopMusicChannel(fadeOutChannel);
        }

        public SoundChannel PlayFx(string fxKey, float vol = 1)
        {
            if (!_sfxsLevelMap.ContainsKey(fxKey)) return null;

            uint nextChannel = (_currentChanneId++) % 31;
            _sfxChannels[nextChannel] = _sfxsLevelMap[fxKey].Play(false, nextChannel, vol);
            _currentFxChannel = _sfxChannels[nextChannel];
            return _sfxChannels[nextChannel];
        }

        public void PlayFxLoop(string fxKey, float vol = 1)
        {
            if (!_sfxsLoopLevelMap.ContainsKey(fxKey)) return;

            if (_loopSfxChannelsMap.TryGetValue(fxKey, out var channel))
            {
                if (channel.IsPaused)
                {
                    channel.IsPaused = false;
                    return;
                }
            }
            
            uint nextChannel = (_currentChanneId++) % 31;
            _sfxChannels[nextChannel] = _sfxsLoopLevelMap[fxKey].Play(false, nextChannel, vol);
            _currentLoopFxChannel = _sfxChannels[nextChannel];

            if (_loopSfxChannelsMap.ContainsKey(fxKey))
            {
                _loopSfxChannelsMap[fxKey] = _sfxChannels[nextChannel];
            }
            else
            {
                _loopSfxChannelsMap.Add(fxKey, _sfxChannels[nextChannel]);
            }
        }

        public void StopFxLoopSound(string fxKey)
        {
            if (_loopSfxChannelsMap.TryGetValue(fxKey, out var channel))
            {
                if (channel.IsPlaying)
                {
                    channel.Stop();
                }
            }
        }
        
        public void PauseFxLoopSound(string fxKey)
        {
            if (_loopSfxChannelsMap.TryGetValue(fxKey, out var channel))
            {
                if (channel.IsPlaying)
                {
                    channel.IsPaused = true;
                }
            }
        }
        
        public void StopCurrentFxLoop()
        {
            _currentLoopFxChannel?.Stop();
        }

        public Dictionary<string, Sound> SfxsLoopLevelMap => _sfxsLoopLevelMap;

        public Dictionary<string, Sound> SfxsLevelMap => _sfxsLevelMap;

        public Dictionary<string, Sound> MusicsLevelMap => _musicsLevelMap;

        public string LastMusic => _lastMusic;

        public void StopAllSounds()
        {
            StopAllLoopFxs();
            StopAllFxs();

            if (_currentMusicChannel != null)
                FadeOutCurrentMusic();
        }

        void StopAllLoopFxs()
        {
            if (_currentLoopFxChannel == null)
                return;

            if (_currentLoopFxChannel.IsPlaying || _currentLoopFxChannel.IsPaused)
                _currentLoopFxChannel?.Stop();
        }

        void StopAllFxs()
        {
            if (_currentFxChannel == null)
                return;

            if (_currentFxChannel.IsPlaying || _currentFxChannel.IsPaused)
                _currentFxChannel?.Stop();
        }

        void LoadSfxByFilename(string filename)
        {
            if (!_sfxsLevelMap.ContainsKey(filename))
            {
                _sfxsLevelMap.Add(filename, new Sound(filename, false, false));
            }
        }

        void LoadLoopSfxByFilename(string filename)
        {
            if (!_sfxsLoopLevelMap.ContainsKey(filename))
            {
                _sfxsLoopLevelMap.Add(filename, new Sound(filename, true, false));
            }
        }
    }
}