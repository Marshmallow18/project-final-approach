using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TiledMapParserExtended;

namespace GXPEngine
{
    public class FlashBackTriggersManager : GameObject
    {
        public static FlashBackTriggersManager Instance;
        
        private Dictionary<string, FlashBackTrigger> _flashTriggersMap;

        private MapGameObject _map;
        private BaseLevel _level;

        public FlashBackTriggersManager(MapGameObject pMap, BaseLevel pLevel) : base(false)
        {
            Instance = this;
            _flashTriggersMap = new Dictionary<string, FlashBackTrigger>();
            _level = pLevel;
            _map = pMap;

            var flashesData = _map.ObjectGroups.SelectMany(og => og.Objects)
                .Where(tileObj => tileObj?.Type.ToLower() == "flashback");

            foreach (var flash in flashesData)
            {
                var flashTrigger = AddFlashbackTriggerToLevel(flash);
                _flashTriggersMap.Add(flashTrigger.name.ToLower(), flashTrigger);

            }
        }
        
        FlashBackTrigger AddFlashbackTriggerToLevel(TiledObject flashData)
        {
            string objUniqueName = flashData.Name.Trim().ToLower();

            int counter = 0;
            while (_flashTriggersMap.ContainsKey(objUniqueName))
            {
                objUniqueName = flashData.Name.Trim() + "_" + counter;
                counter++;
            }

            flashData.Name = objUniqueName;

            var flashTrigger = new FlashBackTrigger("data/Flashback Pickup.png", flashData)
            {
                name = objUniqueName
            };
            
            _level.AddChild(flashTrigger);
            flashTrigger.width = Mathf.Round(flashData.Width);
            flashTrigger.height = Mathf.Round(flashData.Height);
            flashTrigger.SetOrigin(0, flashTrigger.texture.height); //Tmx use origin left,bottom :/
            flashTrigger.rotation = flashData.rotation;
            flashTrigger.SetXY(flashData.X, flashData.Y);

            return flashTrigger;
        }
        
        public Dictionary<string, FlashBackTrigger> FlashTriggersMap => _flashTriggersMap;
    }
}