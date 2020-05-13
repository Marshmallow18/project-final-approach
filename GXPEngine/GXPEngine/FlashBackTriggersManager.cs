using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TiledMapParserExtended;

namespace GXPEngine
{
    public class FlashBackTriggersManager : GameObject
    {
        private Dictionary<string, FlashBackTrigger> _flashTriggersMap;

        private MapGameObject _map;
        private BaseLevel _level;

        public FlashBackTriggersManager(MapGameObject pMap, BaseLevel pLevel) : base(false)
        {
            _flashTriggersMap = new Dictionary<string, FlashBackTrigger>();
            _level = pLevel;
            _map = pMap;

            var flashesData = _map.ObjectGroups.SelectMany(og => og.Objects)
                .Where(tileObj => tileObj?.Type.ToLower() == "flashback");

            foreach (var flash in flashesData)
            {
                AddFlashbackTriggerToLevel(flash);
            }
        }
        
        void AddFlashbackTriggerToLevel(TiledObject flashData)
        {
            string objUniqueName = flashData.Name.Trim();

            int counter = 0;
            while (_flashTriggersMap.ContainsKey(objUniqueName.ToLower()))
            {
                objUniqueName = flashData.Name.Trim() + "_" + counter;
                counter++;
            }

            var flashTrigger = new FlashBackTrigger("data/Flashback Pickup.png", flashData)
            {
                name = objUniqueName
            };
            
            _flashTriggersMap.Add(objUniqueName.ToLower(), flashTrigger);

            _level.AddChild(flashTrigger);
            flashTrigger.width = Mathf.Round(flashData.Width);
            flashTrigger.height = Mathf.Round(flashData.Height);
            flashTrigger.SetOrigin(0, flashTrigger.texture.height); //Tmx use origin left,bottom :/
            flashTrigger.rotation = flashData.rotation;
            flashTrigger.SetXY(flashData.X, flashData.Y);
        }
        
        public Dictionary<string, FlashBackTrigger> FlashTriggersMap => _flashTriggersMap;
    }
}