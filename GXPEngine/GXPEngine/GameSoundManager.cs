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
        
        private string _currentMusic = "";
        private string _lastMusic = "";

        public GameSoundManager(BaseLevel pLevel) : base(false)
        {
            Instance = this;

            _sfxsLevelMap = new Dictionary<string, Sound>();
            _sfxsLoopLevelMap = new Dictionary<string, Sound>();
            _musicsLevelMap = new Dictionary<string, Sound>();

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

            Console.WriteLine();

            //Read all audio files in Objects
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

            _currentMusicChannel = _musicsLevelMap[musicKey].Play(false, 0, vol);
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
            CoroutineManager.StartCoroutine(FadeOutMusicRoutine(duration), this);
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

        public void PlayFx(string fxKey, float vol = 1)
        {
            if (!_sfxsLevelMap.ContainsKey(fxKey)) return;

            uint nextChannel = (_currentChanneId++) % 31;
            _currentFxChannel = _sfxsLevelMap[fxKey].Play(false, nextChannel, vol);
        }
        
        public void PlayFxLoop(string fxKey, float vol = 1)
        {
            if (!_sfxsLoopLevelMap.ContainsKey(fxKey)) return;

            uint nextChannel = (_currentChanneId++) % 31;
            _currentLoopFxChannel = _sfxsLoopLevelMap[fxKey].Play(false, nextChannel, vol);
        }

        public void StopCurrentFxLoop()
        {
            _currentLoopFxChannel?.Stop();
        }

        public Dictionary<string, Sound> SfxsLoopLevelMap => _sfxsLoopLevelMap;

        public Dictionary<string, Sound> SfxsLevelMap => _sfxsLevelMap;

        public Dictionary<string, Sound> MusicsLevelMap => _musicsLevelMap;

        public string LastMusic => _lastMusic;
    }
}