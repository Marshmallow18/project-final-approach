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
                var flashesStr = Settings.Flashback_Pickups_Collected.Split(' ');
                for (int i = 0; i < flashesStr.Length; i++)
                {
                    string valStr = flashesStr[i].Trim();
                    if (int.TryParse(valStr, out var val))
                    {
                        _flashesPickupsToSkip.Add($"Flashback Pickup {val}");
                    }
                }
            }

            foreach (var flash in flashesData)
            {
                AddFlashbackPickupToLevel(flash);
            }
        }

        void AddFlashbackPickupToLevel(TiledObject flashData)
        {
            string objUniqueName = flashData.Name.Trim();

            int counter = 0;
            while (_flashPickupsMap.ContainsKey(objUniqueName.ToLower()))
            {
                objUniqueName = flashData.Name.Trim() + "_" + counter;
                counter++;
            }

            var flashPickup = new FlashbackPickup("data/Flashback Pickup.png", flashData, 1, 1)
            {
                name = objUniqueName
            };

            _flashPickupsMap.Add(objUniqueName.ToLower(), flashPickup);

            _level.AddChild(flashPickup);
            flashPickup.Enabled = false;
            flashPickup.width = Mathf.Round(flashData.Width);
            flashPickup.height = Mathf.Round(flashData.Height);
            flashPickup.SetOrigin(0, flashPickup.texture.height); //Tmx use origin left,bottom :/
            flashPickup.rotation = flashData.rotation;
            flashPickup.SetXY(flashData.X, flashData.Y);
        }

        public void EnableFlashbackPickups()
        {
            foreach (var kv in _flashPickupsMap)
            {
                var pck = kv.Value;

                //If is set in Setting for test, simulate that those flash pickups were taken
                if (_flashesPickupsToSkip.Contains(pck.FlashbackData.Name))
                {
                    FlashbackManager.Instance.PlayerPickedupFlashblack(pck);
                    continue;
                }
                
                pck.Enabled = true;
                DrawableTweener.TweenSpriteAlpha(pck, 0, 1, Settings.Default_AlphaTween_Duration, () =>
                {
                    pck.Blink();
                });
                DrawableTweener.TweenScale(pck, Vector2.one, Vector2.one * 1.1f,
                    Settings.Default_AlphaTween_Duration / 2,
                    () =>
                    {
                        DrawableTweener.TweenScale(pck, Vector2.one * 1.1f, Vector2.one,
                            Settings.Default_AlphaTween_Duration / 2, null);
                    });
            }
        }

        void Update()
        {

        }
        
        public Dictionary<string, FlashbackPickup> FlashPickupsMap => _flashPickupsMap;

        public List<string> FlashesPickupsToSkip => _flashesPickupsToSkip;
    }
}