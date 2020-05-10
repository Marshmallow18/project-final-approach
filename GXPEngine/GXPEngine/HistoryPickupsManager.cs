using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GXPEngine
{
    public class HistoryPickupsManager : GameObject
    {
        private Dictionary<string, HistoryPickUp> _pickupsMap;

        private MapGameObject _map;
        private BaseLevel _level;
        
        public HistoryPickupsManager(MapGameObject pMap, BaseLevel pLevel) : base(false)
        {
            _pickupsMap = new Dictionary<string, HistoryPickUp>();
            _level = pLevel;
            _map = pMap;

            var historicData = _map.ObjectGroups.SelectMany(og => og.Objects)
                .Where(tileObj => tileObj?.Type.ToLower() == "history");

            foreach (var memData in historicData)
            {
                string historyImageFileName = memData.GetStringProperty("history_image", "data/history images/No Image.png");
                
                AddHistoryPickupToLevel(memData.Name, historyImageFileName, memData.X, memData.Y, memData.rotation, memData.Width, memData.Height);
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
        }

        void AddHistoryPickupToLevel(string pName, string historyImageFileName, float pX, float pY, float pRotation, float pWidth, float pHeight)
        {
            string objUniqueName = pName.Trim();

            int counter = 0;
            while (_pickupsMap.ContainsKey(objUniqueName.ToLower()))
            {
                objUniqueName = pName.Trim() + "_" + counter;
                counter++;
            }
            
            var history = new HistoryPickUp(historyImageFileName, "data/History Pickup.png", 1, 1)
            {
                name = objUniqueName
            };
            
            
            _pickupsMap.Add(objUniqueName.ToLower(), history);
            
            _level.AddChild(history);
            history.width = Mathf.Round(pWidth);
            history.height = Mathf.Round(pHeight);
            history.SetOrigin(0, history.texture.height);
            history.rotation = pRotation;
            history.SetXY(pX, pY);
        }
    }
}