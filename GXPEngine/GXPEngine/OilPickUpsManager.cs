using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GXPEngine
{
    public class OilPickUpsManager : GameObject
    {
        private Dictionary<string, OilPickUp> _pickupsMap;

        private MapGameObject _map;
        private BaseLevel _level;

        
        public OilPickUpsManager(MapGameObject pMap, BaseLevel pLevel) : base(false)
        {
            _pickupsMap = new Dictionary<string, OilPickUp>();

            _level = pLevel;
            _map = pMap;

            var oilData = _map.ObjectGroups.SelectMany(og => og.Objects)
                .Where(tileObj => tileObj?.Type.ToLower() == "oil");

            foreach (var memData in oilData)
            {
                int oilType = memData.GetIntProperty("Spawn related to 1/2/3 :", 0);
                int numType = memData.GetIntProperty("Number of type");

                AddOilPickupToLevel(numType, oilType, memData.Name,"", memData.X, memData.Y, memData.rotation, memData.Width, memData.Height);
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

        void AddOilPickupToLevel(int numType, int oilType, string pName, string oilImageFileName, float pX, float pY, float pRotation, float pWidth, float pHeight)
        {
            string objUniqueName = pName.Trim();

            int counter = 0;
            while (_pickupsMap.ContainsKey(objUniqueName.ToLower()))
            {
                objUniqueName = pName.Trim() + "_" + counter;
                counter++;
            }

            var oil = new OilPickUp( oilType, oilImageFileName, "data/Oil Pickup.png", 1, 1)
            {
                name = objUniqueName
            };


            //spawnOilPickup(oilType);
            _pickupsMap.Add(objUniqueName.ToLower(), oil);

            _level.AddChild(oil);
            oil.width = Mathf.Round(pWidth);
            oil.height = Mathf.Round(pHeight);
            oil.SetOrigin(0, oil.texture.height);
            oil.rotation = pRotation;
            oil.SetXY(pX, pY);
        }

        private OilPickUp spawnOilPickup (int type)
        {
            List<OilPickUp> candidates = new List<OilPickUp>();
            foreach (OilPickUp oilpickup in _pickupsMap.Values.ToList())
            {
                if (oilpickup._oilType == type) candidates.Add(oilpickup);
            }

            return candidates[Utils.Random(0, candidates.Count)];
        }


    }
}