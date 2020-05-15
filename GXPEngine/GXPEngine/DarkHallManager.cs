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

        private Dictionary<string, DarkHall> _darkTriggersMap;

        private BaseLevel _level; //the level

        public DarkHallManager(MapGameObject pMap, BaseLevel pLevel) : base(false)
        {
            _darkTriggersMap = new Dictionary<string, DarkHall>();
            _map = pMap;
            _level = pLevel;

            //Load doors objects from Map
            var darkTriggersData = _map.ObjectGroups.SelectMany(og => og.Objects)
                .Where(tileObj => tileObj?.Type.ToLower() == "dark");

            //Creates DoorTrigger Game Objects in scene, IGNORE if doesn't exist a door with the same name "NameofTheDoor Trigger"
            foreach (var memData in darkTriggersData)
            {
                AddDarkHall(memData.Name, memData.X, memData.Y, memData.rotation, memData.Width, memData.Height);
            }

        }

        private void AddDarkHall(string pName, float pX, float pY, float rot, float pWidth, float pHeight)
        {
            string numberString = pName.Replace("Dark Hall ", "");

            var darkHall = new DarkHall("data/darkhall"+ numberString +".png");
            _darkTriggersMap.Add(pName, darkHall);

            darkHall.width = Mathf.Round(pWidth);
            darkHall.height = Mathf.Round(pHeight);
            darkHall.SetOrigin(0, darkHall.texture.height);

            _level.AddChild(darkHall);
            darkHall.rotation = rot;
            darkHall.SetXY(pX, pY);

            Console.WriteLine($"{darkHall}: {darkHall.scaleX} | {darkHall.scaleY}");
        }
    }
}
