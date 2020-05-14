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
                .SelectMany(og => og.Objects).FirstOrDefault(tileObj => tileObj?.Type.ToLower() == "finalflashbackpickup");

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
            foreach (var kv in _flashPickupsMap)
            {
                EnablePickup(kv.Value);
            }
            
            EnablePickup(_finalPickup);
        }

        private void EnablePickup(FlashbackPickup pickup)
        {
            pickup.collider.Enabled = true;

            //If is set in Setting for test, simulate that those flash pickups were taken
            if (_flashesPickupsToSkip.Contains(pickup.FlashbackData.Name))
            {
                FlashbackManager.Instance.PlayerPickedupFlashblack(pickup, false);
                return;
            }

            pickup.Enabled = true;
            DrawableTweener.TweenSpriteAlpha(pickup, 0, 1, Settings.Default_AlphaTween_Duration, () => { pickup.Blink(); });
            DrawableTweener.TweenScale(pickup, Vector2.one, Vector2.one * 1.1f,
                Settings.Default_AlphaTween_Duration / 2,
                () =>
                {
                    DrawableTweener.TweenScale(pickup, Vector2.one * 1.1f, Vector2.one,
                        Settings.Default_AlphaTween_Duration / 2, null);
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