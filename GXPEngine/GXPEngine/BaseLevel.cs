﻿using System.Linq;
using GXPEngine.Components;
using GXPEngine.Core;

namespace GXPEngine
{
    public class BaseLevel : GameObject
    {
        private CaveLevelMapGameObject _caveLevelMap;
        private Player _player;
        private FollowCamera _cam;
        private PlayerCollision _playerCollision;

        public BaseLevel(CaveLevelMapGameObject pCaveLevel, MCamera pCam)
        {
            _caveLevelMap = pCaveLevel;
            _cam = (FollowCamera)pCam;
            
            AddChild(_caveLevelMap);
            
            var doorsManager = new DoorsManager(_caveLevelMap, this);
            AddChild(doorsManager);
            
            var memoriesManager = new HistoryPickupsManager(_caveLevelMap, this);
            AddChild(memoriesManager);

            var oilPickUpManager = new OilPickUpsManager(_caveLevelMap, this);
            AddChild(oilPickUpManager);
            
            var flashbacksTriggerManager = new FlashBackTriggersManager(_caveLevelMap, this);
            AddChild(flashbacksTriggerManager);

            var flashbackPickupsManager = new FlashbackPickupsManager(_caveLevelMap, this);
            AddChild(flashbackPickupsManager);
            
            var flashbacksManager = new FlashbackManager(this, flashbacksTriggerManager.FlashTriggersMap.Count);
            AddChild(flashbacksManager);

            var darkHallManager = new DarkHallManager(_caveLevelMap, this);
            AddChild(darkHallManager);

            var particlesManager = new ParticleManager();

            _player = new Player();
            //_player.SetOrigin(_player.width / 2, 78);
            AddChild(_player);

            var spawnPoint = GetPlayerSpawnPoint();
            _player.SetXY(spawnPoint.x, spawnPoint.y);

            AddChild(_cam);
            
            _cam.Target = _player;
            _cam.Map = _caveLevelMap;
            _cam.SetXY(1000, 1000);
            
            _playerCollision = new PlayerCollision(_player, _caveLevelMap, _cam);
            AddChild(_playerCollision);
            
            // var fogsystem = new FogSystemManager(_cam, _player);
            // AddChild(fogsystem);
        }

        private Vector2 GetPlayerSpawnPoint()
        {
            var spawnObjData = _caveLevelMap?.ObjectGroups.SelectMany(og => og.Objects)
                .FirstOrDefault(tObj => tObj.Type.Trim().ToLower() == "playerspawnpoint");

            if (spawnObjData != null)
            {
                return new Vector2(spawnObjData.X + spawnObjData.Width/2f, spawnObjData.Y - spawnObjData.Height / 2f );
            }
            
            return Vector2.zero;
        }

        public Player Player
        {
            get => _player;
            set => _player = value;
        }

        public PlayerCollision PlayerCollision
        {
            get => _playerCollision;
            set => _playerCollision = value;
        }

        public CaveLevelMapGameObject LevelMap
        {
            get => _caveLevelMap;
            set => _caveLevelMap = value;
        }

        void Update()
        {
            float camZoomSpeed = 3;
            
            if (Input.GetKey(Key.C))
            {
                _cam.scale += camZoomSpeed * Time.delta;
            }
            else if (Input.GetKey(Key.E))
            {
                _cam.scale -= camZoomSpeed * Time.delta;
            }
        }
    }
}