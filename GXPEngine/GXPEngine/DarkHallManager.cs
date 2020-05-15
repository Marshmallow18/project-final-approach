using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    public class DarkHallManager : GameObject
    {
        private MapGameObject _map;

        private Dictionary<string, DarkTrigger> _darkTriggersMap;

        private BaseLevel _level; //the level

        public DarkHallManager(MapGameObject pMap, BaseLevel pLevel) : base(false)
        {
            _darkTriggersMap = new Dictionary<string, DarkTrigger>();
            _map = pMap;
            _level = pLevel;

            //Load doors objects from Map
            var darkTriggersData = _map.ObjectGroups.SelectMany(og => og.Objects)
                .Where(tileObj => tileObj?.Type.ToLower() == "dark");

            //Creates DoorTrigger Game Objects in scene, IGNORE if doesn't exist a door with the same name "NameofTheDoor Trigger"
            foreach (var memData in darkTriggersData)
            {
                AddDarkTrigger(memData.Name, memData.X, memData.Y, memData.rotation, memData.Width, memData.Height);
            }

            //CoroutineManager.StartCoroutine(Start(), this);
        }

        IEnumerator Start()
        {
            //Set player objects to collide after player is set
            while (_level.Player == null)
            {
                yield return null;
            }

            //_level.Player.objectsToCheck = _level.Player.objectsToCheck.Concat(_darkMap.Values).ToArray();
        }

        private void AddDarkTrigger(string pName, float pX, float pY, float rot, float pWidth, float pHeight)
        {
            var darkTrigger = new DarkTrigger();
            _darkTriggersMap.Add(pName, darkTrigger);

            darkTrigger.width = Mathf.Round(pWidth);
            darkTrigger.height = Mathf.Round(pHeight);
            darkTrigger.SetOrigin(0, darkTrigger.texture.height);

            _level.LateAddChild(darkTrigger);
            darkTrigger.rotation = rot;
            darkTrigger.SetXY(pX, pY);

            Console.WriteLine($"{darkTrigger}: {darkTrigger.scaleX} | {darkTrigger.scaleY}");
        }
    }
}
