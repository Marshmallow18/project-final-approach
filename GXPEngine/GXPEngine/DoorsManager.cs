using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GXPEngine.Core;

namespace GXPEngine
{
    public class DoorsManager : GameObject
    {
        private MapGameObject _map;
        
        private Dictionary<string, Door> _doorsMap;
        private Dictionary<string, DoorTrigger> _doorsTriggersMap;

        private BaseLevel _level; //the level
        
        public DoorsManager(MapGameObject pMap, BaseLevel pLevel) : base(false)
        {
            _doorsMap = new Dictionary<string, Door>();
            _doorsTriggersMap = new Dictionary<string, DoorTrigger>();
            _map = pMap;
            _level = pLevel;

            //Load doors objects from Map

            var doorsData = _map.ObjectGroups.SelectMany(og => og.Objects).Where(tileObj => !string.IsNullOrWhiteSpace(tileObj.Name) && !string.IsNullOrWhiteSpace(tileObj.Type) && (tileObj.Type?.ToLower() == "door" || tileObj.Type?.ToLower() == "dooronesided"));
            var doorsTriggersData = _map.ObjectGroups.SelectMany(og => og.Objects).Where(tileObj => !string.IsNullOrWhiteSpace(tileObj.Name) && !string.IsNullOrWhiteSpace(tileObj.Type) && tileObj.Type?.ToLower() == "doortrigger").ToDictionary(k => k.Name.Trim().ToLower().Replace(" trigger", ""), v => v);

            //Creates Door Game Objects in scene
            //Converts names to lowercase to prevent incorrect camelcase input in Tiled object names
            foreach (var doorData in doorsData)
            {
                doorData.Name = doorData.Name.ToLower();

                bool isOpenForever = doorData.GetBoolProperty("open forever", true);
                bool isOneSidedDoor = doorData.Type.Trim().ToLower() == "dooronesided";
                
                AddDoor(doorData.Name, isOpenForever, isOneSidedDoor, doorData.X, doorData.Y, doorData.rotation, doorData.Width, doorData.Height);
            }
            
            //Creates DoorTrigger Game Objects in scene, IGNORE if doesn't exist a door with the same name "NameofTheDoor Trigger"
            foreach (var kv in doorsTriggersData)
            {
                var doorTData = kv.Value;
                doorTData.Name = doorTData.Name.ToLower();

                bool isDarkTrigger = doorTData.GetBoolProperty("dark_trigger", false);

                if (_doorsMap.TryGetValue(kv.Key, out var door))
                {
                    AddDoorTrigger(kv.Key, isDarkTrigger, door, doorTData.X, doorTData.Y, doorTData.rotation, doorTData.Width, doorTData.Height);
                }
            }

            CoroutineManager.StartCoroutine(Start(), this);
        }

        IEnumerator Start()
        {
            //Set player objects to collide after player is set
            while (_level.Player == null)
            {
                yield return null;
            }

            _level.Player.objectsToCheck = _level.Player.objectsToCheck.Concat(_doorsMap.Values).ToArray();
        }

        private void AddDoor(string doorName, bool isOpenForever, bool isOneSided, float pX, float pY, float rot, float pWidth, float pHeight)
        {
            var door = new Door(isOneSided, "data/Door0.png", 2, 2)
            {
                name = doorName,
                IsOpenForever = isOpenForever
                
            };
            
            _doorsMap.Add(doorName, door);
            
            _level.AddChild(door);
            door.width = Mathf.Round(pWidth);
            door.height = Mathf.Round(pHeight);
            door.SetOrigin(0, door.texture.height / 2f); //2 rows
            door.rotation = rot;
            door.SetXY(pX, pY);
        }

        private void AddDoorTrigger(string pName, bool pIsDarkTrigger, Door door, float pX, float pY, float rot, float pWidth, float pHeight)
        {
            var doorTrigger = new DoorTrigger(door, pIsDarkTrigger);
            _doorsTriggersMap.Add(pName, doorTrigger);   
            
            doorTrigger.width = Mathf.Round(pWidth);
            doorTrigger.height = Mathf.Round(pHeight);
            doorTrigger.SetOrigin(0, doorTrigger.texture.height);
            
            _level.AddChild(doorTrigger);
            doorTrigger.rotation = rot;
            doorTrigger.SetXY(pX, pY);
        }
    }
}