using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GXPEngine.Core;
using TiledMapParserExtended;

namespace GXPEngine
{
    public class FlashbackPickupsManager : GameObject
    {
        public static FlashbackPickupsManager Instance;

        private Dictionary<string, FlashbackPickup> _flashPickupsMap;

        private MapGameObject _map;
        private BaseLevel _level;
        private List<string> _flashesPickupsToSkip;

        private FlashbackPickup _finalPickup;

        public FlashbackPickupsManager(MapGameObject pMap, BaseLevel pLevel) : base(false)
        {
            Instance = this;

            _flashPickupsMap = new Dictionary<string, FlashbackPickup>();
            _level = pLevel;
            _map = pMap;

            var flashesData = _map.ObjectGroups.SelectMany(og => og.Objects)
                .Where(tileObj => tileObj?.Type.ToLower() == "flashbackpickup");

            //Get from settings to skip some flashbacks
            _flashesPickupsToSkip = new List<string>();
            if (Settings.Flashback_Pickups_Collected != "0")
            {
                var flashesStr = Settings.Flashback_Pickups_Collected.Trim().Split(' ');
                for (int i = 0; i < flashesStr.Length; i++)
                {
                    string valStr = flashesStr[i].Trim();
                    if (int.TryParse(valStr, out var val))
                    {
                        _flashesPickupsToSkip.Add($"Flashback {val}".ToLower());
                    }
                }
            }

            foreach (var flashData in flashesData)
            {
                var flashPickup = AddFlashbackPickupToLevel(flashData);
                _flashPickupsMap.Add(flashData.Name.ToLower(), flashPickup);
            }

            //Final trigger flashback
            var finalFlashData = _map.ObjectGroups
                .SelectMany(og => og.Objects)
                .FirstOrDefault(tileObj => tileObj?.Type.ToLower() == "finalflashbackpickup");

            if (finalFlashData != null)
            {
                _finalPickup = AddFlashbackPickupToLevel(finalFlashData);
            }
        }

        FlashbackPickup AddFlashbackPickupToLevel(TiledObject flashData)
        {
            string objUniqueName = flashData.Name.Trim();
            flashData.Name = flashData.Name.Replace("Pickup ", "").Trim().ToLower();

            int counter = 0;
            while (_flashPickupsMap.ContainsKey(objUniqueName.ToLower()))
            {
                objUniqueName = flashData.Name.Trim().ToLower() + "_" + counter;
                counter++;
            }

            var flashPickup = new FlashbackPickup("data/Flashback Pickup.png", flashData, 1, 1)
            {
                name = objUniqueName
            };

            _level.AddChild(flashPickup);
            flashPickup.Enabled = false;
            flashPickup.width = Mathf.Round(flashData.Width);
            flashPickup.height = Mathf.Round(flashData.Height);
            flashPickup.SetOrigin(0, flashPickup.texture.height); //Tmx use origin left,bottom :/
            flashPickup.rotation = flashData.rotation;
            flashPickup.SetXY(flashData.X, flashData.Y);

            return flashPickup;
        }

        public void EnableFlashbackPickups()
        {
            var flashPickupsOrdered = _flashPickupsMap.Values.OrderBy(f => f.FlashbackData.Name);
            foreach (var flashbackPickup in flashPickupsOrdered)
            {
                EnablePickup(flashbackPickup);
            }

            EnablePickup(_finalPickup);

            //Simulate pickup for test
            foreach (var flashName in _flashesPickupsToSkip)
            {
                if (!_flashPickupsMap.TryGetValue(flashName, out var pickup))
                {
                    continue;
                }

                FlashbackManager.Instance.PlayerPickedupFlashblack(pickup, false);
            }
        }

        private void EnablePickup(FlashbackPickup pickup)
        {
            pickup.Enabled = true;
            
            DrawableTweener.TweenSpriteAlpha(pickup, 0, 1, Settings.Default_AlphaTween_Duration,
                () => { pickup.Blink(); });
            
            DrawableTweener.TweenScale(pickup, Vector2.one, Vector2.one * 1.1f,
                Settings.Default_AlphaTween_Duration / 2,
                () =>
                {
                    DrawableTweener.TweenScale(pickup, Vector2.one * 1.1f, Vector2.one,
                        Settings.Default_AlphaTween_Duration / 2, () =>
                        {
                            pickup.collider.Enabled = true;
                        });
                });
        }

        void Update()
        {
        }

        public Dictionary<string, FlashbackPickup> FlashPickupsMap => _flashPickupsMap;

        public List<string> FlashesPickupsToSkip => _flashesPickupsToSkip;

        public FlashbackPickup FinalPickup => _finalPickup;
    }
}