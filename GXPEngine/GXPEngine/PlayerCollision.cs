using System;
using System.Drawing;
using System.Linq;
using GXPEngine.Components;
using GXPEngine.Core;
using GXPEngine.Extensions;

namespace GXPEngine
{
    public class PlayerCollision : GameObject
    {
        private Player _player;
        private CaveLevelMapGameObject _caveLevel;
        private MCamera _cam;

        /// <summary>
        /// Debug fields
        /// </summary>
        private EasyDraw _debugColl;

        private EasyDraw _debugPOI;
        private Arrow _debugNormalPOI;

        public PlayerCollision(Player pPlayer, CaveLevelMapGameObject pCaveLevel, MCamera pCam)
        {
            _player = pPlayer;
            _caveLevel = pCaveLevel;
            _cam = pCam;
        }

        void Update()
        {
            var isWalkable = _caveLevel.IsWalkablePosition(_player.Position) || Settings.Cheat_Mode;

            if (!isWalkable)
            {
                var nextPos = _caveLevel.GetCollisionPOI(_player.Position, _player.lastPos);

                var normalCollision = _caveLevel.GetCollisionNormal(nextPos);

                //Console.WriteLine($"nextpos: {nextPos} | normal: {normalCollision}");

                _player.SetXY(_player.lastPos.x, _player.lastPos.y);

                _debugPOI?.SetXY(nextPos.x, nextPos.y);

                if (_debugNormalPOI != null)
                {
                    _debugNormalPOI.startPoint = _cam.WorldPositionToScreenPosition(nextPos);
                    _debugNormalPOI.vector = normalCollision;
                }
            }
        }

        public void ToogleDebug()
        {
            _debugColl = game.GetChildren().FirstOrDefault(g => g.name == "Debug_Coll") as EasyDraw;
            if (_debugColl == null)
            {
                _debugColl = new EasyDraw(2, 2, false)
                {
                    name = "Debug_Coll"
                };
                _debugColl.CentralizeOrigin();
                _debugColl.Clear(Color.GreenYellow);
                game.AddChild(_debugColl);
            }
            else
            {
                _debugColl?.Destroy();
            }

            _debugPOI = game.GetChildren().FirstOrDefault(g => g.name == "Debug_POI") as EasyDraw;
            if (_debugPOI == null)
            {
                _debugPOI = new EasyDraw(9, 9, false)
                {
                    name = "Debug_POI"
                };
                _debugPOI.CentralizeOrigin();
                _debugPOI.Clear(Color.FromArgb(50, Color.Red));
                game.AddChild(_debugPOI);
            }
            else
            {
                _debugPOI?.Destroy();
            }

            _debugNormalPOI = game.GetChildren().FirstOrDefault(g => g.name == "Debug_Normal_POI") as Arrow;
            if (_debugNormalPOI == null)
            {
                _debugNormalPOI = new Arrow(Vector2.zero, Vector2.one, 100, (uint) Color.HotPink.ToArgb())
                {
                    name = "Debug_Normal_POI"
                };
                game.AddChild(_debugNormalPOI);
            }
            else
            {
                _debugNormalPOI?.Destroy();
            }
        }
    }
}