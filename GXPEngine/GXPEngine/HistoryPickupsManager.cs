using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GXPEngine
{
    public class MemoriesManager : GameObject
    {
        private Dictionary<string, HistoryPickUp> _memoriesMap;

        private MapGameObject _map;
        private BaseLevel _level;
        
        public MemoriesManager(MapGameObject pMap, BaseLevel pLevel) : base(false)
        {
            _memoriesMap = new Dictionary<string, HistoryPickUp>();
            _level = pLevel;
            _map = pMap;

            var memoriesData = _map.ObjectGroups.SelectMany(og => og.Objects)
                .Where(tileObj => !string.IsNullOrWhiteSpace(tileObj?.Name) && !string.IsNullOrWhiteSpace(tileObj?.Type) && tileObj?.Type.ToLower() == "memory");

            foreach (var memData in memoriesData)
            {
                AddMemoryToLevel(memData.Name, memData.X, memData.Y, memData.rotation, memData.Width, memData.Height);
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

            //_level.Player.objectsToCheck = _level.Player.objectsToCheck.Concat(_memoriesMap.Values).ToArray();
        }

        void AddMemoryToLevel(string pName, float pX, float pY, float pRotation, float pWidth, float pHeight)
        {
            var memory = new HistoryPickUp("data/Memory Pickup.png", 1, 1)
            {
                name = pName.Trim()
            };
            _memoriesMap.Add(pName.Trim().ToLower(), memory);
            
            _level.AddChild(memory);
            memory.width = Mathf.Round(pWidth);
            memory.height = Mathf.Round(pHeight);
            memory.SetOrigin(0, memory.texture.height);
            memory.rotation = pRotation;
            memory.SetXY(pX, pY);
        }
    }
}